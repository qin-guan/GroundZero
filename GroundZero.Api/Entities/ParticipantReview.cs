namespace GroundZero.Api.Entities;

public class ParticipantReview
{
    public Guid Id { get; set; }

    public ParticipantReviewStatus Status { get; set; }
    public string Reason { get; set; }

    public Guid ParticipantId { get; set; }
    public Participant Participant { get; set; }
}