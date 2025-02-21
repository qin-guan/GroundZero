namespace GroundZero.Api.Endpoints.Hackathon.Judge.Vote;

public class PostVoteRequest
{
    public Guid Id { get; set; }

    public string Action { get; set; }
}