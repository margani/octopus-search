using System;

namespace OctopusSearch.Core.Models
{
    public class FoundVariable
    {
        public string Project { get; set; }
        public string VariableSet { get; set; }
        public string[] Environments { get; set; } = new string[0];
        public string VariableName { get; set; }
        public string VariableValue { get; set; }
        public FoundVariable[] UsedInVariables { get; set; } = new FoundVariable[0];

        public FoundVariable(FoundProjectVariable _)
        {
            Environments = _.Environments;
            Project = _.Project;
            VariableName = _.VariableName;
            VariableSet = _.VariableSet;
            VariableValue = _.VariableValue;
        }

        public void Print(int indent = 0)
        {
            Console.WriteLine($"{"".PadRight(indent * 4, ' ')}Project: {Project}, VariableSet: {VariableSet}, Environments: [{string.Join(",", Environments)}], Variable: {VariableName}, Value: {VariableValue}");

            UsedInVariables.ForEach(_ => _.Print(indent + 1));
        }
    }
}
