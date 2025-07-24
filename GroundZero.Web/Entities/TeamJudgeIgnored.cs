using SqlSugar;

namespace GroundZero.Web.Entities;

public class TeamJudgeIgnored
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid TeamId { get; set; }
    
    [SugarColumn(IsPrimaryKey = true)]
    public Guid JudgeId { get; set; }
}
