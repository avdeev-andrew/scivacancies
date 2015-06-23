﻿using System;
using MediatR;
using Microsoft.AspNet.Mvc;
using SciVacancies.Domain.Aggregates.Interfaces;
using SciVacancies.Domain.DataModels;
using SciVacancies.ReadModel;
using SciVacancies.WebApp.Commands;
using SciVacancies.WebApp.ViewModels;

using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SciVacancies.WebApp.Controllers
{
    public class InitializeController : Controller
    {
        private readonly IResearcherService _res;
        private readonly IOrganizationService _org;
        private readonly IReadModelService _rm;
        private readonly IMediator _mediator;

        public InitializeController(IResearcherService res, IOrganizationService org, IReadModelService rm, IMediator mediator)
        {
            _res = res;
            _org = org;
            _rm = rm;
            _mediator = mediator;
        }
        // GET: /<controller>/
        public void Index()
        {


            var command = new RegisterUserResearcherCommand
            {
                Data = new AccountRegisterViewModel
                {
                    Email = "researcher1@mailer.org",
                    UserName = "researcher1",
                    FirstName = "Генрих",
                    SecondName = "Дубощит",
                    Patronymic = "Иванович",
                    FirstNameEng = "Genrih",
                    SecondNameEng = "Pupkin",
                    PatronymicEng = "Ivanovich",
                    BirthYear = DateTime.Now.AddYears(-50).Year
                }
            };
            var user = _mediator.Send(command);

            //Guid resGuid = Guid.Parse(user.Id)/*_res.CreateResearcher(new ResearcherDataModel()*/
            //{
            //    UserId = "Cartman",
            //    FirstName = "Генрих",
            //    SecondName = "Дубощит",
            //    Patronymic = "Иванович",
            //    FirstNameEng = "Genrih",
            //    SecondNameEng = "Pupkin",
            //    PatronymicEng = "Ivanovich",
            //    BirthDate = DateTime.Now.AddYears(-50)
            //});
            //Claims.FirstOrDefault(c => c.Type == Key);
            Guid resGuid = Guid.Parse(user.Claims.Single(s => s.ClaimType.Equals("researcher_id")).ClaimValue);

            Guid subGuid = _res.CreateSearchSubscription(resGuid, new SearchSubscriptionDataModel()
            {
                Title = "Разведение лазерных акул"
            });

            Guid orgGuid = _org.CreateOrganization(new OrganizationDataModel()
            {
                Name = "Научно Исследотельский Институт Горных массивов",
                ShortName = "НИИ Горных массивов",
                OrgFormId = 1,
                FoivId = 42,
                ActivityId = 1,
                HeadFirstName = "Овидий",
                HeadLastName = "Грек",
                HeadPatronymic = "Иванович"
            });

            Guid posGuid1 = _org.CreatePosition(orgGuid, new PositionDataModel()
            {
                Name = "Разводчик акул",
                FullName = "Младший сотрудник по разведению лазерных акул",
                PositionTypeGuid = Guid.Parse("b7280ace-d237-c007-42fe-ec4aed8f52d4"),
                ResearchDirection = "Аналитическая химия",
                ResearchDirectionId = 3026
            });
            Guid posGuid2 = _org.CreatePosition(orgGuid, new PositionDataModel()
            {
                Name = "Настройщик лазеров",
                FullName = "Младший сотрудник по настройке лазеров",
                PositionTypeGuid = Guid.Parse("b7280ace-d237-c007-42fe-ec4aed8f52d4"),
                ResearchDirection = "Прикладная математика",
                ResearchDirectionId = 2999
            });
            Guid vacGuid1 = _org.PublishVacancy(orgGuid, posGuid1, new VacancyDataModel()
            {
                Name = "Разводчик акул",
                FullName = "Младший сотрудник по разведению лазерных акул",
                ResearchDirection = "Аналитическая химия",
            });


            Guid orgGuid1 = _org.CreateOrganization(new OrganizationDataModel()
            {
                Name = "НИИ добра",
                ShortName = "Good Science",
                OrgFormId = 2,
                FoivId = 42,
                ActivityId = 1,
                HeadFirstName = "Саруман",
                HeadLastName = "Саур",
                HeadPatronymic = "Сауронович"
            });

            Guid posGuid3 = _org.CreatePosition(orgGuid1, new PositionDataModel()
            {
                Name = "Ремонтник всевидящего ока",
                FullName = "Младший сотрудник по калибровке фокусного зеркала",
                PositionTypeGuid = Guid.Parse("b7280ace-d237-c007-42fe-ec4aed8f52d4"),
                ResearchDirection = "Аналитическая химия",
                ResearchDirectionId = 3026
            });
            Guid vacGuid3 = _org.PublishVacancy(orgGuid1, posGuid3, new VacancyDataModel()
            {
                Name = "Ремонтник всевидящего ока",
                FullName = "Младший сотрудник по калибровке фокусного зеркала",
                ResearchDirection = "Аналитическая химия",
            });
        }
    }
}
