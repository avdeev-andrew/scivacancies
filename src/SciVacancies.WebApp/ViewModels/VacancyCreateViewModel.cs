﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using MediatR;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using SciVacancies.Domain.Enums;
using SciVacancies.ReadModel.Core;
using SciVacancies.WebApp.Engine;
using SciVacancies.WebApp.Queries;
using SciVacancies.WebApp.ViewModels.Base;

namespace SciVacancies.WebApp.ViewModels
{
    public class VacancyCreateViewModel : PageViewModelBase
    {
        public VacancyCreateViewModel() { }

        public VacancyCreateViewModel(Guid organizationGuid)
        {
            if (organizationGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(organizationGuid));
            OrganizationGuid = organizationGuid;
        }

        [HiddenInput]
        public Guid OrganizationGuid { get; set; }
        /// <summary>
        /// Сохранить как черновик (true) или сохранить и опубликовать данные (false)
        /// </summary>
        [HiddenInput]
        public bool ToPublish { get; set; }

        public VacancyStatus Status { get; set; }

        /// <summary>
        /// Инициализация справочников для формы
        /// </summary>
        /// <returns></returns>
        public void InitDictionaries(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (OrganizationGuid == Guid.Empty)
                throw new NullReferenceException($"Property {nameof(OrganizationGuid)} has Empty value");


            PositionTypes = mediator.Send(new SelectAllPositionTypesQuery()).Select(c => new SelectListItem { Text = c.title, Value = c.id.ToString() });
            ResearchDirections = mediator.Send(new SelectAllResearchDirectionsQuery()).Select(c => new SelectListItem { Text = c.title, Value = c.id.ToString() });
            Regions = mediator.Send(new SelectAllRegionsQuery()).Select(c => new SelectListItem { Text = c.title, Value = c.id.ToString() });

            if (PositionTypeId.HasValue && PositionTypeId != 0)
                PositionType = PositionTypes.Single(c => c.Value == PositionTypeId.Value.ToString()).Text;
            if (ResearchDirectionId.HasValue && ResearchDirectionId.Value != 0)
                ResearchDirection = ResearchDirections.Single(c => c.Value == ResearchDirectionId.Value.ToString()).Text;
            if (RegionId.HasValue && RegionId != 0)
                Region = Regions.Single(c => c.Value == RegionId.Value.ToString()).Text;

            ContractTypes = new List<ContractType> { ContractType.Permanent, ContractType.FixedTerm }
                .Select(c => new SelectListItem { Value = ((int)c).ToString(), Text = c.GetDescription() });


            EmploymentTypes = new List<EmploymentType>
            {
               EmploymentType.Full,
               EmploymentType.Partial,
               EmploymentType.Probation,
               EmploymentType.Temporary,
               EmploymentType.Volunteering
            }
                .Select(c => new SelectListItem { Value = ((int)c).ToString(), Text = c.GetDescription() });

            OperatingScheduleTypes = new List<OperatingScheduleType>
            {
                OperatingScheduleType.Flexible,
                OperatingScheduleType.FullTime,
                OperatingScheduleType.Remote,
                OperatingScheduleType.Replacement,
                OperatingScheduleType.Rotation
            }
                .Select(c => new SelectListItem { Value = ((int)c).ToString(), Text = c.GetDescription() });

            //Заполнение иерархии
            if (CriteriasHierarchy.Count == 0)
            {
                CriteriasHierarchy =
                    mediator.Send(new SelectAllCriteriasQuery()).ToList().ToHierarchyCriteriaViewModelList(Criterias);
            }

        }

        /// <summary>
        /// Guid должности из справочника
        /// </summary>
        //[Required(ErrorMessage = "Требуется выбрать Должность", AllowEmptyStrings = false)]
        //[Range(1, int.MaxValue, ErrorMessage = "Требуется выбрать Должность")]
        public int? PositionTypeId { get; set; }
        public string PositionType { get; set; }
        public IEnumerable<SelectListItem> PositionTypes { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        [Required(ErrorMessage = "Требуется заполнить поле Наименование")]
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string Name { get; set; }

        /// <summary>
        /// Отрасль науки
        /// </summary>
        public string ResearchDirection { get; set; }
        //[Required(ErrorMessage = "Требуется выбрать Отрасль науки")]
        //[Range(1, int.MaxValue, ErrorMessage = "Требуется выбрать Отрасль науки")]
        public int? ResearchDirectionId { get; set; }
        public IEnumerable<SelectListItem> ResearchDirections { get; set; }

        /// <summary>
        /// Тематика исследований
        /// </summary>
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string ResearchTheme { get; set; }

        /// <summary>
        /// Задачи
        /// </summary>
        //[Required(ErrorMessage = "Требуется описать Задачи")]
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string Tasks { get; set; }

        /// <summary>
        /// Зарплата в месяц
        /// </summary>

        //[Required(ErrorMessage = "Укажите минимальную зарплату")]
        [Range(0, int.MaxValue, ErrorMessage= "Зарплата не может быть ниже установленного значения")]
        public int? SalaryFrom { get; set; }

        //[Required(ErrorMessage = "Укажите максимальную зарплату")]
        [Range(0, int.MaxValue, ErrorMessage = "Зарплата не может быть ниже установленного значения")]
        public int? SalaryTo { get; set; }

        /// <summary>
        /// Стимулирующие выплаты
        /// </summary>
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string Bonuses { get; set; }

        /// <summary>
        /// Дополнительно
        /// </summary>
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string Details { get; set; }

        public int ContractTypeValue { get; set; }
        /// <summary>
        /// Тип трудового договора
        /// </summary>
        public ContractType ContractType
        {
            get { return (ContractType)ContractTypeValue; }
            set { ContractTypeValue = (int)value; }
        }
        public IEnumerable<SelectListItem> ContractTypes { get; set; }

        /// <summary>
        /// Срок трудового договора (для срочного договора)
        /// </summary>
        public decimal? ContractTime { get; set; }

        /// <summary>
        /// Срок трудового договора (для срочного договора)
        /// </summary>
        [Range(minimum: 0, maximum: 50, ErrorMessage = "Вы ввели недопустимое значение. Допустимо от 0 до 50 лет.")]
        public int? ContractTimeYears
        {
            get { return int.Parse((ContractTime ?? 0).ToString(CultureInfo.InvariantCulture).Split('.')[0]); }
            set { ContractTime = (value??0) + ((ContractTime ?? 0) - Math.Truncate(ContractTime ?? 0)); }
        }
        /// <summary>
        /// Срок трудового договора (для срочного договора)
        /// </summary>
        [Range(minimum: 0, maximum: 11, ErrorMessage = "Вы ввели недопустимое значение. Допустимо от 0 до 11 месяцев.")]
        public int? ContractTimeMonths
        {
            get
            {
                var splits = (ContractTime ?? 0).ToString(CultureInfo.InvariantCulture).Split('.');
                return splits.Length == 1 ? 0 : int.Parse(splits[1]);
            }
            set { ContractTime = decimal.Parse($"{ Math.Truncate(ContractTime ?? 0)}.{value}", CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Социальный пакет
        /// </summary>
        public bool SocialPackage { get; set; }

        /// <summary>
        /// Найм жилья
        /// </summary>
        public bool Rent { get; set; }

        /// <summary>
        /// Служебное жильё
        /// </summary>
        public bool OfficeAccomodation { get; set; }

        /// <summary>
        /// Компенсация транспорта
        /// </summary>
        public bool TransportCompensation { get; set; }

        /// <summary>
        /// Лицо для получения дополнительных справок
        /// </summary>
        //[Required(ErrorMessage = "Укажите контактное лицо")]
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string ContactName { get; set; }
        //[Required(ErrorMessage = "Укажите E-mail")]
        [EmailAddress(ErrorMessage = "Поле E-mail содержит не допустимый адрес электронной почты.")]
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string ContactEmail { get; set; }
        //[Required(ErrorMessage = "Укажите номер телефона")]
        [Phone(ErrorMessage = "Поле Телефон содержит не допустимый номер телефона.")]
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string ContactPhone { get; set; }
        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string ContactDetails { get; set; }


        //[Required(ErrorMessage = "Требуется выбрать тип занятости")]
        public EmploymentType EmploymentType { get; set; }
        public IEnumerable<SelectListItem> EmploymentTypes { get; set; }

        //[Required(ErrorMessage = "Требуется выбрать режим работы")]
        public OperatingScheduleType OperatingScheduleType { get; set; }
        public IEnumerable<SelectListItem> OperatingScheduleTypes { get; set; }

        //[Required(ErrorMessage = "Требуется выбрать регион")]
        //[Range(1, int.MaxValue, ErrorMessage = "Требуется выбрать регион")]
        public int? RegionId { get; set; }
        public string Region { get; set; }
        public IEnumerable<SelectListItem> Regions { get; set; }

        [MaxLength(1500, ErrorMessage = "Длина строки не более 1500 символов")]
        public string CityName { get; set; }

        /// <summary>
        /// Критерии
        /// </summary>
        public List<CriteriaItemViewModel> CriteriasHierarchy { get; set; } = new List<CriteriaItemViewModel>();
        public List<VacancyCriteria> Criterias { get; set; }
        /// <summary>
        /// Критерии, которые организация добавляет по своему усмотрению
        /// </summary>
        public List<CustomCriteriaViewModel> CustomCriterias { get; set; } = new List<CustomCriteriaViewModel>();

        /// <summary>
        /// Autoincrimented field in DB - может быть null
        /// </summary>
        public long? ReadId { get; set; }

    }
}