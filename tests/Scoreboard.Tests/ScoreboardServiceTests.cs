using Scoreboard.Models;

namespace Scoreboard.Tests;

public class ScoreboardServiceTests
{
    private readonly InMemoryScoreboardService _sut;

    public ScoreboardServiceTests()
    {
        _sut = new InMemoryScoreboardService(TimeProvider.System);
    }

    [Fact]
    public async Task StartMatch_NewMatch_ZeroScore()
    {
        var matchId = await _sut.StartMatchAsync("home", "away");

        Assert.NotEqual(matchId, Guid.Empty);
        var summary = await _sut.GetLiveSummaryAsync();
        Assert.Collection(summary, s => Assert.Equal(s, new SummaryItem("home", 0, "away", 0)));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    public async Task StartMatch_InvalidNames_Throws(string homeName, string awayName)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.StartMatchAsync(homeName, awayName));
    }

    [Theory]
    [InlineData("home", "away")]
    [InlineData("away", "home")]
    public async Task StartMatch_MatchInProgress_Throws(string homeName, string awayName)
    {
        await _sut.StartMatchAsync("home", "away");

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.StartMatchAsync(homeName, awayName));
    }
    
    [Fact]    
    public async Task FinishMatchAsync_MatchExists_Success()
    {
        var matchId = await _sut.StartMatchAsync("home", "away");

        await _sut.FinishMatchAsync(matchId);

        var summary = await _sut.GetLiveSummaryAsync();
        Assert.Collection(summary, []);
    }

    [Fact]
    public async Task FinishMatchAsync_MatchDoesntExists_Throws()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.FinishMatchAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateScoreAsync_MatchExists_Success()
    {
        var matchId = await _sut.StartMatchAsync("home", "away");

        await _sut.UpdateScoreAsync(matchId, 1, 0);

        var summary = await _sut.GetLiveSummaryAsync();
        Assert.Collection(summary,
            s => Assert.Equal(s, new SummaryItem("home", 1, "away", 0)));
    }

    [Fact]
    public async Task UpdateScoreAsync_MatchDoesntExists_Throws()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.UpdateScoreAsync(Guid.NewGuid(), 1, 0));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(1, 2)]
    public async Task UpdateScoreAsync_WrongScore_Throws(int newHomeScore, int newAwayScore)
    {
        var matchId = await SetMatchAsync("home", 2, "away", 2);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.UpdateScoreAsync(matchId, newHomeScore, newAwayScore));
    }

    [Fact]
    public async Task GetLiveSummary_NoMatches_Empty()
    {
        var result = await _sut.GetLiveSummaryAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLiveSummary_Matches_Ordered()
    {
        await SetMatchAsync("Mexico", 0, "Canada", 5);
        await SetMatchAsync("Spain", 10, "Brasil", 2);
        await SetMatchAsync("Germany", 2, "France", 2);
        await SetMatchAsync("Uruguay", 6, "Italy", 6);
        await SetMatchAsync("Argentina", 3, "Australia", 1);

        var result = await _sut.GetLiveSummaryAsync();

        Assert.Collection(result,
            s => Assert.Equal(s, new SummaryItem("Uruguay", 6, "Italy", 6)),
            s => Assert.Equal(s, new SummaryItem("Spain", 10, "Brasil", 2)),
            s => Assert.Equal(s, new SummaryItem("Mexico", 0, "Canada", 5)),
            s => Assert.Equal(s, new SummaryItem("Argentina", 3, "Australia", 1)),
            s => Assert.Equal(s, new SummaryItem("Germany", 2, "France", 2))
            );
    }

    private async Task<Guid> SetMatchAsync(string homeName, int homeScore, string awayName, int awayScore)
    {
        var matchId = await _sut.StartMatchAsync(homeName, awayName);
        await _sut.UpdateScoreAsync(matchId, homeScore, awayScore);

        return matchId;
    }
}