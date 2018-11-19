using System;
using System.Collections.Generic;
using System.Configuration;
using EasyConsole;
using NDesk.Options;

namespace OctopusSearch.ConsoleApp
{
    class Program
    {
        private static OctopusApi _octopusApi;
        private static bool Exiting = false;

        private static void Main(string[] args)
        {
            var apiUrl = ConfigurationManager.AppSettings["OctopusAPIUrl"];
            var apiKey = ConfigurationManager.AppSettings["OctopusAPIKey"];
            var help = false;

            var p = new OptionSet {
                { "api-url",      v => apiUrl = v },
                { "api-key",      v => apiKey = v },
                { "h|?|help",   v => help = v != null },
            };

            var extra = p.Parse(args);

            if (!apiUrl.IsEmpty() &&
                !apiKey.IsEmpty())
            {
                _octopusApi = new OctopusApi(apiUrl, apiKey);
            }
            else
            {
                Console.WriteLine("Please specify Octopus API url and key in eaither app.config or as a command line argument.");
                return;
            }

            if (help)
            {
                Console.WriteLine("Octopus Search" +
                                  "example: octopus-search -api-url 'http://myoctopuswebsite.com' -api-key 'API-XXX'");
            }

            var menu = new EasyConsole.Menu()
                .Add("Search variables (by name and value)", () =>
                    {
                        var searchFor = Input.ReadString("Search for: ");

                        _octopusApi.SearchVariablesFor(searchFor)
                            .ForEach(o => Console.WriteLine($"Project: {o.Project}, Variable: {o.VariableName}, Value: {o.VariableValue}"));
                    })
                .Add("Get projects by role(s) (which are specified in the project's process)", () =>
                    {
                        var roles = new List<string>();
                        string enteredRole;
                        Console.WriteLine("Enter role name(s), you can use wildcards:");
                        do
                        {
                            enteredRole = Input.ReadString("");
                            if (!enteredRole.IsEmpty())
                                roles.Add(enteredRole);
                        } while (enteredRole.IsEmpty());

                        _octopusApi.GetProjectsByRole(roles.ToArray())
                            .ForEach(o => Console.WriteLine($"Project: {o.Project}, Found Roles: {string.Join(",", o.FoundRoles)}, All Roles: {string.Join(",", o.AllRoles)}"));
                    })
                .Add("Get projects which are using a variable set", () =>
                    {
                        var variableSetId = Input.ReadString("Enter the variable set id: ");

                        _octopusApi.GetProjectsUsingAVariableSet(variableSetId)
                            .ForEach(o => Console.WriteLine($"Project: {o.Project}"));
                    })
                .Add("Get deployment targets which are using a variable set in an enviroment (indirectly via a project)", () =>
                    {
                        var variableSetId = Input.ReadString("Enter the variable set id: ");
                        var environmentId = Input.ReadString("Enter the environment id: ");

                        _octopusApi.GetDeploymentTargetsWhichThisVariableSetIsUsed(variableSetId, environmentId)
                            .ForEach(o => Console.WriteLine($"Project: {o.Project}, Machines: {string.Join(", ", o.Machines)}"));
                    })
                .Add("Exit", () =>
                    {
                        Exiting = true;
                        Environment.Exit(0);
                    })
                ;

            while (!Exiting)
            {
                menu.Display();

                PressAnyKey();
            }
        }

        private static void PressAnyKey()
        {
            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
