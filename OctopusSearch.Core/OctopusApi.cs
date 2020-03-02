using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;
using OctopusSearch.Core.Models;

namespace OctopusSearch.Core
{
    public class OctopusApi
    {
        private readonly OctopusRestClient _octopusClient;

        private const string OctopusActionTargetRolesKey = "Octopus.Action.TargetRoles";

        public OctopusApi(string server, string apiKey)
        {
            _octopusClient = new OctopusRestClient(server, apiKey);
        }

        public async Task<FoundProjectVariable[]> SearchVariablesFor(string search, bool doSearchInVariableSetsToo = true)
        {
            var findings = new List<FoundProjectVariable>();

            Console.WriteLine("Getting all library variable sets");
            var libraryVariableSets = await _octopusClient.GetResources<LibraryVariableSetResource>("LibraryVariableSets/all");

            //Console.WriteLine("Getting all variable sets");
            //var variableSets = await _octopusClient.GetResources<VariableSetResource>("Variables/all");

            Console.WriteLine("Getting all projects");
            var projects = await _octopusClient.GetResources<ProjectResource>("Projects/all");

            Console.Write("Searching project variables...");
            foreach (var project in projects)
            {
                var projectVariableSet = await _octopusClient.GetResource<VariableSetResource>($"Variables/{project.VariableSetId}");
                var variables = projectVariableSet.Variables;
                if (doSearchInVariableSetsToo)
                {
                    var vs = project
                        .IncludedLibraryVariableSetIds
                        .SelectMany(_ =>
                        {
                            var libraryVariableSetId = libraryVariableSets.Single(__ => __.Id == _).VariableSetId;
                            var variableSet = _octopusClient.GetResource<VariableSetResource>($"Variables/{libraryVariableSetId}").Result;
                            return variableSet.Variables;
                        });

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
            foreach (var libraryVariableSet in libraryVariableSets)
            {
                var variableSet = await _octopusClient.GetResource<VariableSetResource>($"Variables/{libraryVariableSet.VariableSetId}");
                var variables = variableSet.Variables;
                findings.AddRange(
                    from variable in variables
                    where variable.Name?.Contains(search) == true || variable.Value?.Contains(search) == true
                    select new FoundProjectVariable { VariableSet = libraryVariableSet.Name, VariableName = variable.Name, VariableValue = variable.Value });
            }

            return findings.ToArray();
        }

        public async Task<FoundProjectByRole[]> GetProjectsByRole(params string[] roles)
        {
            Console.WriteLine("Getting projects...");
            var projects = await _octopusClient.GetResources<ProjectResource>("Projects/all");

            var findings = new List<FoundProjectByRole>();
            for (var projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                var project = projects[projectIndex];
                if (project.IsDisabled)
                    continue;

                Console.Write($"\rSearching projects {projectIndex + 1} of {projects.Count}...");
                var deploymentProcess = await _octopusClient.GetResource<DeploymentProcessResource>($"DeploymentProcesses/{project.DeploymentProcessId}");
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

        public async Task<FoundProjectByVariableSetName[]> GetProjectsUsingAVariableSet(string variableSetId)
        {
            Console.WriteLine("Getting projects...");
            var projects = await _octopusClient.GetResources<ProjectResource>("Projects/all");

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

        public async Task<FoundDeploymentTargetsWhichVariableSetIsUsed[]> GetDeploymentTargetsWhichThisVariableSetIsUsed(string variableSetId, string environmentId, Func<MachineResource, bool> fExcludes = null)
        {
            Console.WriteLine("Getting environments...");
            var environment = await _octopusClient.GetResource<EnvironmentResource>($"Environments/{environmentId}");

            Console.WriteLine("Getting machines...");
            var machines = (await _octopusClient.GetResources<MachineResource>($"Environments/{environmentId}/machines"))
                .Where(o => fExcludes?.Invoke(o) == false)
                .ToArray();

            Console.WriteLine("Getting projects...");
            var projects = await _octopusClient.GetResources<ProjectResource>("Projects/all");

            var findings = new List<FoundDeploymentTargetsWhichVariableSetIsUsed>();
            for (var projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                var project = projects[projectIndex];
                if (project.IsDisabled)
                    continue;

                Console.Write($"\rSearching projects {projectIndex + 1} of {projects.Count}...");

                if (project.IncludedLibraryVariableSetIds.Contains(variableSetId))
                {
                    var deploymentProcess = await _octopusClient.GetResource<DeploymentProcessResource>($"DeploymentProcesses/{project.DeploymentProcessId}");
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
