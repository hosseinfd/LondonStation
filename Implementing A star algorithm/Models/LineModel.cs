using System.Collections.Generic;
using System.Linq;
using Implementing_A_star_algorithm.RetriveData;

namespace Implementing_A_star_algorithm.Models;

public class LineModel
{
    public string LineId { get;  set; }
    public string Name { get;  set; }
    public List<DisruptionOfLineModel> Disruption { get; set; }
    public LineModel(string lineId, string name)
    {
        LineId = lineId;
        Name = name;
    }

    public LineModel()
    {
        
    }

    public void AddDisruption(DisruptionOfLineModel disruptionOfLineModel)
    {
        Disruption ??= new List<DisruptionOfLineModel>();
        Disruption.Add(disruptionOfLineModel);
    }
    
    public void AddDisruption(List<TflDisruptionDto.Disruption> disruptionOfLineModel,string lineId)
    {
        Disruption ??= new List<DisruptionOfLineModel>();
        var disruption = disruptionOfLineModel
            .Where(q => q.id == lineId)
            .Select(q => new DisruptionOfLineModel(q.name, q.id,
                q.lineStatuses.Select(d =>
                    new LineStatusesModel
                    {
                        Reason = d.reason,
                        StatusSeverity = d.statusSeverity,
                        ValidityPeriods = d.validityPeriods.Select(v => new ValidityPeriodsModel
                        {
                            FromDate = v.fromDate,
                            IsNow = v.isNow,
                            ToDate = v.toDate
                        }).ToList(),
                        StatusSeverityDescription = d.statusSeverityDescription
                    }).ToList())).ToList()
            .FirstOrDefault();
        
        Disruption.Add(disruption);
    }
}
