﻿using SciVacancies.Domain.Enums;
using SciVacancies.Domain.Events;
using SciVacancies.ReadModel.Core;

using System;

using MediatR;
using NPoco;
using AutoMapper;

namespace SciVacancies.ReadModel.EventHandlers
{
    public class VacancyApplicationEventsHandler :
        INotificationHandler<VacancyApplicationCreated>,
        INotificationHandler<VacancyApplicationUpdated>,
        INotificationHandler<VacancyApplicationRemoved>,
        INotificationHandler<VacancyApplicationApplied>,
        INotificationHandler<VacancyApplicationCancelled>,
        INotificationHandler<VacancyApplicationWon>,
        INotificationHandler<VacancyApplicationPretended>,
        INotificationHandler<VacancyApplicationLost>
    {
        private readonly IDatabase _db;

        public VacancyApplicationEventsHandler(IDatabase db)
        {
            _db = db;
        }
        public void Handle(VacancyApplicationCreated msg)
        {
            VacancyApplication vacancyApplication = Mapper.Map<VacancyApplication>(msg);

            using (var transaction = _db.GetTransaction())
            {
                _db.Insert(vacancyApplication);
                if(vacancyApplication.attachments!=null)
                {
                    foreach (VacancyApplicationAttachment at in vacancyApplication.attachments)
                    {
                        if (at.guid == Guid.Empty) at.guid = Guid.NewGuid();
                        at.vacancyapplication_guid = vacancyApplication.guid;
                        _db.Insert(at);
                    }
                }

                transaction.Complete();
            }
        }
        public void Handle(VacancyApplicationUpdated msg)
        {
            VacancyApplication vacancyApplication = _db.SingleById<VacancyApplication>(msg.VacancyApplicationGuid);

            VacancyApplication updatedVacancyApplication = Mapper.Map<VacancyApplication>(msg);
            updatedVacancyApplication.creation_date = vacancyApplication.creation_date;

            using (var transaction = _db.GetTransaction())
            {
                _db.Update(updatedVacancyApplication);
                //TODO - без удаления
                _db.Execute(new Sql($"DELETE FROM res_vacancyapplication_attachments WHERE vacancyapplication_guid = @0", msg.VacancyApplicationGuid));
                if(updatedVacancyApplication.attachments!=null)
                {
                    foreach (VacancyApplicationAttachment at in updatedVacancyApplication.attachments)
                    {
                        if (at.guid == Guid.Empty) at.guid = Guid.NewGuid();
                        at.vacancyapplication_guid = vacancyApplication.guid;
                        _db.Insert(at);
                    }
                }

                transaction.Complete();
            }
        }
        public void Handle(VacancyApplicationRemoved msg)
        {
            using (var transaction = _db.GetTransaction())
            {
                _db.Execute(new Sql($"UPDATE res_vacancyapplications SET status = @0, update_date = @1 WHERE guid = @2", VacancyApplicationStatus.Removed, msg.TimeStamp, msg.VacancyApplicationGuid));
                transaction.Complete();
            }
        }
        public void Handle(VacancyApplicationApplied msg)
        {
            using (var transaction = _db.GetTransaction())
            {
                _db.Execute(new Sql($"UPDATE res_vacancyapplications SET apply_date = @0, status = @1, update_date = @2 WHERE guid = @3", msg.TimeStamp, VacancyApplicationStatus.Applied, msg.TimeStamp, msg.VacancyApplicationGuid));
                transaction.Complete();
            }
        }
        public void Handle(VacancyApplicationCancelled msg)
        {
            using (var transaction = _db.GetTransaction())
            {
                _db.Execute(new Sql($"UPDATE res_vacancyapplications SET status = @0, update_date = @1 WHERE guid = @2", VacancyApplicationStatus.Cancelled, msg.TimeStamp, msg.VacancyApplicationGuid));
                transaction.Complete();
            }
        }
        public void Handle(VacancyApplicationWon msg)
        {
            using (var transaction = _db.GetTransaction())
            {
                _db.Execute(new Sql($"UPDATE res_vacancyapplications SET status = @0, update_date = @1 WHERE guid = @2", VacancyApplicationStatus.Won, msg.TimeStamp, msg.VacancyApplicationGuid));
                transaction.Complete();
            }
        }
        public void Handle(VacancyApplicationPretended msg)
        {
            using (var transaction = _db.GetTransaction())
            {
                _db.Execute(new Sql($"UPDATE res_vacancyapplications SET status = @0, update_date = @1 WHERE guid = @2", VacancyApplicationStatus.Pretended, msg.TimeStamp, msg.VacancyApplicationGuid));
                transaction.Complete();
            }
        }
        public void Handle(VacancyApplicationLost msg)
        {
            using (var transaction = _db.GetTransaction())
            {
                _db.Execute(new Sql($"UPDATE res_vacancyapplications SET status = @0, update_date = @1 WHERE guid = @2", VacancyApplicationStatus.Lost, msg.TimeStamp, msg.VacancyApplicationGuid));
                transaction.Complete();
            }
        }
    }
}