﻿using Nest;

namespace SciVacancies.Services.Elastic
{
    public class VacancyPaymentsAnalythicQuery
    {
        public int? RegionId { get; set; }
        public DateInterval Interval { get; set; }
        public int BarsNumber { get; set; }
    }


    public class VacancyPaymentsByResearchDirectionAnalythicQuery
    {
        public int ResearchDirectionId { get; set; }
        //public int? RegionId { get; set; }
        //public DateInterval Interval { get; set; }
        //public int BarsNumber { get; set; }
    }
}
