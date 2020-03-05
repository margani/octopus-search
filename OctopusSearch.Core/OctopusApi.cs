using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Octopus.Client.Model;
using OctopusSearch.Core.Models;

namespace OctopusSearch.Core
{
    public class OctopusApi
    {
        private readonly OctopusRestClient _octopusClient;

        private readonly List<EnvironmentResource> _environments;
        private readonly List<LibraryVariableSetResource> _libraryVariableSets;
        private readonly List<ProjectResource> _projects;
        private readonly List<VariableSetResource> _variableSets;

        private VariableSetResource GetVariableSet(string variableSetId) =>
            _variableSets?.SingleOrDefault(_ => _.Id == variableSetId) ??
             _octopusClient.GetResource<VariableSetResource>($"Variables/{variableSetId}").Result;

        private const string OctopusActionTargetRolesKey = "Octopus.Action.TargetRoles";

        public OctopusApi(string server, string apiKey, bool preFetchOctopusVariableSets = false)
        {
            _octopusClient = new OctopusRestClient(server, apiKey);

            Console.WriteLine("Getting all library variable sets");
            _libraryVariableSets = _octopusClient.GetResources<LibraryVariableSetResource>("LibraryVariableSets/all").Result;

            Console.WriteLine("Getting all projects");
            _projects = _octopusClient.GetResources<ProjectResource>("Projects/all").Result;

            Console.WriteLine("Getting environments...");
            _environments = _octopusClient.GetResources<EnvironmentResource>("Environments/all").Result;

            if (preFetchOctopusVariableSets)
            {
                Console.WriteLine("Getting variables...");
                _variableSets = _octopusClient.GetResources<VariableSetResource>("Variables/all").Result;
            }
        }

        public IEnumerable<FoundProjectVariable> SearchVariablesFor(string search, bool doSearchInVariableSetsToo = true) =>
            SearchVariablesFor(
                    _ => _.Name?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || _.Value?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0,
                    doSearchInVariableSetsToo);

        public IEnumerable<FoundVariable> DeepSearchVariablesFor(string search, bool doSearchInVariableSetsToo = true)
        {
            var foundVariables =
                 SearchVariablesFor(
                    _ =>
                        _.Name?.Equals(search, StringComparison.OrdinalIgnoreCase) == true ||
                        _.Value?.Equals(search, StringComparison.OrdinalIgnoreCase) == true,
                    doSearchInVariableSetsToo)
                .Select(_ => new FoundVariable(_));

            foreach (var foundVariable in foundVariables)
            {
                var foundVariablesDeep =
                     SearchVariablesFor(
                        _ =>
                            _.Value?.Equals($"#{{{foundVariable}}}", StringComparison.OrdinalIgnoreCase) == true ||
                            _.Value?.Equals(search, StringComparison.OrdinalIgnoreCase) == true,
                        doSearchInVariableSetsToo)
                    .Select(_ => new FoundVariable(_));

                foundVariable.UsedInVariables = foundVariablesDeep.ToArray();
            }

            return foundVariables;
        }

        private IEnumerable<FoundProjectVariable> SearchVariablesFor(Func<VariableResource, bool> whereVariable, bool doSearchInVariableSetsToo = true)
        {
            var findings = new List<FoundProjectVariable>();

            Console.WriteLine("Searching project variables...");
            foreach (var project in _projects)
            {
                var projectVariableSet = GetVariableSet(project.VariableSetId);
                findings.AddRange(projectVariableSet
                        .Variables
                        .Where(whereVariable)
                        .Select(_ => new FoundProjectVariable { Project = project.Name, VariableName = _.Name, VariableValue = _.Value })
                    );

                if (doSearchInVariableSetsToo)
                {
                    foreach (var includedLibraryVariableSetId in project.IncludedLibraryVariableSetIds)
                    {
                        var libraryVariableSet = _libraryVariableSets.Single(_ => _.Id == includedLibraryVariableSetId);

                        findings.AddRange(SearchVariablesFor(whereVariable, libraryVariableSet, project));
                    }
                }
            }

            Console.WriteLine("Searching variable sets...");
            foreach (var libraryVariableSet in _libraryVariableSets)
            {
                findings.AddRange(SearchVariablesFor(whereVariable, libraryVariableSet));
            }

            return findings.ToArray();
        }

        private IEnumerable<FoundProjectVariable> SearchVariablesFor(Func<VariableResource, bool> whereVariable, LibraryVariableSetResource libraryVariableSet, ProjectResource project = null)
        {
            var variableSet = GetVariableSet(libraryVariableSet.VariableSetId);
            var variables = variableSet.Variables.Where(whereVariable);

            var findings = new List<FoundProjectVariable>();
            foreach (var variable in variables)
            {
                var environmentIds = variable.Scope.SelectMany(_ => _.Value).ToList();
                var variableEnvironments = _environments.Where(_ => environmentIds.Contains(_.Id)).Select(_ => _.Name).ToArray();
                findings.Add(new FoundProjectVariable
                {
                    VariableSet = libraryVariableSet.Name,
                    VariableName = variable.Name,
                    VariableValue = variable.Value,
                    Project = project?.Name,
                    Environments = variableEnvironments
                });
            }

            return findings;
        }

        public async Task<IEnumerable<FoundProjectByRole>> GetProjectsByRole(params string[] roles)
        {
            Console.WriteLine("Getting projects...");
            var projects = await _octopusClient.GetResources<ProjectResource>("Projects/all").ConfigureAwait(false);

            var findings = new List<FoundProjectByRole>();
            for (var projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                var project = projects[projectIndex];
                if (project.IsDisabled)
                    continue;

                Console.WriteLine($"\rSearching projects {projectIndex + 1} of {projects.Count}...");
                var deploymentProcess = await _octopusClient.GetResource<DeploymentProcessResource>($"DeploymentProcesses/{project.DeploymentProcessId}").ConfigureAwait(false);
                var allRolesUsed = deploymentProcess.Steps
                    .Where(o => o.Properties.ContainsKey(OctopusActionTargetRolesKey))
                    .SelectMany(o => o.Properties[OctopusActionTargetRolesKey]?.Value.Split(','))
                    .Distinct()
                    .ToArray();

                var foundRoles = roles.Where(o => allRolesUsed.Contains(o)).ToArray();
                if (foundRoles.Length > 0)
                    findings.Add(new FoundProjectByRole { Project = project.Name, FoundRoles = foundRoles, AllRoles = allRolesUsed });
            }

            Console.WriteLine(", Done!");

            return findings.ToArray();
        }

        public async Task<IEnumerable<FoundProjectByVariableSetName>> GetProjectsUsingAVariableSet(string variableSetId)
        {
            Console.WriteLine("Getting projects...");
            var projects = await _octopusClient.GetResources<ProjectResource>("Projects/all").ConfigureAwait(false);

            var findings = new List<FoundProjectByVariableSetName>();
            for (var projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                var project = projects[projectIndex];
                if (project.IsDisabled)
                    continue;

                Console.WriteLine($"\rSearching projects {projectIndex + 1} of {projects.Count}...");

                if (project.IncludedLibraryVariableSetIds.Contains(variableSetId))
                    findings.Add(new FoundProjectByVariableSetName { Project = project.Name });
            }

            Console.WriteLine(", Done!");

            return findings.ToArray();
        }

        public async Task<IEnumerable<FoundDeploymentTargetsWhichVariableSetIsUsed>> GetDeploymentTargetsWhichThisVariableSetIsUsed(string variableSetId, string environmentId, Func<MachineResource, bool> fExcludes = null)
        {
            Console.WriteLine("Getting environments...");
            var environment = await _octopusClient.GetResource<EnvironmentResource>($"Environments/{environmentId}").ConfigureAwait(false);

            Console.WriteLine("Getting machines...");
            var machines = (await _octopusClient.GetResources<MachineResource>($"Environments/{environmentId}/machines").ConfigureAwait(false))
                .Where(o => fExcludes?.Invoke(o) == false)
                .ToArray();

            Console.WriteLine("Getting projects...");
            var projects = await _octopusClient.GetResources<ProjectResource>("Projects/all").ConfigureAwait(false);

            var findings = new List<FoundDeploymentTargetsWhichVariableSetIsUsed>();
            for (var projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                var project = projects[projectIndex];
                if (project.IsDisabled)
                    continue;

                Console.WriteLine($"\rSearching projects {projectIndex + 1} of {projects.Count}...");

                if (project.IncludedLibraryVariableSetIds.Contains(variableSetId))
                {
                    var deploymentProcess = await _octopusClient.GetResource<DeploymentProcessResource>($"DeploymentProcesses/{project.DeploymentProcessId}").ConfigureAwait(false);
                    var allRolesUsed = deploymentProcess.Steps
                        .Where(o => o.Properties.ContainsKey(OctopusActionTargetRolesKey))
                        .SelectMany(o => o.Properties[OctopusActionTargetRolesKey]?.Value.Split(','))
                        .Distinct()
                        .ToArray();

                    var targetMachines = machines.Where(o => allRolesUsed.Any(oo => o.Roles.Any(ooo => ooo == oo))).Distinct().ToArray();
                    if (targetMachines.Length > 0)
                        findings.Add(new FoundDeploymentTargetsWhichVariableSetIsUsed { Project = project.Name, Machines = targetMachines.Select(o => o.Name.ToLower()).OrderBy(o => o).ToArray() });
                }
            }

            Console.WriteLine(", Done!");

            return findings.ToArray();
        }
    }
}
