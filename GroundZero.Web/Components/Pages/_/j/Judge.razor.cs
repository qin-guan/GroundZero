using System.Collections.Immutable;
using GroundZero.Gavel;
using GroundZero.Web.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Logging;
using MoreLinq;
using SqlSugar;

namespace GroundZero.Web.Components.Pages._.j;

public partial class Judge : ComponentBase, IDisposable
{
    [Inject]
    public required ISqlSugarClient Db { get; init; }

    [Inject]
    public required PersistentComponentState ApplicationState { get; init; }

    [Inject]
    public required NavigationManager Navigation { get; init; }

    [Parameter]
    public required Guid Secret { get; init; }

    private bool SubmissionIsPending { get; set; }

    private Entities.Judge JudgeJudge { get; set; } = null!;

    private Hackathon? Hackathon { get; set; }

    private bool _isPending = true;
    private bool _hasPrerenderedData;
    private PersistingComponentStateSubscription _persistingSubscription;

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationState.TryTakeFromJson<bool>(nameof(_hasPrerenderedData), out var d) && d)
        {
            ApplicationState.TryTakeFromJson<Entities.Hackathon>(nameof(Hackathon), out var hackathon);
            ApplicationState.TryTakeFromJson<Entities.Judge>(nameof(JudgeJudge), out var judgeJudge);
            Hackathon = hackathon;
            JudgeJudge = judgeJudge!;
        }
        else
        {
            await FetchData();
            _hasPrerenderedData = true;
        }

        await MaybeInitAnnotator();

        _isPending = false;
        _persistingSubscription = ApplicationState.RegisterOnPersisting(PersistStateAsync);
    }

    private Task PersistStateAsync()
    {
        ApplicationState.PersistAsJson(nameof(JudgeJudge), JudgeJudge);
        ApplicationState.PersistAsJson(nameof(Hackathon), Hackathon);
        ApplicationState.PersistAsJson(nameof(_hasPrerenderedData), _hasPrerenderedData);
        return Task.CompletedTask;
    }

    private async Task FetchData()
    {
        var entity = await Db.Queryable<Entities.Judge>()
            .Includes(j => j.NextTeam!, t => t.JudgesViewed)
            .Includes(j => j.PreviousTeam)
            .Includes(j => j.IgnoredTeams)
            .Includes(j => j.ViewedTeams)
            .LeftJoin<Hackathon>((j, h) => j.HackathonId == h.Id)
            .Where((j) => j.Secret == Secret)
            .Select((j, h) => new
                { Judge = j, Hackathon = h, j.NextTeam, j.PreviousTeam, j.IgnoredTeams, j.ViewedTeams })
            .SingleAsync();

        JudgeJudge = entity.Judge;
        JudgeJudge.NextTeam = entity.NextTeam;
        JudgeJudge.PreviousTeam = entity.PreviousTeam;
        JudgeJudge.IgnoredTeams = entity.IgnoredTeams;
        JudgeJudge.ViewedTeams = entity.ViewedTeams;
        Hackathon = entity.Hackathon;
    }

    private async Task MaybeInitAnnotator()
    {
        if (JudgeJudge.NextTeamId is not null)
            return;

        var next = await ChooseNext();
        if (next is null)
            return;

        JudgeJudge.NextTeamId = next.Id;
        JudgeJudge.NextTeam = next;

        await Db.Updateable(JudgeJudge).ExecuteCommandAsync();
    }

    private async Task<IList<Team>> GetAvailableTeams()
    {
        var ignoredTeamIds = JudgeJudge.IgnoredTeams.Select(t => t.Id).ToList();

        var query = Db.Queryable<Team>()
            .Includes(t => t.JudgesViewed)
            .Where(t => t.Active && t.HackathonId == JudgeJudge.HackathonId);

        if (ignoredTeamIds.Count > 0)
        {
            query = query.Where(t => !ignoredTeamIds.Contains(t.Id));
        }

        return await query.ToListAsync();
    }

    private async Task<IList<Team>> PreferredItems()
    {
        var availableItems = await GetAvailableTeams();

        var prioritizedTeams = availableItems.Where(t => t.Prioritized).ToImmutableList();
        var teams = prioritizedTeams.Count > 0 ? prioritizedTeams : availableItems;

        var busyTeamIds = await Db.Queryable<Entities.Judge>()
            .Where(j =>
                j.Active &&
                j.HackathonId == JudgeJudge.HackathonId &&
                j.NextTeamId != null &&
                j.UpdatedAt != null &&
                DateTimeOffset.Now - j.UpdatedAt <= TimeSpan.FromMinutes(1)
            )
            .Select(j => j.NextTeamId)
            .ToListAsync();

        var nonBusyTeams = teams.Where(t => !busyTeamIds.Contains(t.Id)).ToImmutableArray();
        var preferred = nonBusyTeams.Length > 0 ? nonBusyTeams : teams;

        var lessSeenTeams = preferred.Where(t => t.JudgesViewed.Count < 3).ToImmutableArray();
        return lessSeenTeams.Length > 0 ? lessSeenTeams : preferred;
    }

    private async Task<Team?> ChooseNext()
    {
        var teams = (await PreferredItems()).Shuffle().ToImmutableArray();
        if (teams.Length == 0)
            return null;

        if (JudgeJudge.PreviousTeamId is null || Random.Shared.NextDouble() < CrowdBt.Epsilon)
            return teams.First();

        return CrowdBt.Argmax(
            i => CrowdBt.ExpectedInformationGain(
                JudgeJudge.Alpha,
                JudgeJudge.Beta,
                JudgeJudge.PreviousTeam!.Mu,
                JudgeJudge.PreviousTeam!.SigmaSq,
                i.Mu,
                i.SigmaSq
            ),
            teams
        );
    }

    private async Task BeginContinue()
    {
        if (JudgeJudge.NextTeamId == null)
            throw new ArgumentNullException(nameof(JudgeJudge.NextTeamId));

        SubmissionIsPending = true;

        await Db.Insertable(new TeamJudgeIgnored
        {
            TeamId = JudgeJudge.NextTeamId.Value,
            JudgeId = JudgeJudge.Id
        }).ExecuteCommandAsync();

        await Db.Insertable(new TeamJudgeViewed
        {
            TeamId = JudgeJudge.NextTeamId.Value,
            JudgeId = JudgeJudge.Id
        }).ExecuteCommandAsync();

        JudgeJudge.PreviousTeam = JudgeJudge.NextTeam;
        JudgeJudge.PreviousTeamId = JudgeJudge.NextTeamId;

        await Db.Updateable(JudgeJudge).ExecuteCommandAsync();

        await UpdateNext();

        SubmissionIsPending = false;
    }

    private async Task BeginSkip()
    {
        if (JudgeJudge.NextTeamId == null)
            throw new ArgumentNullException(nameof(JudgeJudge.NextTeamId));

        SubmissionIsPending = true;

        await Db.Insertable(new TeamJudgeIgnored
        {
            TeamId = JudgeJudge.NextTeamId.Value,
            JudgeId = JudgeJudge.Id
        }).ExecuteCommandAsync();

        JudgeJudge.NextTeamId = null;
        await Db.Updateable(JudgeJudge).ExecuteCommandAsync();

        await UpdateNext();

        SubmissionIsPending = false;
    }

    private async Task VoteSkip()
    {
        if (JudgeJudge.NextTeamId == null)
            throw new ArgumentNullException(nameof(JudgeJudge.NextTeamId));

        SubmissionIsPending = true;

        await Db.Insertable(new TeamJudgeIgnored
        {
            TeamId = JudgeJudge.NextTeamId.Value,
            JudgeId = JudgeJudge.Id
        }).ExecuteCommandAsync();

        await UpdateNext();

        SubmissionIsPending = false;
    }

    private async Task Vote(bool previousWon)
    {
        SubmissionIsPending = true;
        if (JudgeJudge.PreviousTeam?.Active == true && JudgeJudge.NextTeam?.Active == true)
        {
            var winner = previousWon ? JudgeJudge.PreviousTeam : JudgeJudge.NextTeam;
            var loser = previousWon ? JudgeJudge.NextTeam : JudgeJudge.PreviousTeam;

            var (newAlpha, newBeta, winnerMu, winnerSigmaSq, loserMu, loserSigmaSq) =
                CrowdBt.Update(JudgeJudge.Alpha, JudgeJudge.Beta, winner.Mu, winner.SigmaSq, loser.Mu, loser.SigmaSq);

            JudgeJudge.Alpha = newAlpha;
            JudgeJudge.Beta = newBeta;
            winner.Mu = winnerMu;
            winner.SigmaSq = winnerSigmaSq;
            loser.Mu = loserMu;
            loser.SigmaSq = loserSigmaSq;

            await Db.Updateable(winner).ExecuteCommandAsync();
            await Db.Updateable(loser).ExecuteCommandAsync();
            await Db.Insertable(new Decision
            {
                JudgeId = JudgeJudge.Id,
                WinnerId = winner.Id,
                LoserId = loser.Id
            }).ExecuteCommandAsync();
        }

        if (JudgeJudge.NextTeamId == null)
            throw new ArgumentNullException(nameof(JudgeJudge.NextTeamId));

        await Db.Insertable(new TeamJudgeViewed
        {
            TeamId = JudgeJudge.NextTeamId.Value,
            JudgeId = JudgeJudge.Id
        }).ExecuteCommandAsync();

        JudgeJudge.PreviousTeam = JudgeJudge.NextTeam;
        JudgeJudge.PreviousTeamId = JudgeJudge.NextTeamId;

        await Db.Updateable(JudgeJudge).ExecuteCommandAsync();

        await Db.Insertable(new TeamJudgeIgnored
        {
            TeamId = JudgeJudge.PreviousTeamId.Value,
            JudgeId = JudgeJudge.Id
        }).ExecuteCommandAsync();

        await UpdateNext();

        SubmissionIsPending = false;
    }

    private async Task UpdateNext()
    {
        await FetchData();

        var next = await ChooseNext();
        JudgeJudge.NextTeamId = next?.Id;
        JudgeJudge.UpdatedAt = DateTimeOffset.Now;

        if (next is not null)
        {
            JudgeJudge.NextTeam = next;
        }

        await Db.Updateable(JudgeJudge).ExecuteCommandAsync();
    }

    public void Dispose()
    {
        _persistingSubscription.Dispose();
    }
}