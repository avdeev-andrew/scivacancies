﻿using AutoMapper;
using SciVacancies.Domain.DataModels;
using SciVacancies.Domain.Events;
using SciVacancies.ReadModel.Core;
using SciVacancies.WebApp.ViewModels;
using System.Linq;

namespace SciVacancies.WebApp.Infrastructure
{
    public static class MappingConfigurationOrganization
    {
        public static void InitializeOrganization()
        {
            //Создание организации
            Mapper.CreateMap<OrganizationCreated, Organization>()
                    .ForMember(d => d.guid, o => o.MapFrom(s => s.OrganizationGuid))
                    .ForMember(d => d.name, o => o.MapFrom(s => s.Data.Name))
                    .ForMember(d => d.shortname, o => o.MapFrom(s => s.Data.ShortName))
                    .ForMember(d => d.address, o => o.MapFrom(s => s.Data.Address))
                    .ForMember(d => d.email, o => o.MapFrom(s => s.Data.Email))
                    .ForMember(d => d.inn, o => o.MapFrom(s => s.Data.INN))
                    .ForMember(d => d.ogrn, o => o.MapFrom(s => s.Data.OGRN))
                    .ForMember(d => d.head_firstname, o => o.MapFrom(s => s.Data.HeadFirstName))
                    .ForMember(d => d.head_secondname, o => o.MapFrom(s => s.Data.HeadSecondName))
                    .ForMember(d => d.head_patronymic, o => o.MapFrom(s => s.Data.HeadPatronymic))
                    .ForMember(d => d.image_name, o => o.MapFrom(s => s.Data.ImageName))
                    .ForMember(d => d.image_size, o => o.MapFrom(s => s.Data.ImageSize))
                    .ForMember(d => d.image_extension, o => o.MapFrom(s => s.Data.ImageExtension))
                    .ForMember(d => d.image_url, o => o.MapFrom(s => s.Data.ImageUrl))
                    .ForMember(d => d.foiv_id, o => o.MapFrom(s => s.Data.FoivId))
                    .ForMember(d => d.orgform_id, o => o.MapFrom(s => s.Data.OrgFormId))
                    .ForMember(d => d.researchdirections, o => o.MapFrom(s => s.Data.ResearchDirections))
                    .ForMember(d => d.creation_date, o => o.MapFrom(s => s.TimeStamp));
            //Обновление организации
            Mapper.CreateMap<OrganizationUpdated, Organization>()
                .ForMember(d => d.guid, o => o.MapFrom(s => s.OrganizationGuid))
                .ForMember(d => d.name, o => o.MapFrom(s => s.Data.Name))
                .ForMember(d => d.shortname, o => o.MapFrom(s => s.Data.ShortName))
                .ForMember(d => d.address, o => o.MapFrom(s => s.Data.Address))
                .ForMember(d => d.email, o => o.MapFrom(s => s.Data.Email))
                .ForMember(d => d.inn, o => o.MapFrom(s => s.Data.INN))
                .ForMember(d => d.ogrn, o => o.MapFrom(s => s.Data.OGRN))
                .ForMember(d => d.head_firstname, o => o.MapFrom(s => s.Data.HeadFirstName))
                .ForMember(d => d.head_secondname, o => o.MapFrom(s => s.Data.HeadSecondName))
                .ForMember(d => d.head_patronymic, o => o.MapFrom(s => s.Data.HeadPatronymic))
                .ForMember(d => d.image_name, o => o.MapFrom(s => s.Data.ImageName))
                .ForMember(d => d.image_size, o => o.MapFrom(s => s.Data.ImageSize))
                .ForMember(d => d.image_extension, o => o.MapFrom(s => s.Data.ImageExtension))
                .ForMember(d => d.image_url, o => o.MapFrom(s => s.Data.ImageUrl))
                .ForMember(d => d.foiv_id, o => o.MapFrom(s => s.Data.FoivId))
                .ForMember(d => d.orgform_id, o => o.MapFrom(s => s.Data.OrgFormId))
                .ForMember(d => d.researchdirections, o => o.MapFrom(s => s.Data.ResearchDirections))
                .ForMember(d => d.update_date, o => o.MapFrom(s => s.TimeStamp));

            Mapper.CreateMap<Domain.Core.ResearchDirection, ReadModel.Core.ResearchDirection>()
                .ForMember(d => d.id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.parent_id, o => o.MapFrom(s => s.ParentId))
                .ForMember(d => d.title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.title_eng, o => o.MapFrom(s => s.TitleEng))
                .ForMember(d => d.lft, o => o.MapFrom(s => s.Lft))
                .ForMember(d => d.rgt, o => o.MapFrom(s => s.Rgt))
                .ForMember(d => d.lvl, o => o.MapFrom(s => s.Lvl))
                .ForMember(d => d.oecd_code, o => o.MapFrom(s => s.OecdCode))
                .ForMember(d => d.wos_code, o => o.MapFrom(s => s.WosCode))
                .ForMember(d => d.root_id, o => o.MapFrom(s => s.RootId));

            //информация об организации во View
            Mapper.CreateMap<Organization, OrganizationDetailsViewModel>().IncludePagedResultMapping()
                .ForMember(d => d.Guid, o => o.MapFrom(s => s.guid))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.name))
                .ForMember(d => d.ShortName, o => o.MapFrom(s => s.shortname))
                .ForMember(d => d.Address, o => o.MapFrom(s => s.address))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.email))
                .ForMember(d => d.INN, o => o.MapFrom(s => s.inn))
                .ForMember(d => d.OGRN, o => o.MapFrom(s => s.ogrn))
                .ForMember(d => d.HeadFirstName, o => o.MapFrom(s => s.head_firstname))
                .ForMember(d => d.HeadSecondName, o => o.MapFrom(s => s.head_secondname))
                .ForMember(d => d.HeadPatronymic, o => o.MapFrom(s => s.head_patronymic))
                .ForMember(d => d.FoivId, o => o.MapFrom(s => s.foiv_id))
                .ForMember(d => d.OrgFormId, o => o.MapFrom(s => s.orgform_id))
                .ForMember(d => d.ImageName, o => o.MapFrom(s => s.image_name))
                .ForMember(d => d.ImageSize, o => o.MapFrom(s => s.image_size))
                .ForMember(d => d.ImageExtension, o => o.MapFrom(s => s.image_extension))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.image_url))
                //.ForMember(d => d.ResearchDirectionIds, o => o.MapFrom(s => s.researchdirections))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.status));
            //Mapper.CreateMap<ResearchDirection, int>().ForMember(d => d, o => o.MapFrom(s => s.id));
        }
        public static void InitializeVacancy()
        {
            Mapper.CreateMap<VacancyCreated, Vacancy>()
                .ForMember(d => d.guid, o => o.MapFrom(s => s.VacancyGuid))
                .ForMember(d => d.name, o => o.MapFrom(s => s.Data.Name))
                .ForMember(d => d.fullname, o => o.MapFrom(s => s.Data.Name))
                //.ForMember(d => d.fullname, o => o.MapFrom(s => s.Data.FullName))
                .ForMember(d => d.tasks, o => o.MapFrom(s => s.Data.Tasks))
                .ForMember(d => d.researchtheme, o => o.MapFrom(s => s.Data.ResearchTheme))
                .ForMember(d => d.cityname, o => o.MapFrom(s => s.Data.CityName))
                .ForMember(d => d.details, o => o.MapFrom(s => s.Data.Details))
                .ForMember(d => d.bonuses, o => o.MapFrom(s => s.Data.Bonuses))
                .ForMember(d => d.contact_name, o => o.MapFrom(s => s.Data.ContactName))
                .ForMember(d => d.contact_email, o => o.MapFrom(s => s.Data.ContactEmail))
                .ForMember(d => d.contact_phone, o => o.MapFrom(s => s.Data.ContactPhone))
                .ForMember(d => d.contact_details, o => o.MapFrom(s => s.Data.ContactDetails))
                .ForMember(d => d.salary_from, o => o.MapFrom(s => s.Data.SalaryFrom))
                .ForMember(d => d.salary_to, o => o.MapFrom(s => s.Data.SalaryTo))
                .ForMember(d => d.contract_type, o => o.MapFrom(s => s.Data.ContractType))
                .ForMember(d => d.contract_time, o => o.MapFrom(s => s.Data.ContractTime))
                .ForMember(d => d.employment_type, o => o.MapFrom(s => s.Data.EmploymentType))
                .ForMember(d => d.operatingschedule_type, o => o.MapFrom(s => s.Data.OperatingScheduleType))
                .ForMember(d => d.socialpackage, o => o.MapFrom(s => s.Data.SocialPackage))
                .ForMember(d => d.rent, o => o.MapFrom(s => s.Data.Rent))
                .ForMember(d => d.officeaccomodation, o => o.MapFrom(s => s.Data.OfficeAccomodation))
                .ForMember(d => d.transportcompensation, o => o.MapFrom(s => s.Data.TransportCompensation))
                .ForMember(d => d.positiontype_id, o => o.MapFrom(s => s.Data.PositionTypeId))
                .ForMember(d => d.region_id, o => o.MapFrom(s => s.Data.RegionId))
                .ForMember(d => d.researchdirection_id, o => o.MapFrom(s => s.Data.ResearchDirectionId))
                .ForMember(d => d.criterias, o => o.MapFrom(s => s.Data.Criterias))
                .ForMember(d => d.attachments, o => o.MapFrom(s => s.Data.Attachments))
                .ForMember(d => d.organization_guid, o => o.MapFrom(s => s.OrganizationGuid))
                .ForMember(d => d.creation_date, o => o.MapFrom(s => s.TimeStamp));

            Mapper.CreateMap<VacancyUpdated, Vacancy>()
                .ForMember(d => d.guid, o => o.MapFrom(s => s.VacancyGuid))
                .ForMember(d => d.name, o => o.MapFrom(s => s.Data.Name))
                .ForMember(d => d.fullname, o => o.MapFrom(s => s.Data.Name))
                //.ForMember(d => d.fullname, o => o.MapFrom(s => s.Data.FullName))
                .ForMember(d => d.tasks, o => o.MapFrom(s => s.Data.Tasks))
                .ForMember(d => d.researchtheme, o => o.MapFrom(s => s.Data.ResearchTheme))
                .ForMember(d => d.cityname, o => o.MapFrom(s => s.Data.CityName))
                .ForMember(d => d.details, o => o.MapFrom(s => s.Data.Details))
                .ForMember(d => d.bonuses, o => o.MapFrom(s => s.Data.Bonuses))
                .ForMember(d => d.contact_name, o => o.MapFrom(s => s.Data.ContactName))
                .ForMember(d => d.contact_email, o => o.MapFrom(s => s.Data.ContactEmail))
                .ForMember(d => d.contact_phone, o => o.MapFrom(s => s.Data.ContactPhone))
                .ForMember(d => d.contact_details, o => o.MapFrom(s => s.Data.ContactDetails))
                .ForMember(d => d.salary_from, o => o.MapFrom(s => s.Data.SalaryFrom))
                .ForMember(d => d.salary_to, o => o.MapFrom(s => s.Data.SalaryTo))
                .ForMember(d => d.contract_type, o => o.MapFrom(s => s.Data.ContractType))
                .ForMember(d => d.contract_time, o => o.MapFrom(s => s.Data.ContractTime))
                .ForMember(d => d.employment_type, o => o.MapFrom(s => s.Data.EmploymentType))
                .ForMember(d => d.operatingschedule_type, o => o.MapFrom(s => s.Data.OperatingScheduleType))
                .ForMember(d => d.socialpackage, o => o.MapFrom(s => s.Data.SocialPackage))
                .ForMember(d => d.rent, o => o.MapFrom(s => s.Data.Rent))
                .ForMember(d => d.officeaccomodation, o => o.MapFrom(s => s.Data.OfficeAccomodation))
                .ForMember(d => d.transportcompensation, o => o.MapFrom(s => s.Data.TransportCompensation))
                .ForMember(d => d.positiontype_id, o => o.MapFrom(s => s.Data.PositionTypeId))
                .ForMember(d => d.region_id, o => o.MapFrom(s => s.Data.RegionId))
                .ForMember(d => d.researchdirection_id, o => o.MapFrom(s => s.Data.ResearchDirectionId))
                .ForMember(d => d.criterias, o => o.MapFrom(s => s.Data.Criterias))
                .ForMember(d => d.attachments, o => o.MapFrom(s => s.Data.Attachments))
                .ForMember(d => d.organization_guid, o => o.MapFrom(s => s.OrganizationGuid))
                .ForMember(d => d.update_date, o => o.MapFrom(s => s.TimeStamp));

            Mapper.CreateMap<Domain.Core.VacancyCriteria, ReadModel.Core.VacancyCriteria>()
                .ForMember(d => d.criteria_id, o => o.MapFrom(s => s.CriteriaId))
                .ForMember(d => d.count, o => o.MapFrom(s => s.Count));
            Mapper.CreateMap<CriteriaItemViewModel, Domain.Core.VacancyCriteria>()
                .ForMember(d => d.CriteriaId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CriteriaParentId, o => o.MapFrom(s => s.ParentId))
                .ForMember(d => d.Count, o => o.MapFrom(s => s.Count))
                .ForMember(d => d.CriteriaTitle, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.CriteriaCode, o => o.MapFrom(s => s.Code))
                ;
            Mapper.CreateMap<Domain.Core.VacancyAttachment, ReadModel.Core.VacancyAttachment>()
                .ForMember(d => d.name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.size, o => o.MapFrom(s => s.Size))
                .ForMember(d => d.extension, o => o.MapFrom(s => s.Extension))
                .ForMember(d => d.url, o => o.MapFrom(s => s.Url))
                .ForMember(d => d.type_id, o => o.MapFrom(s => s.TypeId))
                .ForMember(d => d.upload_date, o => o.MapFrom(s => s.UploadDate));

            //vacancy
            Mapper.CreateMap<Vacancy, VacancyCreateViewModel>()
                .ForMember(d => d.Guid, o => o.MapFrom(s => s.guid))
                .ForMember(d => d.ReadId, o => o.MapFrom(s => s.read_id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.name))
                //.ForMember(d => d.FullName, o => o.MapFrom(s => s.fullname))
                .ForMember(d => d.Tasks, o => o.MapFrom(s => s.tasks))
                .ForMember(d => d.ResearchTheme, o => o.MapFrom(s => s.researchtheme))
                .ForMember(d => d.CityName, o => o.MapFrom(s => s.cityname))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.details))
                .ForMember(d => d.ContactName, o => o.MapFrom(s => s.contact_name))
                .ForMember(d => d.ContactEmail, o => o.MapFrom(s => s.contact_email))
                .ForMember(d => d.ContactPhone, o => o.MapFrom(s => s.contact_phone))
                .ForMember(d => d.ContactDetails, o => o.MapFrom(s => s.contact_details))
                .ForMember(d => d.SalaryFrom, o => o.MapFrom(s => s.salary_from))
                .ForMember(d => d.SalaryTo, o => o.MapFrom(s => s.salary_to))
                .ForMember(d => d.Bonuses, o => o.MapFrom(s => s.bonuses))
                .ForMember(d => d.ContractType, o => o.MapFrom(s => s.contract_type))
                .ForMember(d => d.ContractTime, o => o.MapFrom(s => s.contract_time))
                .ForMember(d => d.EmploymentType, o => o.MapFrom(s => s.employment_type))
                .ForMember(d => d.OperatingScheduleType, o => o.MapFrom(s => s.operatingschedule_type))
                .ForMember(d => d.SocialPackage, o => o.MapFrom(s => s.socialpackage))
                .ForMember(d => d.Rent, o => o.MapFrom(s => s.rent))
                .ForMember(d => d.OfficeAccomodation, o => o.MapFrom(s => s.officeaccomodation))
                .ForMember(d => d.TransportCompensation, o => o.MapFrom(s => s.transportcompensation))
                .ForMember(d => d.PositionTypeId, o => o.MapFrom(s => s.positiontype_id))
                .ForMember(d => d.RegionId, o => o.MapFrom(s => s.region_id))
                .ForMember(d => d.ResearchDirectionId, o => o.MapFrom(s => s.researchdirection_id))
                .ForMember(d => d.Criterias, o => o.MapFrom(s => s.criterias))
                //.ForMember(d => d, o => o.MapFrom(s => s.attachments))
                .ForMember(d => d.OrganizationGuid, o => o.MapFrom(s => s.organization_guid))
                ;
            Mapper.CreateMap<Vacancy, VacancyDetailsViewModel>().IncludePagedResultMapping()
                .ForMember(d => d.Guid, o => o.MapFrom(s => s.guid))
                .ForMember(d => d.ReadId, o => o.MapFrom(s => s.read_id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.name))
                //.ForMember(d => d.FullName, o => o.MapFrom(s => s.fullname))
                .ForMember(d => d.Tasks, o => o.MapFrom(s => s.tasks))
                .ForMember(d => d.CancelReason, o => o.MapFrom(s => s.cancel_reason))
                .ForMember(d => d.CommitteeReasolution, o => o.MapFrom(s => s.committee_resolution))
                .ForMember(d => d.InCommitteeStartDate, o => o.MapFrom(s => s.committee_start_date))
                .ForMember(d => d.InCommitteeEndDate, o => o.MapFrom(s => s.committee_end_date))
                .ForMember(d => d.Attachments, o => o.MapFrom(s => s.attachments))
                .ForMember(d => d.ResearchTheme, o => o.MapFrom(s => s.researchtheme))
                .ForMember(d => d.CityName, o => o.MapFrom(s => s.cityname))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.details))
                .ForMember(d => d.ContactName, o => o.MapFrom(s => s.contact_name))
                .ForMember(d => d.ContactEmail, o => o.MapFrom(s => s.contact_email))
                .ForMember(d => d.ContactPhone, o => o.MapFrom(s => s.contact_phone))
                .ForMember(d => d.ContactDetails, o => o.MapFrom(s => s.contact_details))
                .ForMember(d => d.SalaryFrom, o => o.MapFrom(s => s.salary_from))
                .ForMember(d => d.SalaryTo, o => o.MapFrom(s => s.salary_to))
                .ForMember(d => d.Bonuses, o => o.MapFrom(s => s.bonuses))
                .ForMember(d => d.ContractType, o => o.MapFrom(s => s.contract_type))
                .ForMember(d => d.ContractTime, o => o.MapFrom(s => s.contract_time))
                .ForMember(d => d.EmploymentType, o => o.MapFrom(s => s.employment_type))
                .ForMember(d => d.OperatingScheduleType, o => o.MapFrom(s => s.operatingschedule_type))
                .ForMember(d => d.SocialPackage, o => o.MapFrom(s => s.socialpackage))
                .ForMember(d => d.Rent, o => o.MapFrom(s => s.rent))
                .ForMember(d => d.OfficeAccomodation, o => o.MapFrom(s => s.officeaccomodation))
                .ForMember(d => d.TransportCompensation, o => o.MapFrom(s => s.transportcompensation))
                .ForMember(d => d.PositionTypeId, o => o.MapFrom(s => s.positiontype_id))
                .ForMember(d => d.RegionId, o => o.MapFrom(s => s.region_id))
                .ForMember(d => d.ResearchDirectionId, o => o.MapFrom(s => s.researchdirection_id))
                //.ForMember(d => d., o => o.MapFrom(s => s.criterias))
                .ForMember(d => d.OrganizationGuid, o => o.MapFrom(s => s.organization_guid))
                .ForMember(d => d.IsWinnerAccept, o => o.MapFrom(s => s.is_winner_accept))
                .ForMember(d => d.WinnerResearcherGuid, o => o.MapFrom(s => s.winner_researcher_guid))
                .ForMember(d => d.WinnerRequestDate, o => o.MapFrom(s => s.winner_request_date))
                .ForMember(d => d.WinnerResponseDate, o => o.MapFrom(s => s.winner_response_date))
                .ForMember(d => d.WinnerVacancyApplicationGuid, o => o.MapFrom(s => s.winner_vacancyapplication_guid))
                .ForMember(d => d.IsPretenderAccept, o => o.MapFrom(s => s.is_pretender_accept))
                .ForMember(d => d.PretenderResearcherGuid, o => o.MapFrom(s => s.pretender_researcher_guid))
                .ForMember(d => d.PretenderRequestDate, o => o.MapFrom(s => s.pretender_request_date))
                .ForMember(d => d.PretenderResponseDate, o => o.MapFrom(s => s.pretender_response_date))
                .ForMember(d => d.PretenderVacancyApplicationGuid, o => o.MapFrom(s => s.pretender_vacancyapplication_guid))
                ;
            Mapper.CreateMap<VacancyCreateViewModel, VacancyDataModel>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name))
                //.ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
                .ForMember(d => d.Tasks, o => o.MapFrom(s => s.Tasks))
                .ForMember(d => d.ResearchTheme, o => o.MapFrom(s => s.ResearchTheme))
                .ForMember(d => d.CityName, o => o.MapFrom(s => s.CityName))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.Details))
                .ForMember(d => d.ContactName, o => o.MapFrom(s => s.ContactName))
                .ForMember(d => d.ContactEmail, o => o.MapFrom(s => s.ContactEmail))
                .ForMember(d => d.ContactPhone, o => o.MapFrom(s => s.ContactPhone))
                .ForMember(d => d.ContactDetails, o => o.MapFrom(s => s.ContactDetails))
                .ForMember(d => d.SalaryFrom, o => o.MapFrom(s => s.SalaryFrom))
                .ForMember(d => d.SalaryTo, o => o.MapFrom(s => s.SalaryTo))
                .ForMember(d => d.Bonuses, o => o.MapFrom(s => s.Bonuses))
                .ForMember(d => d.ContractType, o => o.MapFrom(s => s.ContractType))
                .ForMember(d => d.ContractTime, o => o.MapFrom(s => s.ContractTime))
                .ForMember(d => d.EmploymentType, o => o.MapFrom(s => s.EmploymentType))
                .ForMember(d => d.OperatingScheduleType, o => o.MapFrom(s => s.OperatingScheduleType))
                .ForMember(d => d.SocialPackage, o => o.MapFrom(s => s.SocialPackage))
                .ForMember(d => d.Rent, o => o.MapFrom(s => s.Rent))
                .ForMember(d => d.OfficeAccomodation, o => o.MapFrom(s => s.OfficeAccomodation))
                .ForMember(d => d.TransportCompensation, o => o.MapFrom(s => s.TransportCompensation))
                .ForMember(d => d.PositionType, o => o.MapFrom(s => s.PositionType))
                .ForMember(d => d.PositionTypeId, o => o.MapFrom(s => s.PositionTypeId))
                .ForMember(d => d.Region, o => o.MapFrom(s => s.Region))
                .ForMember(d => d.RegionId, o => o.MapFrom(s => s.RegionId))
                .ForMember(d => d.ResearchDirection, o => o.MapFrom(s => s.ResearchDirection))
                .ForMember(d => d.ResearchDirectionId, o => o.MapFrom(s => s.ResearchDirectionId))
                .ForMember(d => d.Criterias, o => o.MapFrom(s => s.CriteriasHierarchy.SelectMany(c => c.Items.Where(d => d.Count > 0))))
                //.ForMember(d => d.OrganizationFoiv, o => o.MapFrom(s => s))
                //.ForMember(d => d.OrganizationFoivId, o => o.MapFrom(s => s))
                ;
            Mapper.CreateMap<ReadModel.ElasticSearchModel.Model.Vacancy, VacancyElasticResult>().IncludePagedResultMapping();
            ;

            //TODO - VACANCY EDIT VIEW MODEL MAPPINGS
        }
    }
}
