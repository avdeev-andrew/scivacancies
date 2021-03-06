﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Logging;

namespace SciVacancies.WebApp.Infrastructure
{
    public class CompositionRoot
    {
        public ContainerBuilder Builder { get; set; } = new ContainerBuilder();
        public Lazy<IContainer> Container => new Lazy<IContainer>(() => Builder.Build());

        public CompositionRoot(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Builder.RegisterModule(new EventStoreModule(configuration, loggerFactory));
            Builder.RegisterModule(new EventBusModule());
            Builder.RegisterModule(new EventHandlersModule());
            Builder.RegisterModule(new ReadModelModule(configuration));
            Builder.RegisterModule(new ServicesModule(configuration));
            Builder.RegisterModule(new IdentityModule());
        }
    }
}
