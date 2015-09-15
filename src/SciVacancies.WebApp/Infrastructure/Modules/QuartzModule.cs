﻿using Autofac;
using Quartz;
using Quartz.Spi;
using SciVacancies.Services.Quartz;
using SciVacancies.WebApp.Infrastructure;

namespace SciVacancies.Services
{
    public class QuartzModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<QuartzService>().AsImplementedInterfaces();
            builder.RegisterType<AutofacJobFactory>().As<IJobFactory>().InstancePerLifetimeScope();
            builder.RegisterTypes(System.Reflection.Assembly.GetAssembly(typeof(QuartzModule)).GetTypes())
               .Where(t => t != typeof(IJob) && typeof(IJob).IsAssignableFrom(t))
               .AsSelf()
               .InstancePerLifetimeScope();
        }
    }
}
