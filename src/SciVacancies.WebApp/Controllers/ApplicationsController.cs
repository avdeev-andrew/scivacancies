﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using SciVacancies.Domain.DataModels;
using SciVacancies.Domain.Enums;
using SciVacancies.ReadModel.Core;
using SciVacancies.WebApp.Commands;
using SciVacancies.WebApp.Engine;
using SciVacancies.WebApp.Queries;
using SciVacancies.WebApp.ViewModels;

namespace SciVacancies.WebApp.Controllers
{
    [ResponseCache(NoStore = true)]
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ApplicationsController(IMediator mediator, IHostingEnvironment hostingEnvironment)
        {
            _mediator = mediator;
            _hostingEnvironment = hostingEnvironment;
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

            var researcher = _mediator.Send(new SingleResearcherQuery { ResearcherGuid = researcherGuid });
            researcher.educations = _mediator.Send(new SelectResearcherEducationsQuery { ResearcherGuid = researcherGuid }).ToList();
            researcher.publications = _mediator.Send(new SelectResearcherPublicationsQuery { ResearcherGuid = researcherGuid }).ToList();
            var vacancy = _mediator.Send(new SingleVacancyQuery { VacancyGuid = vacancyGuid });

            //TODO: оптимизировать запрос и его обработку
            var appliedVacancyApplications =
                _mediator.Send(new SelectPagedVacancyApplicationsByVacancyQuery
                {
                    PageSize = 1000,
                    VacancyGuid = vacancyGuid,
                    PageIndex = 1,
                    OrderBy = nameof(VacancyApplication.creation_date),
                    OrderDirection = ConstTerms.OrderByDescending
                });

            if (appliedVacancyApplications.Items.Count > 0
                && appliedVacancyApplications.Items.Where(c => c.status == VacancyApplicationStatus.Applied).Select(c => c.researcher_guid).Distinct().ToList().Any(c => c == researcherGuid))
                return View("Error", "Вы не можете подать повторную Заявку на Вакансию ");

            var model = new VacancyApplicationCreateViewModel
            {
                ResearcherGuid = researcherGuid,
                VacancyGuid = vacancyGuid,
                PositionName = vacancy.name,
                Email = researcher.email,
                Phone = researcher.phone,
                ResearcherFullName = $"{researcher.secondname} {researcher.firstname} {researcher.patronymic}",
                Educations = Mapper.Map<List<EducationEditViewModel>>(researcher.educations),
                Publications = Mapper.Map<List<PublicationEditViewModel>>(researcher.publications),
                ResearchActivity = researcher.research_activity,
                TeachingActivity = researcher.teaching_activity,
                OtherActivity = researcher.other_activity,
                ScienceDegree = researcher.science_degree,
                ScienceRank = researcher.science_rank,
                Rewards = researcher.rewards
            };
            //TODO: Applications -> Create : вернуть добавление дополнительнительных публикаций
            return View(model);
        }


        [Authorize(Roles = ConstTerms.RequireRoleResearcher)]
        [HttpPost]
        [BindResearcherIdFromClaims]
        public IActionResult Create(VacancyApplicationCreateViewModel model, Guid researcherGuid)
        {
            if (model.VacancyGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(model.VacancyGuid), "Не указан идентификатор Вакансии");

            var vacancyData = _mediator.Send(new SingleVacancyQuery { VacancyGuid = model.VacancyGuid });

            if (vacancyData.status != VacancyStatus.Published)
                return View("Error", $"Вы не можете подать Заявку на Вакансию в статусе: {vacancyData.status.GetDescription()}");

            //TODO: оптимизировать запрос и его обработку
            var appliedVacancyApplications =
                _mediator.Send(new SelectPagedVacancyApplicationsByVacancyQuery
                {
                    PageSize = 1000,
                    VacancyGuid = model.VacancyGuid,
                    PageIndex = 1,
                    OrderBy = nameof(VacancyApplication.creation_date),
                    OrderDirection = ConstTerms.OrderByDescending
                });

            if (appliedVacancyApplications.Items.Count > 0
                && appliedVacancyApplications.Items.Where(c => c.status == VacancyApplicationStatus.Applied).Select(c => c.researcher_guid).Distinct().ToList().Any(c => c == researcherGuid))
                return View("Error", "Вы не можете подать повторную Заявку на Вакансию");

            //TODO: Application -> Attachments : как проверять безопасность, прикрепляемых файлов
            //TODO: Application -> Attachments : вынести в конфиг это магическое число
            if (model.Attachments != null && model.Attachments.Any(c => c.Length > 100000))
            {
                ModelState.AddModelError("Attachments", @"Размер изображения превышает допустимый объём. Повторите создания Заявки ещё раз.");
            }

            if (!ModelState.IsValid)
                return View(model);


            var attachmentsList = new List<SciVacancies.Domain.Core.VacancyApplicationAttachment>();
            var newFolderName = Guid.NewGuid();
            //save attachments
            var fullDirectoryPath = $"{_hostingEnvironment.WebRootPath}{ConstTerms.FolderApplicationsAttachments}\\{newFolderName}\\";

            if (model.Attachments != null && model.Attachments.Any())
            {
                foreach (var file in model.Attachments)
                {
                    var fileName = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition)
                        .FileName
                        .Trim('"');

                    //сценарий-А: сохранить файл на диск
                    try
                    {
                        //TODO: Application -> Attachments : что делать с Директорией при удалении(отмене, отклонении) Заявки
                        //TODO: Application -> Attachments : как искать Текущую директорию при повторном добавлении(изменении текущего списка) файлов
                        //TODO: Application -> Attachments : можно ли редактировать список файлов, или Заявки создаются разово и для каждой генеиртся новая папка с вложениями
                        Directory.CreateDirectory(fullDirectoryPath);
                        var filePath =
                            $"{_hostingEnvironment.WebRootPath}{ConstTerms.FolderApplicationsAttachments}\\{newFolderName}\\{fileName}";
                        file.SaveAs(filePath);
                        attachmentsList.Add(new SciVacancies.Domain.Core.VacancyApplicationAttachment
                        {
                            Size = file.Length,
                            Extension = fileName.Split('.')[1],
                            Name = fileName,
                            UploadDate = DateTime.Now,
                            Url = $"/{newFolderName}/{fileName}"
                        });

                    }
                    catch (Exception)
                    {
                        RemoveAttachmentDirectory(fullDirectoryPath);
                        return View("Error", "Ошибка при сохранении прикреплённых файлов");
                    }

                    //TODO: сохранение файл в БД (сделать)
                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    файл в byte
                    //    byte[] byteData;
                    //    //сценарий-Б: сохранить файл в БД
                    //    //var openReadStream = file.OpenReadStream();
                    //    //var scale = (int)(500000 / file.Length);
                    //    //var resizedImage = new Bitmap(image, new Size(image.Width * scale, image.Height * scale));
                    //    //((Image)resizedImage).Save(memoryStream, ImageFormat.Jpeg);
                    //    //byteData = memoryStream.ToArray();
                    //    //memoryStream.SetLength(0);

                    //    //сценарий-В: сохранить файл в БД
                    //    //var openReadStream = file.OpenReadStream();
                    //    //openReadStream.CopyTo(memoryStream);
                    //    //byteData = memoryStream.ToArray();
                    //    //memoryStream.SetLength(0);
                    //}

                }
            }


            Guid vacancyApplicationGuid;
            try
            {
                //с формы мы не получаем практически никаких данных, поэтоу заново наполняем ViewModel
                var researcher = _mediator.Send(new SingleResearcherQuery { ResearcherGuid = researcherGuid });
                researcher.educations = _mediator.Send(new SelectResearcherEducationsQuery { ResearcherGuid = researcherGuid }).ToList();
                researcher.publications = _mediator.Send(new SelectResearcherPublicationsQuery { ResearcherGuid = researcherGuid }).ToList();
                model.Conferences = researcher.conferences;
                model.Educations = Mapper.Map<List<EducationEditViewModel>>(researcher.educations);
                model.Email = researcher.email;
                model.ExtraEmail = researcher.extraemail;
                model.ExtraPhone = researcher.extraphone;
                model.OtherActivity = researcher.other_activity;
                model.Phone = researcher.phone;
                model.Publications = Mapper.Map<List<PublicationEditViewModel>>(researcher.publications);
                model.PositionName = vacancyData.name;
                model.ResearchActivity = researcher.research_activity;
                model.ResearcherFullName = $"{researcher.secondname} {researcher.firstname} {researcher.patronymic}";
                model.ResearcherGuid = researcherGuid;
                model.Rewards = researcher.rewards;
                model.ScienceDegree = researcher.science_degree;
                model.ScienceRank = researcher.science_rank;
                model.TeachingActivity = researcher.teaching_activity;
                model.VacancyGuid = vacancyData.guid;
                var data = Mapper.Map<VacancyApplicationDataModel>(model);
                data.Attachments = attachmentsList;
                vacancyApplicationGuid = _mediator.Send(new CreateAndApplyVacancyApplicationCommand { ResearcherGuid = model.ResearcherGuid, VacancyGuid = model.VacancyGuid, Data = data });
            }
            catch (Exception)
            {
                RemoveAttachmentDirectory(fullDirectoryPath);
                return View("Error", "Что-то пошло не так при сохранении Заявки");
            }
            return RedirectToAction("details", "applications", new { id = vacancyApplicationGuid });
        }

        private void RemoveAttachmentDirectory(string fullpath)
        {
            if (Directory.Exists(fullpath))
                Directory.Delete(fullpath);
        }

        [Authorize(Roles = ConstTerms.RequireRoleResearcher)]
        [PageTitle("Детали заявки")]
        [BindResearcherIdFromClaims]
        public IActionResult Details(Guid id, Guid researcherGuid)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            var preModel = _mediator.Send(new SingleVacancyApplicationQuery { VacancyApplicationGuid = id });

            if (preModel == null)
                return HttpNotFound(); //throw new ObjectNotFoundException($"Не найдена Заявка c идентификатором: {id}");

            if (researcherGuid != Guid.Empty
                && User.IsInRole(ConstTerms.RequireRoleResearcher)
                && preModel.researcher_guid != researcherGuid)
                return View("Error", "Вы не можете просматривать Заявки других соискателей.");

            var model = Mapper.Map<VacancyApplicationDetailsViewModel>(preModel);
            model.Researcher = Mapper.Map<ResearcherDetailsViewModel>(_mediator.Send(new SingleResearcherQuery { ResearcherGuid = researcherGuid }));
            model.Researcher.Publications = Mapper.Map<List<PublicationEditViewModel>>(_mediator.Send(new SelectResearcherPublicationsQuery { ResearcherGuid = researcherGuid }));
            model.Researcher.Educations = Mapper.Map<List<EducationEditViewModel>>(_mediator.Send(new SelectResearcherEducationsQuery { ResearcherGuid = researcherGuid }));
            model.Vacancy = Mapper.Map<VacancyDetailsViewModel>(_mediator.Send(new SingleVacancyQuery { VacancyGuid = preModel.vacancy_guid }));
            model.Attachments = _mediator.Send(new SelectVacancyApplicationAttachmentsQuery { VacancyApplicationGuid = id });
            //TODO: ntemnikov : показать Научные интересы
            return View(model);
        }

        /// <summary>
        /// организации рпосматривают присланные Заявки
        /// </summary>
        /// <param name="id"></param>
        /// <param name="organizationGuid"></param>
        /// <returns></returns>
        [Authorize(Roles = ConstTerms.RequireRoleOrganizationAdmin)]
        [PageTitle("Детали заявки")]
        [BindOrganizationIdFromClaims]
        public IActionResult Preview(Guid id, Guid organizationGuid)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            var preModel = _mediator.Send(new SingleVacancyApplicationQuery { VacancyApplicationGuid = id });

            if (preModel == null)
                return HttpNotFound(); //throw new ObjectNotFoundException($"Не найдена Заявка c идентификатором: {id}");

            if (preModel.status == VacancyApplicationStatus.InProcess
                || preModel.status == VacancyApplicationStatus.Cancelled
                || preModel.status == VacancyApplicationStatus.Lost
                || preModel.status == VacancyApplicationStatus.Removed)
                return View("Error", $"Вы не можете просматривать Заявку со статусом: {preModel.status.GetDescription()}");

            var vacancy = _mediator.Send(new SingleVacancyQuery { VacancyGuid = preModel.vacancy_guid });
            if (vacancy.organization_guid != organizationGuid)
                return View("Error", "Вы не можете просматривать Заявки, поданные на вакансии других организаций.");

            var model = Mapper.Map<VacancyApplicationDetailsViewModel>(preModel);
            model.Researcher = Mapper.Map<ResearcherDetailsViewModel>(_mediator.Send(new SingleResearcherQuery { ResearcherGuid = preModel.researcher_guid }));
            model.Researcher.Publications = Mapper.Map<List<PublicationEditViewModel>>(_mediator.Send(new SelectResearcherPublicationsQuery { ResearcherGuid = preModel.researcher_guid }));
            model.Researcher.Educations = Mapper.Map<List<EducationEditViewModel>>(_mediator.Send(new SelectResearcherEducationsQuery { ResearcherGuid = preModel.researcher_guid }));
            model.Vacancy = Mapper.Map<VacancyDetailsViewModel>(vacancy);
            //TODO: ntemnikov : показать Приложенные файлы
            //TODO: ntemnikov : показать Научные интересы
            return View(model);
        }


        private object SetWinnerPreValidation(Guid vacancyApplicationGuid, Guid organizationGuid, out Vacancy vacancy, bool isWinner)
        {
            var vacancyApplicaiton = _mediator.Send(new SingleVacancyApplicationQuery { VacancyApplicationGuid = vacancyApplicationGuid });
            vacancy = null;

            if (vacancyApplicaiton == null)
                return HttpNotFound(); //throw new ObjectNotFoundException($"Не найдена Заявка c идентификатором: {vacancyApplicationGuid}");

            if (vacancyApplicaiton.status != VacancyApplicationStatus.Applied)
                return View("Error",
                    $"Вы не можете выбрать в качестве одного из победителей Заявку со статусом: {vacancyApplicaiton.status.GetDescription()}");

            vacancy = _mediator.Send(new SingleVacancyQuery { VacancyGuid = vacancyApplicaiton.vacancy_guid });

            if (vacancy == null)
                return HttpNotFound(); //throw new ObjectNotFoundException($"Не найдена Вакансия c идентификатором: {vacancyApplicaiton.vacancy_guid}");

            if (vacancy.organization_guid != organizationGuid)
                return View("Error", "Вы не можете изменять Заявки, поданные на вакансии других организаций.");

            if (isWinner && vacancy.winner_researcher_guid != Guid.Empty)
                return View("Error", "Для данной Вакансии уже выбран Победитель.");

            if (!isWinner && vacancy.winner_researcher_guid == Guid.Empty)
                return View("Error", "Для данной Вакансии еще не выбран Победитель.");

            if (vacancy.winner_researcher_guid != Guid.Empty && vacancy.pretender_researcher_guid != Guid.Empty)
                return View("Error", "Для данной Вакансии уже выбраны Победитель и Претендент.");

            if (vacancy.status != VacancyStatus.InCommittee)
                return View("Error",
                    $"Вы не можете выбирать победителя для Заявки со статусом: {vacancy.status.GetDescription()}");
            return vacancyApplicaiton;
        }

        [PageTitle("Выбор победителя")]
        [BindOrganizationIdFromClaims]
        [Authorize(Roles = ConstTerms.RequireRoleOrganizationAdmin)]
        public IActionResult SetWinner(Guid id, Guid organizationGuid, bool isWinner)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));
            if (organizationGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(organizationGuid));

            Vacancy vacancy;
            var result = SetWinnerPreValidation(id, organizationGuid, out vacancy, isWinner);
            if (result is HttpNotFoundResult) return (HttpNotFoundResult)result;
            var vacancyApplication = (VacancyApplication)result;

            var model = Mapper.Map<VacancyApplicationSetWinnerViewModel>(vacancyApplication);
            model.Vacancy = Mapper.Map<VacancyDetailsViewModel>(vacancy);
            model.Researcher = Mapper.Map<ResearcherDetailsViewModel>(_mediator.Send(new SingleResearcherQuery { ResearcherGuid = vacancyApplication.researcher_guid }));
            model.IsWinner = isWinner;
            model.SetWinner = isWinner;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PageTitle("Выбор победителя")]
        [BindOrganizationIdFromClaims]
        [Authorize(Roles = ConstTerms.RequireRoleOrganizationAdmin)]
        public IActionResult SetWinner(VacancyApplicationSetWinnerViewModel model, Guid organizationGuid)
        {
            if (organizationGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(organizationGuid));

            Vacancy vacancy;
            var result = SetWinnerPreValidation(model.Guid, organizationGuid, out vacancy, model.SetWinner);
            if (result is HttpNotFoundResult) return (HttpNotFoundResult)result;
            var vacancyApplication = (VacancyApplication)result;

            if (ModelState.IsValid)
            {
                if (model.IsWinner)
                    _mediator.Send(new SetVacancyWinnerCommand
                    {
                        VacancyGuid = model.VacancyGuid,
                        ResearcherGuid = model.ResearcherGuid,
                        VacancyApplicationGuid = model.Guid,
                        Reason = model.Reason
                    });
                else
                    _mediator.Send(new SetVacancyPretenderCommand
                    {
                        VacancyGuid = model.VacancyGuid,
                        ResearcherGuid = model.ResearcherGuid,
                        VacancyApplicationGuid = model.Guid,
                        Reason = model.Reason
                    });

                return RedirectToAction("preview", "applications", new { id = model.Guid });
            }

            model = Mapper.Map<VacancyApplicationSetWinnerViewModel>(vacancyApplication);
            model.Vacancy = Mapper.Map<VacancyDetailsViewModel>(vacancy);
            model.Researcher = Mapper.Map<ResearcherDetailsViewModel>(_mediator.Send(new SingleResearcherQuery { ResearcherGuid = vacancyApplication.researcher_guid }));

            return View(model);
        }

        //[PageTitle("Удаление шаблона заявки")]
        //[Authorize(Roles = ConstTerms.RequireRoleResearcher)]
        //[BindResearcherIdFromClaims]
        //public IActionResult Delete(Guid id, Guid researcherGuid)
        //{
        //    if (id == Guid.Empty)
        //        throw new ArgumentNullException(nameof(id));
        //    if (researcherGuid == Guid.Empty)
        //        throw new ArgumentNullException(nameof(researcherGuid));

        //    var preModel = _mediator.Send(new SingleVacancyApplicationQuery { VacancyApplicationGuid = id });

        //    if (preModel.status != VacancyApplicationStatus.Applied)
        //        return View("Error", $"Отменить заявку можно только в статусе: {VacancyApplicationStatus.InProcess.GetDescription()}.");

        //    var model = Mapper.Map<VacancyApplicationDetailsViewModel>(preModel);
        //    model.Researcher = Mapper.Map<ResearcherDetailsViewModel>(_mediator.Send(new SingleResearcherQuery { ResearcherGuid = model.ResearcherGuid }));
        //    model.Vacancy = Mapper.Map<VacancyDetailsViewModel>(_mediator.Send(new SingleVacancyQuery { VacancyGuid = model.VacancyGuid }));
        //    return View(model);
        //}

        //[HttpPost]
        //[PageTitle("Удаление шаблона заявки")]
        //[Authorize(Roles = ConstTerms.RequireRoleResearcher)]
        //[BindResearcherIdFromClaims]
        //public IActionResult DeletePost(Guid id, Guid researcherGuid)
        //{
        //    if (id == Guid.Empty)
        //        throw new ArgumentNullException(nameof(id));
        //    if (researcherGuid == Guid.Empty)
        //        throw new ArgumentNullException(nameof(researcherGuid));

        //    var preModel = _mediator.Send(new SingleVacancyApplicationQuery { VacancyApplicationGuid = id });

        //    if (preModel.status != VacancyApplicationStatus.Applied)
        //        return View("Error", $"Отменить заявку можно только в статусе: {VacancyApplicationStatus.InProcess.GetDescription()}.");

        //    if (!ModelState.IsValid)
        //    {
        //        var model = Mapper.Map<VacancyApplicationDetailsViewModel>(preModel);
        //        model.Researcher = Mapper.Map<ResearcherDetailsViewModel>(_mediator.Send(new SingleResearcherQuery { ResearcherGuid = model.ResearcherGuid }));
        //        model.Vacancy = Mapper.Map<VacancyDetailsViewModel>(_mediator.Send(new SingleVacancyQuery { VacancyGuid = model.VacancyGuid }));
        //        return View("delete", model);
        //    }

        //    _mediator.Send(new CancelVacancyApplicationCommand { ResearcherGuid = researcherGuid, VacancyApplicationGuid = id });
        //    return RedirectToAction("applications", "researchers");
        //}

        [Authorize(Roles = ConstTerms.RequireRoleOrganizationAdmin)]
        [PageTitle("Детали заявки")]
        [BindOrganizationIdFromClaims]
        public IActionResult Print(Guid id, Guid organizationGuid)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            var preModel = _mediator.Send(new SingleVacancyApplicationQuery { VacancyApplicationGuid = id });

            if (preModel == null)
                return HttpNotFound(); //throw new ObjectNotFoundException($"Не найдена Заявка c идентификатором: {id}");

            if (preModel.status == VacancyApplicationStatus.InProcess
                || preModel.status == VacancyApplicationStatus.Cancelled
                || preModel.status == VacancyApplicationStatus.Lost
                || preModel.status == VacancyApplicationStatus.Removed)
                return View("Error", $"Вы не можете просматривать Заявку со статусом: {preModel.status.GetDescription()}");

            var vacancy = _mediator.Send(new SingleVacancyQuery { VacancyGuid = preModel.vacancy_guid });
            if (vacancy.organization_guid != organizationGuid)
                return View("Error", "Вы не можете изменять Заявки, поданные на вакансии других организаций.");

            var model = Mapper.Map<VacancyApplicationDetailsViewModel>(preModel);
            model.Researcher = Mapper.Map<ResearcherDetailsViewModel>(_mediator.Send(new SingleResearcherQuery { ResearcherGuid = preModel.researcher_guid }));
            model.Researcher.Publications = Mapper.Map<List<PublicationEditViewModel>>(_mediator.Send(new SelectResearcherPublicationsQuery { ResearcherGuid = preModel.researcher_guid }));
            model.Researcher.Educations = Mapper.Map<List<EducationEditViewModel>>(_mediator.Send(new SelectResearcherEducationsQuery { ResearcherGuid = preModel.researcher_guid }));
            model.Vacancy = Mapper.Map<VacancyDetailsViewModel>(vacancy);

            return View(model);
        }

        private object OfferAcceptionPreValidation(Guid vacancyApplicationGuid, Guid organizationGuid, bool isWinner)
        {
            var vacancyApplicaiton = _mediator.Send(new SingleVacancyApplicationQuery { VacancyApplicationGuid = vacancyApplicationGuid });

            if (vacancyApplicaiton == null)
                return HttpNotFound(); //throw new ObjectNotFoundException($"Не найдена Заявка c идентификатором: {vacancyApplicationGuid}");

            if (isWinner && vacancyApplicaiton.status != VacancyApplicationStatus.Won)
                return View("Error", $"Вы не можете зафиксировать принятие предложения от Победителя если Заявка имеет статус: {vacancyApplicaiton.status.GetDescription()}");

            if (!isWinner && vacancyApplicaiton.status != VacancyApplicationStatus.Pretended)
                return View("Error", $"Вы не можете зафиксировать принятие предложения от Претендента если Заявка имеет статус: {vacancyApplicaiton.status.GetDescription()}");

            var vacancy = _mediator.Send(new SingleVacancyQuery { VacancyGuid = vacancyApplicaiton.vacancy_guid });

            if (vacancy == null)
                return HttpNotFound(); //throw new ObjectNotFoundException($"Не найдена Вакансия c идентификатором: {vacancyApplicaiton.vacancy_guid}");

            if (vacancy.organization_guid != organizationGuid)
                return View("Error", "Вы не можете изменять Заявки, поданные на вакансии других организаций.");

            if (vacancy.status != VacancyStatus.OfferResponseAwaiting)
                return View("Error",
                    $"Вы не можете зафиксироватиьо принятие или отказ от предложения если Заявка имеет статус: {vacancy.status.GetDescription()}");
            return vacancy;
        }

        [Authorize(Roles = ConstTerms.RequireRoleOrganizationAdmin)]
        [BindOrganizationIdFromClaims]
        public IActionResult OfferAcception(Guid id, Guid organizationGuid, bool isWinner, bool hasAccepted)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));
            if (organizationGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(organizationGuid));

            var result = OfferAcceptionPreValidation(id, organizationGuid, isWinner);
            if (result is HttpNotFoundResult) return (HttpNotFoundResult)result;
            var vacancy = (Vacancy)result;

            if (isWinner)
                if (hasAccepted)
                    _mediator.Send(new SetWinnerAcceptOfferCommand
                    {
                        VacancyGuid = vacancy.guid
                    });
                else
                    _mediator.Send(new SetWinnerRejectOfferCommand
                    {
                        VacancyGuid = vacancy.guid
                    });
            else
                if (hasAccepted)
                _mediator.Send(new SetPretenderAcceptOfferCommand
                {
                    VacancyGuid = vacancy.guid
                });
            else
                _mediator.Send(new SetPretenderRejectOfferCommand
                {
                    VacancyGuid = vacancy.guid
                });

            return RedirectToAction("preview", "applications", new { id });

        }

    }
}
