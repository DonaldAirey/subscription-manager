// <copyright file="Startup.cs" company="Gamma Four, Inc.">
//    Copyright © 2018 - Gamma Four, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.SubscriptionManager.WebApplication
{
    using System;
    using System.Collections.Generic;
    using GammaFour.Data;
    using GammaFour.SubscriptionManager.ServerDomain;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Configures the Web Service.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Used to install a database provider based on the configuration file settings.
        /// </summary>
        private Dictionary<DbProvider, Action<IServiceCollection>> dbConfigurations = new Dictionary<DbProvider, Action<IServiceCollection>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            // Initialize the object.
            this.Configuration = configuration;

            // Initialize the vector table for configuring the database provider for the ORM.
            this.dbConfigurations[DbProvider.MySql] = _ => throw new NotImplementedException();
            this.dbConfigurations[DbProvider.PostgreSql] = this.ConfigureNpgsql;
            this.dbConfigurations[DbProvider.SqlServer] = this.ConfigureSqlServer;
        }

        /// <summary>
        /// Gets the publically available configuration for the application.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the service.
        /// </summary>
        /// <param name="applicationBuilder">The application builder.</param>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        public static void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment)
        {
            // This allows us to provide feedback in the development envrionment.
            if (hostingEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
            }
            else
            {
                applicationBuilder.UseHsts();
            }

            // Configure for MVC.
            applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseMvc();
        }

        /// <summary>
        /// Add services to the container.
        /// </summary>
        /// <param name="serviceCollection">The collection of services.</param>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            // This configures the database service according to the settings in the configuraiton file.
            this.dbConfigurations[this.Configuration.GetValue<DbProvider>("dbProvider")](serviceCollection);

            // Add the dependencies.
            serviceCollection.AddSingleton<Domain>();

            // Add the controllers.
            serviceCollection.AddMvc().AddControllersAsServices();

            // Set the compatibility.
            serviceCollection.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// Configure Microsoft SQL Server to provide the persistence.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        private void ConfigureSqlServer(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<DomainContext>(
                options => options.UseSqlServer(this.Configuration.GetConnectionString("SqlServerConnection"), b => b.MigrationsAssembly("GammaFour.SubscriptionManager.WebApplication")),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient);
        }

        /// <summary>
        /// Configure Amazon PostreSQL to provide the persistence.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        private void ConfigureNpgsql(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<DomainContext>(
                options => options.UseNpgsql(this.Configuration.GetConnectionString("PostgreSqlConnection"), b => b.MigrationsAssembly("GammaFour.SubscriptionManager.WebApplication")),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient);
        }
    }
}