using System;

namespace OctopusSearch.Core.Models
{
    public class FoundProjectVariable
    {
        public string Project { get; set; }
        public string VariableSet { get; set; }
        public string[] Environments { get; set; } = new string[0];
        public string VariableName { get; set; }
        public string VariableValue { get; set; }
    }
}
