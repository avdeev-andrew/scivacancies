﻿using SciVacancies.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NPoco;

namespace SciVacancies.ReadModel.Core
{
    [TableName("SearchSubscriptions")]
    [PrimaryKey("Guid", AutoIncrement = false)]
    public class SearchSubscription : BaseEntity
    {

    }
}
