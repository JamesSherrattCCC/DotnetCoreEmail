using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmailDaemon.Config
{
    abstract class AConfig<T>
    {
        public static T ReadFromJsonFile(string path)
        {
            IConfigurationRoot Configuration;

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(path);

            Configuration = builder.Build();
            return Configuration.Get<T>();
        }
    }
}
