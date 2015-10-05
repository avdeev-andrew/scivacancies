﻿using SciVacancies.Domain.Enums;
using SciVacancies.ReadModel.ElasticSearchModel.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using Nest;
using MediatR;
using NPoco;
using Microsoft.Framework.OptionsModel;

namespace SciVacancies.WebApp.Queries
{
    public class SearchQueriesHandler : IRequestHandler<SearchQuery, Page<Vacancy>>
    {
        private readonly IElasticClient _elastic;
        readonly IOptions<ElasticSettings> settings;

        public SearchQueriesHandler(IElasticClient elastic, IOptions<ElasticSettings> settings)
        {
            _elastic = elastic;
            this.settings = settings;
        }

        public Page<Vacancy> Handle(SearchQuery msg)
        {
            Func<SearchQuery, SearchDescriptor<Vacancy>> searchSelector = VacancySearchDescriptor;

            var results = _elastic.Search<Vacancy>(searchSelector(msg));

            var pageVacancies = new Page<Vacancy>
            {
                CurrentPage = msg.CurrentPage.Value,
                ItemsPerPage = msg.PageSize.Value,
                TotalItems = results.Total,
                TotalPages = msg.PageSize.HasValue ? results.Total / msg.PageSize.Value : 0,
                Items = results.Documents.ToList()
            };

            return pageVacancies;
        }

        #region QueryContainers

        public SearchDescriptor<Vacancy> VacancySearchDescriptor(SearchQuery sq)
        {
            SearchDescriptor<Vacancy> sdescriptor = new SearchDescriptor<Vacancy>();

            if (sq.PageSize.HasValue && sq.CurrentPage.HasValue &&
                sq.PageSize.Value != 0 && sq.CurrentPage.Value != 0)
            {
                sdescriptor.Skip((int)((sq.CurrentPage - 1) * sq.PageSize));
                sdescriptor.Take((int)sq.PageSize);
            }

            sdescriptor.MinScore(settings.Options.MinScore);

            switch (sq.OrderFieldByDirection)
            {
                case ConstTerms.SearchFilterOrderByDateDescending:
                    sdescriptor.Sort(sort => sort.OnField(of => of.PublishDate).Descending());
                    break;
                case ConstTerms.SearchFilterOrderByDateAscending:
                    sdescriptor.Sort(sort => sort.OnField(of => of.PublishDate).Ascending());
                    break;
            }
            Func<SearchQuery, QueryContainer> querySelector = VacancyQueryContainer;

            sdescriptor.Query(querySelector(sq));

            return sdescriptor;
        }
        public QueryContainer VacancyQueryContainer(SearchQuery sq)
        {
            Func<QueryDescriptor<Vacancy>, SearchQuery, QueryContainer> querySelector = VacancyFilteredQueryContainer;
            Func<FilterDescriptor<Vacancy>, SearchQuery, FilterContainer> filterSelector = VacancyFilteredFilterContainer;

            return Query<Vacancy>.Filtered(flt => flt
                                    .Query(flq => querySelector(flq, sq))
                                    .Filter(flf => filterSelector(flf, sq))
                                );
        }
        public QueryContainer VacancyFilteredQueryContainer(QueryDescriptor<Vacancy> queryDescriptor, SearchQuery sq)
        {
            QueryContainer queryContainer;

            if (String.IsNullOrEmpty(sq.Query))
            {
                queryContainer = queryDescriptor.MatchAll();
            }
            else
            {
                queryContainer = queryDescriptor.MultiMatch(m => m
                                                 .Query(sq.Query)
                                                 .OnFieldsWithBoost(fb => fb
                                                     .Add(f => f.FullName, 10.0)
                                                     .Add(f => f.Name, 10.0)
                                                     .Add(f => f.Tasks, 7.0)
                                                     .Add(f => f.Criterias.FirstOrDefault().CriteriaTitle, 7.0)
                                                     .Add(f => f.ResearchTheme, 2.0)
                                                     .Add(f => f.CityName, 2.0)
                                                     .Add(f => f.Details, 2.0)
                                                     .Add(f => f.Bonuses, 2.0)
                                                 )
                                            );
            }

            return queryContainer;
        }
        public FilterContainer VacancyFilteredFilterContainer(FilterDescriptor<Vacancy> filterDescriptor, SearchQuery sq)
        {
            FilterContainer filterContainer;

            List<FilterContainer> filters = new List<FilterContainer>();

            if (sq.FoivIds != null && sq.FoivIds.Count() > 0)
            {
                filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                                .Must(m => m
                                                                    .Terms(t => t.OrganizationFoivId, sq.FoivIds)
                                                                )
                                                            )
                                                        );
            }
            if (sq.PositionTypeIds != null && sq.PositionTypeIds.Count() > 0)
            {
                filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                                .Must(m => m
                                                                    .Terms(t => t.PositionTypeId, sq.PositionTypeIds)
                                                                )
                                                            )
                                                        );
            }
            if (sq.RegionIds != null && sq.RegionIds.Count() > 0)
            {
                filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                                .Must(m => m
                                                                    .Terms(t => t.RegionId, sq.RegionIds)
                                                                )
                                                            )
                                                        );
            }
            if (sq.ResearchDirectionIds != null && sq.ResearchDirectionIds.Count() > 0)
            {
                filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                                .Must(m => m
                                                                    .Terms(t => t.ResearchDirectionId, sq.ResearchDirectionIds)
                                                                )
                                                            )
                                                        );
            }
            if (sq.SalaryFrom.HasValue && sq.SalaryFrom > 0)
            {
                filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                                .Must(m => m
                                                                    .Range(r => r
                                                                        .GreaterOrEquals(sq.SalaryFrom)
                                                                        .OnField(of => of.SalaryFrom)
                                                                    )
                                                                )
                                                            )
                                                        );
            }
            if (sq.SalaryTo.HasValue && sq.SalaryTo > 0)
            {
                filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                                .Must(m => m
                                                                    .Range(r => r
                                                                        .LowerOrEquals(sq.SalaryTo)
                                                                        .OnField(of => of.SalaryTo)
                                                                    )
                                                                )
                                                            )
                                                        );
            }
            if (sq.VacancyStatuses != null && sq.VacancyStatuses.Count() > 0)
            {
                filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                                .Must(m => m
                                                                    .Terms(t => t.Status, sq.VacancyStatuses)
                                                                )
                                                            )
                                                        );
            }
            if (sq.PublishDateFrom.HasValue)
            {
                filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                                .Must(m => m
                                                                    .Range(r => r
                                                                        .GreaterOrEquals(sq.PublishDateFrom)
                                                                        .OnField(of => of.PublishDate)
                                                                    )
                                                                )
                                                            )
                                                        );
            }
            filters.Add(new FilterDescriptor<Vacancy>().Bool(b => b
                                                            .MustNot(mn => mn
                                                                .Terms(t => t.Status, new List<VacancyStatus> {
                                                                    VacancyStatus.InProcess
                                                                })
                                                            )
                                                        )
                                                    );

            filterContainer = filterDescriptor.Bool(b => b.Must(filters.ToArray()));

            return filterContainer;
        }

        #endregion
    }
}