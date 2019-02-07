// <copyright file="Program.cs" company="Gamma Four">
//    Copyright © 2018 - Gamma Four.  All rights reserved.
// </copyright>
// <author>Donald Airey</author>
namespace GammaFour.UnderWriter.Loader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    /// <summary>
    /// This loads a set of JSON files into a REST API to seed a domain.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point of the program.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>A task used for waiting.</returns>
        public static async Task Main(string[] args)
        {
            // This gives us access to the configuration settings.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            string host = configuration["host"];

            // Usage message
            if (args.Length < 1)
            {
                Console.WriteLine("usage: loader <input file path>");
                return;
            }

            // The index contains the instructions for loading the rest of the data into the domain.
            List<Index> indexes = new List<Index>();
            using (StreamReader streamReader = new StreamReader(args[0]))
            {
                indexes = JsonConvert.DeserializeObject<List<Index>>(streamReader.ReadToEnd());
            }

            // Run through each table and load it into the REST API at the given URL.
            foreach (Index index in indexes)
            {
                // This allows entries to be disabled without deleting them.  Very handy for incremental loading.
                if (!index.IsDisabled)
                {
                    DateTime startTime = DateTime.Now;
                    await EntityLoader.LoadAsync(new Uri(host), index.Api, index.Path, index.Key, index.Verb).ConfigureAwait(false);
                    double loadTime = DateTime.Now.Subtract(startTime).TotalMilliseconds;
                    Console.WriteLine($"{index.Message}: {loadTime / 1000.0: 0.000} seconds.");
                }
            }
        }
    }
}