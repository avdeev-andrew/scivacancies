﻿using System;
using System.Collections;
using System.Linq;
using Autofac;
using Autofac.Dnx;
using Insight.Database.Providers.PostgreSQL;
using Microsoft.Framework.OptionsModel;
using Npgsql;
//using SciVacancies.ApplicationInfrastructure;
using Xunit;
using Insight.Database;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using SciVacancies.ReadModel;
using SciVacancies.ReadModel.Core;
using SciVacancies.WebApp;

namespace SciVacancies.SandBox
{
    public class InsightDatabaseTests
    {
        public IConfiguration Configuration { get; set; }
        private IServiceCollection _services;
        private IServiceProvider _serviceProvider;

        public InsightDatabaseTests()
        {
            var vars = Environment.GetEnvironmentVariables();
            var devEnv = vars.Cast<DictionaryEntry>().FirstOrDefault(e => e.Key.Equals("dev_env")).Value;
            // Setup configuration sources.
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{devEnv}.json", optional: true)
                .AddEnvironmentVariables();
            _services = new ServiceCollection().AddOptions();
            _services.Configure<AppSettings>(Configuration.GetSubKey("AppSettings"));
            _services.Configure<DbSettings>(Configuration.GetSubKey("Data"));

            var builder = new ContainerBuilder();

            builder.Populate(_services);
            var container = builder.Build();
            _serviceProvider = container.Resolve<IServiceProvider>();
            PostgreSQLInsightDbProvider.RegisterProvider();
        }

        [Fact]
        public void SomeTest1()
        {
            var _options = _serviceProvider.GetRequiredService<IOptions<DbSettings>>();
            Organization org1;
            var connection = new NpgsqlConnection(_options.Options.ReadModelDb);
            using (var repo = connection.OpenWithTransactionAs<IOrganizationRepository>())
            {
                org1 = repo.GetOrganizationById(new Guid("13e32fc3-b4d4-47a7-9f76-984cfa05debd"));
                repo.Commit();
            }

            Assert.Equal(org1.name, "Корпорация Umbrella");
        }
    }
}
