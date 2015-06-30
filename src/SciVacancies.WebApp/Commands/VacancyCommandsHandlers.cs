﻿using SciVacancies.Domain.Aggregates;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using CommonDomain.Persistence;
using MediatR;

namespace SciVacancies.WebApp.Commands
{
    public class PublishVacancyCommandHandler : IRequestHandler<PublishVacancyCommand, Guid>
    {
        private readonly IRepository _repository;

        public PublishVacancyCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Guid Handle(PublishVacancyCommand message)
        {
            if (message.OrganizationGuid == Guid.Empty) throw new ArgumentNullException($"OrganizationGuid is empty: {message.OrganizationGuid}");
            if (message.PositionGuid == Guid.Empty) throw new ArgumentNullException($"PositionGuid is empty: {message.PositionGuid}");

            var rdm = message.Data;

            Organization organization = _repository.GetById<Organization>(message.OrganizationGuid);
            Guid vacancyGuid = organization.PublishVacancy(message.PositionGuid, rdm);
            _repository.Save(organization, Guid.NewGuid(), null);

            return vacancyGuid;
        }
    }
    public class SwitchVacancyToAcceptApplicationsCommandHandler : RequestHandler<SwitchVacancyToAcceptApplicationsCommand>
    {
        private readonly IRepository _repository;

        public SwitchVacancyToAcceptApplicationsCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected override void HandleCore(SwitchVacancyToAcceptApplicationsCommand message)
        {
            if (message.OrganizationGuid == Guid.Empty) throw new ArgumentNullException($"OrganizationGuid is empty: {message.OrganizationGuid}");
            if (message.VacancyGuid == Guid.Empty) throw new ArgumentNullException($"VacancyGuid is empty: {message.VacancyGuid}");

            Organization organization = _repository.GetById<Organization>(message.OrganizationGuid);
            organization.SwitchVacancyToAcceptApplications(message.VacancyGuid);
            _repository.Save(organization, Guid.NewGuid(), null);
        }
    }
    public class SwitchVacancyInCommitteeCommandHandler : RequestHandler<SwitchVacancyInCommitteeCommand>
    {
        private readonly IRepository _repository;

        public SwitchVacancyInCommitteeCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected override void HandleCore(SwitchVacancyInCommitteeCommand message)
        {
            if (message.OrganizationGuid == Guid.Empty) throw new ArgumentNullException($"OrganizationGuid is empty: {message.OrganizationGuid}");
            if (message.VacancyGuid == Guid.Empty) throw new ArgumentNullException($"VacancyGuid is empty: {message.VacancyGuid}");

            Organization organization = _repository.GetById<Organization>(message.OrganizationGuid);
            organization.SwitchVacancyInCommittee(message.VacancyGuid);
            _repository.Save(organization, Guid.NewGuid(), null);
        }
    }
    public class CloseVacancyCommandHandler : RequestHandler<CloseVacancyCommand>
    {
        private readonly IRepository _repository;

        public CloseVacancyCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected override void HandleCore(CloseVacancyCommand message)
        {
            if (message.OrganizationGuid == Guid.Empty) throw new ArgumentNullException($"OrganizationGuid is empty: {message.OrganizationGuid}");
            if (message.VacancyGuid == Guid.Empty) throw new ArgumentNullException($"VacancyGuid is empty: {message.VacancyGuid}");

            //if (message.WinnerGuid == Guid.Empty) throw new ArgumentNullException($"WinnerGuid is empty: {message.WinnerGuid}");
            //if (message.WinnerVacancyApplicationGuid == Guid.Empty) throw new ArgumentNullException($"WinnerVacancyApplicationGuid is empty: {message.WinnerVacancyApplicationGuid}");

            //if (message.PretenderGuid == Guid.Empty) throw new ArgumentNullException($"PretenderGuid is empty: {message.PretenderGuid}");
            //if (message.PretenderVacancyApplicationGuid == Guid.Empty) throw new ArgumentNullException($"PretenderVacancyApplicationGuid is empty: {message.PretenderVacancyApplicationGuid}");

            Organization organization = _repository.GetById<Organization>(message.OrganizationGuid);
            organization.CloseVacancy(message.VacancyGuid, message.WinnerGuid, message.PretenderGuid);
            _repository.Save(organization, Guid.NewGuid(), null);


            //Researcher winner = _repository.GetById<Researcher>(message.WinnerGuid);
            //winner.MakeVacancyApplicationWinner(message.WinnerVacancyApplicationGuid, message.Reason);
            //_repository.Save(winner, Guid.NewGuid(), null);

            //Researcher pretender = _repository.GetById<Researcher>(message.PretenderGuid);
            //pretender.MakeVacancyApplicationPretender(message.VacancyApplicationGuid, message.Reason);
            //_repository.Save(pretender, Guid.NewGuid(), null);
        }
    }
    public class CancelVacancyCommandHandler : RequestHandler<CancelVacancyCommand>
    {
        private readonly IRepository _repository;

        public CancelVacancyCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected override void HandleCore(CancelVacancyCommand message)
        {
            if (message.OrganizationGuid == Guid.Empty) throw new ArgumentNullException($"OrganizationGuid is empty: {message.OrganizationGuid}");
            if (message.VacancyGuid == Guid.Empty) throw new ArgumentNullException($"VacancyGuid is empty: {message.VacancyGuid}");

            Organization organization = _repository.GetById<Organization>(message.OrganizationGuid);
            organization.CancelVacancy(message.VacancyGuid, message.Reason);
            _repository.Save(organization, Guid.NewGuid(), null);
        }
    }
    public class SetVacancyWinnerCommandHandler : RequestHandler<SetVacancyWinnerCommand>
    {
        private readonly IRepository _repository;

        public SetVacancyWinnerCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected override void HandleCore(SetVacancyWinnerCommand message)
        {
            if (message.OrganizationGuid == Guid.Empty) throw new ArgumentNullException($"OrganizationGuid is empty: {message.OrganizationGuid}");
            if (message.VacancyGuid == Guid.Empty) throw new ArgumentNullException($"VacancyGuid is empty: {message.VacancyGuid}");

            if (message.ResearcherGuid == Guid.Empty) throw new ArgumentNullException($"ResearcherGuid is empty: {message.ResearcherGuid}");
            if (message.VacancyApplicationGuid == Guid.Empty) throw new ArgumentNullException($"VacancyApplicationGuid is empty: {message.VacancyApplicationGuid}");

            Organization organization = _repository.GetById<Organization>(message.OrganizationGuid);
            organization.SetVacancyWinner(message.VacancyGuid, message.ResearcherGuid, message.VacancyApplicationGuid, message.Reason);
            _repository.Save(organization, Guid.NewGuid(), null);

            Researcher researcher = _repository.GetById<Researcher>(message.ResearcherGuid);
            researcher.MakeVacancyApplicationWinner(message.VacancyApplicationGuid, message.Reason);
            _repository.Save(researcher, Guid.NewGuid(), null);
        }
    }
    public class SetVacancyPretenderCommandHandler : RequestHandler<SetVacancyPretenderCommand>
    {
        private readonly IRepository _repository;

        public SetVacancyPretenderCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected override void HandleCore(SetVacancyPretenderCommand message)
        {
            if (message.OrganizationGuid == Guid.Empty) throw new ArgumentNullException($"OrganizationGuid is empty: {message.OrganizationGuid}");
            if (message.VacancyGuid == Guid.Empty) throw new ArgumentNullException($"VacancyGuid is empty: {message.VacancyGuid}");

            if (message.ResearcherGuid == Guid.Empty) throw new ArgumentNullException($"ResearcherGuid is empty: {message.ResearcherGuid}");
            if (message.VacancyApplicationGuid == Guid.Empty) throw new ArgumentNullException($"VacancyApplicationGuid is empty: {message.VacancyApplicationGuid}");

            Organization organization = _repository.GetById<Organization>(message.OrganizationGuid);
            organization.SetVacancyPretender(message.VacancyGuid, message.ResearcherGuid, message.VacancyApplicationGuid, message.Reason);
            _repository.Save(organization, Guid.NewGuid(), null);

            Researcher researcher = _repository.GetById<Researcher>(message.ResearcherGuid);
            researcher.MakeVacancyApplicationPretender(message.VacancyApplicationGuid, message.Reason);
            _repository.Save(researcher, Guid.NewGuid(), null);
        }
    }

    public class AddVacancyToFavoritesCommandHandler : IRequestHandler<AddVacancyToFavoritesCommand, int>
    {
        private readonly IRepository _repository;

        public AddVacancyToFavoritesCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public int Handle(AddVacancyToFavoritesCommand message)
        {
            if (message.ResearcherGuid == Guid.Empty) throw new ArgumentNullException($"ResearcherGuid is empty: {message.ResearcherGuid}");
            if (message.VacancyGuid == Guid.Empty) throw new ArgumentNullException($"VacancyGuid is empty: {message.VacancyGuid}");

            Researcher researcher = _repository.GetById<Researcher>(message.ResearcherGuid);
            int favoritesCount = researcher.AddVacancyToFavorites(message.VacancyGuid);
            _repository.Save(researcher, Guid.NewGuid(), null);

            return favoritesCount;
        }
    }
    public class RemoveVacancyFromFavoritesCommandHandler : IRequestHandler<RemoveVacancyFromFavoritesCommand, int>
    {
        private readonly IRepository _repository;

        public RemoveVacancyFromFavoritesCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public int Handle(RemoveVacancyFromFavoritesCommand message)
        {
            if (message.ResearcherGuid == Guid.Empty) throw new ArgumentNullException($"ResearcherGuid is empty: {message.ResearcherGuid}");
            if (message.VacancyGuid == Guid.Empty) throw new ArgumentNullException($"VacancyGuid is empty: {message.VacancyGuid}");

            Researcher researcher = _repository.GetById<Researcher>(message.ResearcherGuid);
            int favoritesCount = researcher.RemoveVacancyFromFavorites(message.VacancyGuid);
            _repository.Save(researcher, Guid.NewGuid(), null);

            return favoritesCount;
        }
    }
}