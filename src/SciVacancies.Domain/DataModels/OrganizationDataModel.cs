﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SciVacancies.Domain.DataModels
{
    public class OrganizationDataModel
    {
        /// <summary>
        /// Идентификатор аккаунта, к которому привязан агрегат
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Полное наименование
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// Полное наименование (на английском языке)
        /// </summary>
        public string NameEng { get; set; }

        /// <summary>
        /// Сокращенное наименование
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Сокращенное наименование (на английском языке)
        /// </summary>
        public string ShortNameEng { get; set; }

        /// <summary>
        /// Населенный пункт
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Почтовый адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Веб-сайт
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string INN { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        public string OGRN { get; set; }

        /// <summary>
        /// Организационно-правовая форма организации
        /// </summary>
        public string OrgForm { get; set; }
        public int OrgFormId { get; set; }
        public Guid OrgFormGuid { get; set; }

        /// <summary>
        /// Количество опубликованных вакансий на данный момент
        /// </summary>
        public int PublishedVacancies { get; set; }

        /// <summary>
        /// ФОИВ
        /// </summary>
        public string Foiv { get; set; }
        public int FoivId { get; set; }
        public Guid FoivGuid { get; set; }


        /// <summary>
        /// Основной вид деятельности
        /// </summary>
        public string Activity { get; set; }
        public int ActivityId { get; set; }
        public Guid ActivityGuid { get; set; }

        /// <summary>
        /// Отрасли науки
        /// </summary>
        public List<int> ResearchDirections { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public string HeadPatronymic { get; set; } //HeadMiddleName
    }
}
