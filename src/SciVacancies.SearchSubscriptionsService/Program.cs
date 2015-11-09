﻿using System;
using System.Linq;
using System.Collections;
using System.ServiceProcess;
using System.Threading;
using Autofac;
using Autofac.Framework.DependencyInjection;
using Nest;
using Npgsql;
using NPoco;
using Quartz;
using Quartz.Spi;
using SciVacancies.SearchSubscriptionsService.Jobs;
using SciVacancies.Services.Email;
using SciVacancies.Services.Quartz;
using SciVacancies.Services.Elastic;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Logging;
using Microsoft.Framework.DependencyInjection;
//using Microsoft.rd
using SciVacancies.SearchSubscriptionsService.Modules;
using Microsoft.Dnx.Runtime;

using Serilog;

namespace SciVacancies.SearchSubscriptionsService
{
    public class Program
    {
        public IConfiguration Configuration { get; set; }

        public IContainer Container { get; set; }

        private string devEnv;

        public Program(IApplicationEnvironment appEnv)
        {
            var vars = Environment.GetEnvironmentVariables();
            devEnv = (string)vars.Cast<DictionaryEntry>().FirstOrDefault(e => e.Key.Equals("dev_env")).Value;

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{devEnv}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = configurationBuilder.Build();

            var builder = new ContainerBuilder();



            ConfigureContainer(builder);

            Container = builder.Build();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(c => Configuration).As<IConfiguration>().SingleInstance();

            builder.RegisterModule(new ReadModelModule(Configuration));
            builder.RegisterModule(new ServicesModule(Configuration));
            builder.RegisterModule(new QuartzModule());
            builder.RegisterModule(new SmtpNotificationModule());
            builder.RegisterModule(new LoggingModule(Configuration));
        }

        public void Main(string[] args)
        {
            Console.WriteLine("Started");

            SearchSubscriptionService searchSubscriptionService;
            //service initializing
            try
            {
                searchSubscriptionService = Container.Resolve<SearchSubscriptionService>();
                Console.WriteLine("Program: SearchSubscriptionService Resolved");
                searchSubscriptionService.OnStart();
                Console.WriteLine("Program: SearchSubscriptionService Started");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Program:" + exception.Message);
                Console.ReadLine();
                return;
            }
            //searchSubscriptionService.ServiceName = "SearchSubscriptionService";
            //ServiceBase.Run(searchSubscriptionService);

            var wroteCommand = Console.ReadLine();
            while (wroteCommand == null || !wroteCommand.Equals("stop"))
            {
                Thread.Sleep(2000);
                wroteCommand = Console.ReadLine();
            }

            while (!searchSubscriptionService.CanStop)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine("Program: Stopping");
            searchSubscriptionService.Stop();
            Console.WriteLine("Program: SearchSubscriptionService Stopped");
            Console.ReadLine();
        }
    }
}
