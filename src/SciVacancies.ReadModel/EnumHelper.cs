﻿using System;
using System.ComponentModel;
using System.Security.Claims;
using SciVacancies.Domain.Enums;

namespace SciVacancies
{
    public static class EnumExtenions
    {

        /// <summary>
        /// Получить из атрибута Description описание значения (Возвращает точное описание статуса, без обобщений и преобразований)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name == null) return null;

            var field = type.GetField(name);

            if (field == null) return null;

            var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attr?.Description;
        }


        /// <summary>
        /// Получить из атрибута Description описание значения
        /// </summary>
        /// <param name="value"></param>
        /// <param name="user">пользователь для определения роли</param>
        /// <returns></returns>
        public static string GetDescription(this Enum value, ClaimsPrincipal user)
        {
            if (user.IsInRole(ConstTerms.RequireRoleResearcher))
            {
                if (value is VacancyStatus)
                    return ((VacancyStatus)value).GetDescriptionByResearcher();
            }

            return value.GetDescription();
        }

        /// <summary>
        /// Ограниченное описание статуса Вакансии (некоторые статусы объеденены для упрощения восприятия пользователем)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescriptionByResearcher(this VacancyStatus value)
        {
            switch (value)
            {
                case VacancyStatus.Published:
                    return VacancyStatus.Published.GetDescription();

                case VacancyStatus.InCommittee:
                    return VacancyStatus.InCommittee.GetDescription();

                case VacancyStatus.OfferResponseAwaitingFromWinner:
                case VacancyStatus.OfferResponseAwaitingFromPretender:
                case VacancyStatus.OfferAcceptedByWinner:
                case VacancyStatus.OfferAcceptedByPretender:
                case VacancyStatus.OfferRejectedByWinner:
                case VacancyStatus.OfferRejectedByPretender:
                    return "Финал";
                case VacancyStatus.Cancelled:
                case VacancyStatus.Closed:
                    return "Закрыта";
                default:
                    return "статус не определен";
            }
        }

        /// <summary>
        /// Базовые рекомендации на основе статуса вакансии (рекомендуется использовать для Организаций)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescriptionRecommendation(this VacancyStatus value)
        {
            switch (value)
            {
                case VacancyStatus.InCommittee:
                    return "Ожидается выбор Победителя (и Претендента), ввод решения комиссии";
                case VacancyStatus.OfferResponseAwaitingFromWinner:
                    return "Ожидается ответ от Победителя";
                case VacancyStatus.OfferResponseAwaitingFromPretender:
                    return "Ожидается ответ от Претендента";
                case VacancyStatus.OfferAcceptedByWinner:
                case VacancyStatus.OfferAcceptedByPretender:
                    return "Ождидается закрытие вакансии";
                case VacancyStatus.OfferRejectedByWinner:
                    return "Ожидается подача предложения претенденту (если он указан) либо ввод решения комиссии";
                default:
                    return null;
            }
        }
    }
}
