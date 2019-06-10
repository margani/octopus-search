using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusSearch.ConsoleApp.Models;

namespace OctopusSearch.ConsoleApp
{
    public class OctopusApi
    {
        private readonly OctopusRepository _repository;

        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }

        private const string OctopusActionTargetRolesKey = "Octopus.Action.TargetRoles";

        public OctopusApi(string server, string apiKey)
        {
            ApiUrl = server;
            ApiKey = apiKey;

            var endpoint = new OctopusServerEndpoint(server, apiKey);
            _repository = new OctopusRepository(endpoint);
        }

        public FoundProjectVariable[] SearchVariablesFor(string search, bool doSearchInVariableSetsToo = true)
        {
            var findings = new List<FoundProjectVariable>();

            Console.WriteLine("Getting all library variable sets");
            var libraryVariableSets = _repository.LibraryVariableSets.FindAll();

            Console.WriteLine("Getting all projects");
            var projects = _repository.Projects.GetAll();

            Console.Write("Searching project variables...");
            foreach (var project in projects)
            {
                var variables = _repository.VariableSets.Get(project.VariableSetId).Variables;
                if (doSearchInVariableSetsToo)
                {
                    var vs = project
                        .IncludedLibraryVariableSetIds
                        .SelectMany(o => _repository.VariableSets.Get(libraryVariableSets.Single(oo => oo.Id == o).VariableSetId).Variables);

                    variables = variables
                        .Union(vs)
                        .ToList();
                }

                findings.AddRange(
                    from variable in variables
                    where variable.Name?.Contains(search) == true || variable.Value?.Contains(search) == true
                    select new FoundProjectVariable { Project = project.Name, VariableName = variable.Name, VariableValue = variable.Value });
            }

            Console.Write("Searching variable sets...");
            foreach (var variableSet in libraryVariableSets)
            {
                var variables = _repository.VariableSets.Get(variableSet.VariableSetId).Variables;
                findings.AddRange(
                    from variable in variables
                    where variable.Name?.Contains(search) == true || variable.Value?.Contains(search) == true
                    select new FoundProjectVariable { VariableSet = variableSet.Name, VariableName = variable.Name, VariableValue = variable.Value });
            }

            return findings.ToArray();
        }

        public FoundProjectByRole[] GetProjectsByRole(params string[] roles)
        {
            Console.WriteLine("Getting projects...");
            var projects = _repository.Projects.GetAll();

            var findings = new List<FoundProjectByRole>();
            for (var projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                var project = projects[projectIndex];
                if (project.IsDisabled)
                    continue;

                Console.Write($"\rSearching projects {projectIndex + 1} of {projects.Count}...");
                var deploymentProcess = _repository.DeploymentProcesses.Get(project.DeploymentProcessId);
                var allRolesUsed = deploymentProcess.Steps
                    .Where(o => o.Properties.ContainsKey(OctopusActionTargetRolesKey))
                    .SelectMany(o => o.Properties[OctopusActionTargetRolesKey]?.Value.Split(','))
                    .Distinct()
                    .ToArray();

                var foundRoles = roles.Where(o => allRolesUsed.Contains(o)).ToArray();
                if (foundRoles.Any())
                    findings.Add(new FoundProjectByRole { Project = project.Name, FoundRoles = foundRoles, AllRoles = allRolesUsed });
            }

            Console.WriteLine(", Done!");

            return findings.ToArray();
        }

        public FoundProjectByVariableSetName[] GetProjectsUsingAVariableSet(string variableSetId)
        {
            Console.WriteLine("Getting projects...");
            var projects = _repository.Projects.GetAll();

            var findings = new List<FoundProjectByVariableSetName>();
            for (var projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                var project = projects[projectIndex];
                if (project.IsDisabled)
                    continue;

                Console.Write($"\rSearching projects {projectIndex + 1} of {projects.Count}...");

                if (project.IncludedLibraryVariableSetIds.Contains(variableSetId))
                    findings.Add(new FoundProjectByVariableSetName { Project = project.Name });
            }

            Console.WriteLine(", Done!");

            return findings.ToArray();
        }

        public FoundDeploymentTargetsWhichVariableSetIsUsed[] GetDeploymentTargetsWhichThisVariableSetIsUsed(string variableSetId, string environmentId, Func<MachineResource, bool> fExcludes = null)
        {
            Console.WriteLine("Getting environments...");
            var environment = _repository.Environments.Get(environmentId);

            Console.WriteLine("Getting machines...");
            var machines = _repository.Environments.GetMachines(environment)
                .Where(o => fExcludes?.Invoke(o) == false)
                .ToArray();

            Console.WriteLine("Getting projects...");
            var projects = _repository.Projects.GetAll();

            var findings = new List<FoundDeploymentTargetsWhichVariableSetIsUsed>();
            for (var projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                var project = projects[projectIndex];
                if (project.IsDisabled)
                    continue;

                Console.Write($"\rSearching projects {projectIndex + 1} of {projects.Count}...");

                if (project.IncludedLibraryVariableSetIds.Contains(variableSetId))
                {
                    var deploymentProcess = _repository.DeploymentProcesses.Get(project.DeploymentProcessId);
                    var allRolesUsed = deploymentProcess.Steps
                        .Where(o => o.Properties.ContainsKey(OctopusActionTargetRolesKey))
                        .SelectMany(o => o.Properties[OctopusActionTargetRolesKey]?.Value.Split(','))
                        .Distinct()
                        .ToArray();

                    var targetMachines = machines.Where(o => allRolesUsed.Any(oo => o.Roles.Any(ooo => ooo == oo))).Distinct().ToArray();
                    if (targetMachines.Any())
                        findings.Add(new FoundDeploymentTargetsWhichVariableSetIsUsed { Project = project.Name, Machines = targetMachines.Select(o => o.Name.ToLower()).OrderBy(o => o).ToArray() });
                }
            }

            Console.WriteLine(", Done!");

            return findings.ToArray();
        }
    }
}
