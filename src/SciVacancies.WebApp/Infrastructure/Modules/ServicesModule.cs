﻿using Autofac;

using SciVacancies.ReadModel;

namespace SciVacancies.WebApp.Infrastructure
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ElasticService>().As<IElasticService>().SingleInstance();
        }
    }
}