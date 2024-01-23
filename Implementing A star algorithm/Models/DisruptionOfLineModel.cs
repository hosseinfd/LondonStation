using System;
using System.Collections.Generic;

namespace Implementing_A_star_algorithm.Models;

public class DisruptionOfLineModel
{
    public string Name { get; set; }
    public string Id { get; set; }
    public List<LineStatusesModel> LineStatuses { get; set; } = new();

    public DisruptionOfLineModel(string name, string id,List<LineStatusesModel> lineStatuses)
    {
        LineStatuses = lineStatuses;
        Name = name;
        Id = id;
    }
}

public class LineStatusesModel
{
    public int? StatusSeverity { get; set; }
    public string StatusSeverityDescription { get; set; }
    public string Reason { get; set; }
    public List<ValidityPeriodsModel> ValidityPeriods { get; set; } = new();
}

public class ValidityPeriodsModel
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsNow { get; set; }
}