﻿using Autofac;
using MediatR;
using SciVacancies.Domain.Events;
using SciVacancies.ReadModel.EventHandlers;
using SciVacancies.ReadModel.ElasticSearchModel.EventHandlers;
using SciVacancies.WebApp.Commands;

namespace SciVacancies.WebApp.Infrastructure
{
    public class EventHandlersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(SciVacancies.ReadModel.EventHandlers.VacancyApplicationEventsHandler).Assembly).AsClosedTypesOf(typeof(INotificationHandler<>)).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(SciVacancies.ReadModel.ElasticSearchModel.EventHandlers.OrganizationEventsHandler).Assembly).AsClosedTypesOf(typeof(INotificationHandler<>)).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(EventBase).Assembly).SingleInstance();

            builder.RegisterAssemblyTypes(typeof(CommandBase).Assembly).AsClosedTypesOf(typeof(IRequest<>)).SingleInstance();
            builder.RegisterAssemblyTypes(typeof(CreateOrganizationCommandHandler).Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>)).SingleInstance();
        }
    }
}
