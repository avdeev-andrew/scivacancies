﻿using SciVacancies.ReadModel.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPoco;

namespace SciVacancies.ReadModel
{
    public interface IReadModelService
    {
        Researcher SingleResearcher(Guid researcherGuid);
        List<Researcher> SelectResearchers();

        VacancyApplication SingleVacancyApplication(Guid vacancyApplicationGuid);
        List<VacancyApplication> SelectVacancyApplicationsByResearcher(Guid researcherGuid);
        List<VacancyApplication> SelectVacancyApplicationsByVacancy(Guid vacancyGuid);

        SearchSubscription SingleSearchSubscription(Guid searchSubscriptionGuid);
        List<SearchSubscription> SelectSearchSubscriptions();
        List<SearchSubscription> SelectSearchSubscriptions(Guid researcherGuid);

        Attachment SingleAttachment(Guid attachmentGuid);
        List<Attachment> SelectAttachments(Guid vacancyApplicationGuid);

        Vacancy SingleVacancy(Guid vacancyGuid);
        List<Vacancy> SelectVacancies(Guid organizationGuid);
        List<Vacancy> SelectFavoriteVacancies(Guid researcherGuid);

        Notification SingleNotification(Guid notificationGuid);
        List<Notification> SelectNotificationsByResearcher(Guid researcherGuid);
        List<Notification> SelectNotificationsByOrganization(Guid organizationGuid);
        int CountNotificationsByResearcher(Guid researcherGuid);
        int CountNotificationsByOrganization(Guid organizationGuid);

        Organization SingleOrganization(Guid organizationGuid);
        List<Organization> SelectOrganizations();
        /// <summary>
        /// получить список организаций по условия
        /// </summary>
        /// <param name="orderBy">поле для сортировки</param>
        /// <param name="pageSize">размер страницы</param>
        /// <param name="pageIndex">Номер страницы (начиная с 1)</param>
        /// <param name="nameFilterValue">Значение фильтра для свойства Название организации (не обязательно)</param>
        /// <param name="addressFilterValue">Значение фильтра для свойства Адрес организации (не обязательно)</param>
        /// <returns></returns>
        Page<Organization> SelectOrganizations(string orderBy, long pageSize, long pageIndex, string nameFilterValue = null, string addressFilterValue = null);

        Position SinglePosition(Guid positionGuid);
        List<Position> SelectPositions(Guid organizationGuid);
    }
}