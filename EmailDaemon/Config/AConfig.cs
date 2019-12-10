using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmailDaemon.Config
{
    /// <summary>
    /// Class containing a method for initialising a config class from a json file.
    /// </summary>
    /// <typeparam name="T">The class which is initialised from the config.</typeparam>
    public abstract class AConfig<T>
    {
        /// <summary>
        /// Reads a json file into class T when given a path.
        /// </summary>
        /// <param name="path">Path to the json file.</param>
        /// <returns></returns>
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
