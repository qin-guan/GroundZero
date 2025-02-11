namespace GroundZero.Api.Endpoints.Hackathon.Judge.Vote;

public class VoteRequest
{
    public Guid Id { get; set; }
    
    public string Action { get; set; }
}