using System;

namespace TeslaChargeMate.Config
{
    public class EnvironmentVariableNameAttribute : Attribute
    {
        public EnvironmentVariableNameAttribute(string environmentVariableName)
        {
            EnvironmentVariableName = environmentVariableName;
        }

        public string EnvironmentVariableName { get; }
    }
}