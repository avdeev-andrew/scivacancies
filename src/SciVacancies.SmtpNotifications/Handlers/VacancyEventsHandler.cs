﻿//using SciVacancies.Domain.Enums;

using System;
using System.Collections.Generic;
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
            //VacancyStatusChangedSmtpNotificationForOrganization(msg.VacancyGuid);
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
            VacancyStatusChangedSmtpNotificationForResearcher(msg.VacancyGuid);

            var vacancy = _db.SingleOrDefaultById<Vacancy>(msg.VacancyGuid);
            var researcher = _db.SingleOrDefaultById<Researcher>(vacancy.winner_researcher_guid);
            _smtpNotificatorVacancyService.SendWinnerSet(researcher, vacancy.winner_vacancyapplication_guid, vacancy.guid);
        }
        public void Handle(VacancyOfferAcceptedByWinner msg)
        {
            OfferAcceptedByWinner(msg.VacancyGuid);
        }
        public void Handle(VacancyOfferRejectedByWinner msg)
        {
            OfferRejectedByWinner(msg.VacancyGuid);
        }
        public void Handle(VacancyInOfferResponseAwaitingFromPretender msg)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(msg.VacancyGuid);
            var researcher = _db.SingleOrDefaultById<Researcher>(vacancy.pretender_researcher_guid);

            _smtpNotificatorVacancyService.SendWinnerSet(researcher, vacancy.pretender_vacancyapplication_guid, vacancy.guid);
        }
        public void Handle(VacancyOfferAcceptedByPretender msg)
        {
            OfferAcceptedByPretender(msg.VacancyGuid);
        }
        public void Handle(VacancyOfferRejectedByPretender msg)
        {
            OfferRejectedByPretender(msg.VacancyGuid);
        }
        public void Handle(VacancyClosed msg)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(msg.VacancyGuid);
            if (vacancy == null) return;
            var researcherGuids =
                new List<Guid>
                {
                    _db.First<Guid>(new Sql("SELECT a.winner_researcher_guid FROM org_vacancies v WHERE v.guid = @0", msg.VacancyGuid)),
                    _db.First<Guid>(new Sql("SELECT a.pretender_researcher_guid FROM org_vacancies v WHERE v.guid = @0", msg.VacancyGuid))
                };
            if (!researcherGuids.Any()) return;

            VacancyStatusChangedSmtpNotificationForResearcher(researcherGuids.Where(guid => guid!= Guid.Empty).ToList(), vacancy);
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
                _db.Fetch<Guid>(new Sql("SELECT va.researcher_guid FROM res_vacancyapplications va WHERE va.vacancy_guid = @0", vacancyGuid));
            if (!researcherGuids.Any()) return;

            VacancyStatusChangedSmtpNotificationForResearcher(researcherGuids, vacancy);
        }

        private void VacancyStatusChangedSmtpNotificationForResearcher(IList<Guid> researcherGuids, Vacancy vacancy)
        {
            if (researcherGuids == null)
                return;
            if (vacancy == null)
                return;

            var researchers = _db.Fetch<Researcher>(new Sql($"SELECT * FROM res_researchers r WHERE r.guid IN (@0)", researcherGuids));
            if (!researchers.Any())
                return;

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

        private void OfferAcceptedByWinner(Guid vacancyGuid)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(vacancyGuid);
            if (vacancy == null) return;
            var organization =
                _db.SingleOrDefaultById<Organization>(vacancy.organization_guid);
            _smtpNotificatorVacancyService.SendOfferAcceptedByWinner(vacancy, organization);
        }

        private void OfferAcceptedByPretender(Guid vacancyGuid)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(vacancyGuid);
            if (vacancy == null) return;
            var organization =
                _db.SingleOrDefaultById<Organization>(vacancy.organization_guid);
            _smtpNotificatorVacancyService.SendOfferAcceptedByPretender(vacancy, organization);
        }

        private void OfferRejectedByWinner(Guid vacancyGuid)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(vacancyGuid);
            if (vacancy == null) return;
            var organization =
                _db.SingleOrDefaultById<Organization>(vacancy.organization_guid);
            _smtpNotificatorVacancyService.SendOfferRejectedByWinner(vacancy, organization);
        }

        private void OfferRejectedByPretender(Guid vacancyGuid)
        {
            var vacancy = _db.SingleOrDefaultById<Vacancy>(vacancyGuid);
            if (vacancy == null) return;
            var organization =
                _db.SingleOrDefaultById<Organization>(vacancy.organization_guid);
            _smtpNotificatorVacancyService.SendOfferRejectedByPretender(vacancy, organization);
        }

    }
}
