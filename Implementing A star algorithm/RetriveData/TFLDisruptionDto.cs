using System;
using System.Collections.Generic;

namespace Implementing_A_star_algorithm.RetriveData;

public class TflDisruptionDto
{
    public TflDisruptionDto()
    {
    }

    public class Disruption
    {
        public string name { get; set; }
        public string id { get; set; }
        public List<LineStatuses> lineStatuses { get; set; } = new();
    }

    public class LineStatuses
    {
        public int? statusSeverity { get; set; }
        public string statusSeverityDescription { get; set; }
        public string reason { get; set; }
        public List<ValidityPeriods> validityPeriods { get; set; } = new();
    }

    public class ValidityPeriods
    {
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public bool? isNow { get; set; }
    }
}