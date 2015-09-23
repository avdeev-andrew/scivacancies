﻿//using SciVacancies.Domain.Enums;

using System;
using System.Linq;
using MediatR;
using NPoco;
using SciVacancies.Domain.Events;
using SciVacancies.ReadModel.Core;
using SciVacancies.SmtpNotifications.SmtpNotificators;

namespace SciVacancies.SmtpNotifications.Handlers
{
    public class VacancyEventsHandler :
        INotificationHandler<VacancyPublished>,
        INotificationHandler<VacancyInCommittee>,
        INotificationHandler<VacancyProlongedInCommittee>,
        INotificationHandler<VacancyInOfferResponseAwaitingFromWinner>,
        INotificationHandler<VacancyOfferAcceptedByWinner>,
        INotificationHandler<VacancyOfferRejectedByWinner>,
        INotificationHandler<VacancyInOfferResponseAwaitingFromPretender>,
        INotificationHandler<VacancyOfferAcceptedByPretender>,
        INotificationHandler<VacancyOfferRejectedByPretender>,
        INotificationHandler<VacancyClosed>,
        INotificationHandler<VacancyCancelled>
    {
        private readonly IDatabase _db;
        private readonly ISmtpNotificatorVacancyService _smtpNotificatorVacancyService;

        public VacancyEventsHandler(IDatabase db, ISmtpNotificatorVacancyService smtpNotificatorVacancyService)
        {
            _db = db;
            _smtpNotificatorVacancyService = smtpNotificatorVacancyService;
        }
        public void Handle(VacancyPublished msg)
        {
            VacancyStatusChangedSmtpNotificationForResearcher(msg.VacancyGuid);
        }
        public void Handle(VacancyInCommittee msg)
        {
            VacancyStatusChangedSmtpNotificationForOrganization(msg.VacancyGuid);
            VacancyStatusChangedSmtpNotificationForResearcher(msg.VacancyGuid);
        }
        public void Handle(VacancyProlongedInCommittee msg)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(msg.VacancyGuid);
            if (vacancy == null) return;
            var researcherGuids =
                _db.Fetch<Guid>(new Sql(
                    $"SELECT va.researcher_guid FROM res_vacancyapplications va WHERE va.vacancy_guid = @0", msg.VacancyGuid));
            if (!researcherGuids.Any()) return;
            var researchers =
                _db.Fetch<Researcher>(new Sql($"SELECT * FROM res_researchers r WHERE r.guid IN (@0)",
                    researcherGuids));
            if (!researchers.Any()) return;

            foreach (var researcher in researchers)
            {
                _smtpNotificatorVacancyService.SendVacancyProlongedForResearcher(vacancy, researcher);
            }
        }
        public void Handle(VacancyInOfferResponseAwaitingFromWinner msg)
        {
            Vacancy vacancy = _db.SingleOrDefaultById<Vacancy>(msg.VacancyGuid);
            Researcher researcher = _db.SingleOrDefaultById<Researcher>(vacancy.winner_researcher_guid);

            _smtpNotificatorVacancyService.SendWinnerSet(researcher, vacancy.winner_vacancyapplication_guid, vacancy.guid);
        }
        public void Handle(VacancyOfferAcceptedByWinner msg)
        {
            VacancyStatusChangedSmtpNotificationForOrganization(msg.VacancyGuid);
        }
        public void Handle(VacancyOfferRejectedByWinner msg)
        {
            VacancyStatusChangedSmtpNotificationForOrganization(msg.VacancyGuid);
        }
        public void Handle(VacancyInOfferResponseAwaitingFromPretender msg)
        {
            Vacancy vacancy = _db.SingleOrDefaultById<Vacancy>(msg.VacancyGuid);
            Researcher researcher = _db.SingleOrDefaultById<Researcher>(vacancy.pretender_researcher_guid);

            _smtpNotificatorVacancyService.SendWinnerSet(researcher, vacancy.pretender_vacancyapplication_guid, vacancy.guid);
        }
        public void Handle(VacancyOfferAcceptedByPretender msg)
        {
            VacancyStatusChangedSmtpNotificationForOrganization(msg.VacancyGuid);
        }
        public void Handle(VacancyOfferRejectedByPretender msg)
        {
            VacancyStatusChangedSmtpNotificationForOrganization(msg.VacancyGuid);
        }
        public void Handle(VacancyClosed msg)
        {
            VacancyStatusChangedSmtpNotificationForResearcher(msg.VacancyGuid);
        }

        public void Handle(VacancyCancelled msg)
        {
            VacancyStatusChangedSmtpNotificationForResearcher(msg.VacancyGuid);
        }


        private void VacancyStatusChangedSmtpNotificationForResearcher(Guid vacancyGuid)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(vacancyGuid);
            if (vacancy == null) return;
            var researcherGuids =
                _db.Fetch<Guid>(new Sql(
                    $"SELECT va.researcher_guid FROM res_vacancyapplications va WHERE va.vacancy_guid = @0", vacancyGuid));
            if (!researcherGuids.Any()) return;
            var researchers =
                _db.Fetch<Researcher>(new Sql($"SELECT * FROM res_researchers r WHERE r.guid IN (@0)",
                    researcherGuids));
            if (!researchers.Any()) return;

            foreach (var researcher in researchers)
                _smtpNotificatorVacancyService.SendVacancyStatusChangedForResearcher(vacancy, researcher);
        }


        private void VacancyStatusChangedSmtpNotificationForOrganization(Guid vacancyGuid)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(vacancyGuid);
            if (vacancy == null) return;
            var organization =
                _db.SingleOrDefaultById<Organization>(vacancy.organization_guid);

            _smtpNotificatorVacancyService.SendVacancyStatusChangedForOrganization(vacancy, organization);
        }
    }
}
