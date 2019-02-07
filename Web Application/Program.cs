// <copyright file="Program.cs" company="Gamma Four, Inc.">
//    Copyright © 2018 - Gamma Four, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.SubscriptionManager.WebApplication
{
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The main program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point for the program.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .Run();
        }

        /// <summary>
        /// This method is called by external tools such as Entity Framework.
        /// </summary>
        /// <param name="args">The application startup arguments.</param>
        /// <returns>A web host.</returns>
        public static IWebHost BuildWebHost(string[] args) => WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();

        /// <summary>
        /// Configure the configuration settings.
        /// </summary>
        /// <param name="webHostBuilderContext">The context for building the host.</param>
        /// <param name="configurationBuilder">The Fluent builder for configurations.</param>
        private static void ConfigConfiguration(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{webHostBuilderContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        }
    }
}