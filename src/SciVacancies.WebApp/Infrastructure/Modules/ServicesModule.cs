﻿using SciVacancies.Services.Elastic;

using System;

using Nest;
using Autofac;
using Microsoft.Framework.Configuration;
using EventSourceProxy;

namespace SciVacancies.WebApp.Infrastructure
{
    public class ServicesModule : Module
    {
        private readonly IConfiguration _configuration;

        public ServicesModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            ConnectionSettings elasticConnectionSettings = new ConnectionSettings(new Uri(_configuration["ElasticSettings:ConnectionUrl"]), defaultIndex: _configuration["ElasticSettings:DefaultIndex"]);

            builder.Register(c => new ElasticClient(elasticConnectionSettings))
                .As<IElasticClient>()
                .SingleInstance()
                //.OnActivating(x=>x
                //    .ReplaceInstance(TracingProxy.CreateWithActivityScope<IElasticClient>(x.Instance))
                //)
                ;
            //TODO single instanse or not?
            builder.Register(c => new SearchService(_configuration, c.Resolve<IElasticClient>())).As<IElasticService>();
        }
    }
}