using SqlSugar;

namespace GroundZero.Web.Entities;

public class TeamJudgeViewed
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid TeamId { get; set; }
    
    [SugarColumn(IsPrimaryKey = true)]
    public Guid JudgeId { get; set; }
}
