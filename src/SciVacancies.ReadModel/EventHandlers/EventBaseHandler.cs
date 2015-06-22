﻿using SciVacancies.Domain.Events;

using MediatR;
using NPoco;

namespace SciVacancies.ReadModel.EventHandlers
{
    public abstract class EventBaseHandler<T> : INotificationHandler<T> where T : EventBase
    {
        protected readonly IDatabase _db;
        protected EventBaseHandler(IDatabase db)
        {
            _db = db;
        }
        public abstract void Handle(T msg);
    }
}