using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Linq;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Config
{
    public class ConfigProvider : IConfigProvider
    {
        private IConfiguration _configuration;

        public ConfigProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Get<T>() where T : IConfigSection
        {
            var instance = Activator.CreateInstance<T>();
            var props = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(EnvironmentVariableNameAttribute), false).Any());

            foreach (var prop in props)
            {
                var environmentVariableName = ((EnvironmentVariableNameAttribute[])prop.GetCustomAttributes(typeof(EnvironmentVariableNameAttribute), false)).First().EnvironmentVariableName;
                var environmentVariableValue = _configuration.GetSection(environmentVariableName).Value;
                if (environmentVariableValue != null)
                {
                    var configValue = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(environmentVariableValue);
                    prop.SetValue(instance, configValue);
                }                
            }

            return instance;
        }
    }
}