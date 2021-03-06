﻿using System.Collections.Generic;
using SciVacancies.ReadModel.Pager;

namespace SciVacancies.WebApp.ViewModels
{

    public class VacanciesFilterModel
    {
        public bool IsSubscription { get; set; }

        public PagedList<VacancyElasticResult> Items { get; set; }
        public IEnumerable<int> Regions { get; set; }
        public IEnumerable<int> Foivs { get; set; }
        public IEnumerable<int> ResearchDirections { get; set; }
        public IEnumerable<int> PositionTypes { get; set; }
        public IEnumerable<int> VacancyStates { get; set; }

        public int Period { get; set; }
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;

        public int? SalaryMin { get; set; }
        public int? SalaryMax { get; set; }

        private string _orderBy;
        private string _search;

        public string OrderBy
        {
            get
            {
                _orderBy = string.IsNullOrWhiteSpace(_orderBy) ? ConstTerms.OrderByFieldDate : _orderBy.ToLower();
                return _orderBy;
            }
            set { _orderBy = value; }
        }

        public string Search
        {
            get { return _search; }
            set
            {
                _search = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        /// <summary>
        /// Сохранить текущий запрос как Поисковую подписку
        /// </summary>
        public bool NewSubscriptionAdd { get; set; }
        /// <summary>
        /// Заголовок для новой Поисковой подписки
        /// </summary>
        public string NewSubscriptionTitle { get; set; }
        /// <summary>
        /// информация о статусе новой подписки
        /// </summary>
        public SubscriptionInfoViewModel SubscriptionInfo { get; set; }

        /// <summary>
        /// получать уведомления о новых вакансиях по данной подписке
        /// </summary>
        public bool NewSubscriptionNotifyByEmail { get; set; } = true;

    }
}
