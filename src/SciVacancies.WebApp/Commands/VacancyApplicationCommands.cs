﻿using SciVacancies.Domain.DataModels;

using System;

using MediatR;

namespace SciVacancies.WebApp.Commands
{
    public class CreateVacancyApplicationTemplateCommand : CommandBase, IRequest<Guid>
    {
        public Guid ResearcherGuid { get; set; }
        public Guid VacancyGuid { get; set; }

        public VacancyApplicationDataModel Data { get; set; }
    }
    public class CreateAndApplyVacancyApplicationCommand : CommandBase, IRequest<Guid>
    {
        public Guid ResearcherGuid { get; set; }
        public Guid VacancyGuid { get; set; }

        public VacancyApplicationDataModel Data { get; set; }
    }
    public class UpdateVacancyApplicationTemplateCommand : CommandBase, IRequest
    {
        public Guid ResearcherGuid { get; set; }
        public Guid VacancyApplicationGuid { get; set; }

        public VacancyApplicationDataModel Data { get; set; }
    }
    public class RemoveVacancyApplicationTemplateCommand : CommandBase, IRequest
    {
        public Guid ResearcherGuid { get; set; }
        public Guid VacancyApplicationGuid { get; set; }
    }
    public class ApplyVacancyApplicationCommand : CommandBase, IRequest
    {
        public Guid ResearcherGuid { get; set; }
        public Guid VacancyApplicationGuid { get; set; }
    }
    public class CancelVacancyApplicationCommand : CommandBase, IRequest
    {
        public Guid ResearcherGuid { get; set; }
        public Guid VacancyApplicationGuid { get; set; }
    }
    public class MakeVacancyApplicationWinnerCommand : CommandBase, IRequest
    {
        public Guid VacancyApplicationGuid { get; set; }
        public Guid ResearcherGuid { get; set; }

        public string Reason { get; set; }
    }
    public class MakeVacancyApplicationPretenderCommand : CommandBase, IRequest
    {
        public Guid VacancyApplicationGuid { get; set; }
        public Guid ResearcherGuid { get; set; }

        public string Reason { get; set; }
    }
    public class MakeVacancyApplicationLooserCommand : CommandBase, IRequest
    {
        public Guid VacancyApplicationGuid { get; set; }
        public Guid ResearcherGuid { get; set; }
    }
}
