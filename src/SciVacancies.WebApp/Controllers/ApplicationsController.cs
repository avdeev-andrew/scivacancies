﻿using System;
using AutoMapper;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using SciVacancies.Domain.DataModels;
using SciVacancies.Domain.Enums;
using SciVacancies.ReadModel;
using SciVacancies.WebApp.Commands;
using SciVacancies.WebApp.Engine;
using SciVacancies.WebApp.Queries;
using SciVacancies.WebApp.ViewModels;

namespace SciVacancies.WebApp.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IReadModelService _readModelService;

        public ApplicationsController(IMediator mediator, IReadModelService readModelService)
        {
            _mediator = mediator;
            _readModelService = readModelService;
        }

        [Authorize(Roles = ConstTerms.RequireRoleResearcher)]
        [PageTitle("Новая заявка")]
        [BindResearcherIdFromClaims]
        public ViewResult Create(Guid researcherGuid, Guid vacancyGuid)
        {
            if (researcherGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(researcherGuid));
            if (vacancyGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(vacancyGuid));

            //var researcher = _mediator.Send(new SingleResearcherQuery { ResearcherGuid = researcherGuid });
            var researcher = _readModelService.SingleResearcher(researcherGuid);
            //var vacancy = _mediator.Send(new sin)
            var vacancy = _readModelService.SingleVacancy(vacancyGuid);

            var model = new VacancyApplicationCreateViewModel
            {
                ResearcherGuid = researcherGuid,
                VacancyGuid = vacancyGuid,
                PositionName = vacancy.Name,
                Email = researcher.Email,
                Phone = researcher.Phone,
                ResearcherFullName = $"{researcher.SecondName} {researcher.FirstName} {researcher.Patronymic}",
                ResearchActivity = researcher.ResearchActivity,
                TeachingActivity = researcher.TeachingActivity,
                OtherActivity = researcher.OtherActivity,
                ScienceDegree = researcher.ScienceDegree,
                AcademicStatus = researcher.AcademicStatus,
                Rewards = researcher.Rewards
            };

            return View(model);
        }


        [Authorize(Roles = ConstTerms.RequireRoleResearcher)]
        [HttpPost]
        public ActionResult Create(VacancyApplicationCreateViewModel model)
        {
            var data = Mapper.Map<VacancyApplicationDataModel>(model);
            var vacancyApplicationGuid = _mediator.Send(new CreateVacancyApplicationTemplateCommand
            {
                ResearcherGuid = model.ResearcherGuid,
                VacancyGuid = model.VacancyGuid,
                Data = data
            });
            return RedirectToAction("applications", "researchers", new { id = vacancyApplicationGuid });
        }

        [Authorize(Roles = ConstTerms.RequireRoleResearcher)]
        [PageTitle("Детали заявки")]
        public ViewResult Details(Guid id)
        {
            //_mediator.Send(new SingleVacancyApplicationQuery { ResearcherGuid = id });
            _readModelService.SingleResearcher(id);

            return View();
        }

        [Authorize(Roles = ConstTerms.RequireRoleOrganizationAdmin)]
        [PageTitle("Детали заявки")]
        public ViewResult ApplicationInVacancy(Guid id)
        {
            //TODO: VacancyApplication -> Details: реализовать метод просмотра отправленной заявки в вакансии
            throw new NotImplementedException();
            //_mediator.Send(new SingleVacancyApplicationQuery{});
            return View();
        }


        [Authorize(Roles = ConstTerms.RequireRoleOrganizationAdmin)]
        [BindResearcherIdFromClaims]
        public IActionResult Cancel(Guid id, Guid researcherGuid)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));
            if (researcherGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(researcherGuid));

            var vacancyApplication = _mediator.Send(new SingleVacancyApplicationQuery {VacancyApplicationGuid = id});

            if (vacancyApplication.ResearcherGuid != researcherGuid)
                throw new Exception("Заявку может отменить только Заявитель, подавший её");

            //TODO: VacancyApplication -> Cancel -> Statuses :  в каких статусах допустимо отменять поданные заявки
            if(vacancyApplication.Status!= VacancyApplicationStatus.InProcess
                && vacancyApplication.Status != VacancyApplicationStatus.Applied)
                throw new Exception($"Вы не можете отменить подачу заявки со статусом: {vacancyApplication.Status.GetDescription()}");

            //TODO: VacancyApplication -> Cancel -> Command :  Какая комманда отменяет поданные заявки
            //_mediator.Send();
            throw new NotImplementedException();

            return View();
        }
    }
}
