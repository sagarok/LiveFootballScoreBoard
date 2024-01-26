namespace Scoreboard.Tests;

public class ScoreboardServiceTests
{
    private readonly ScoreboardService _sut;

    public ScoreboardServiceTests()
    {
        _sut = new ScoreboardService();
    }

    [Fact]
    public async Task StartMatch_NewMatch_Success()
    {
        var matchId = await _sut.StartMatchAsync("home", "away");

        Assert.NotEqual(matchId, Guid.Empty);
    }

    [Fact]
    public async Task StartMatch_MatchInProgress_Throws()
    {
        await _sut.StartMatchAsync("home", "away");

        await Assert.ThrowsAsync<Exception>(() => _sut.StartMatchAsync("home", "away"));
    }


    [Fact]
    public async Task FinishMatchAsyncTest_MatchExists_Success()
    {
        var matchId = await _sut.StartMatchAsync("home", "away");

        await _sut.FinishMatchAsync(matchId);
    }

    [Fact]
    public async Task FinishMatchAsyncTest_MatchDoesntExists_Throws()
    {
        await Assert.ThrowsAsync<Exception>(() => _sut.FinishMatchAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateScoreAsyncTest_MatchExists_Success()
    {
        var matchId = await _sut.StartMatchAsync("home", "away");

        await _sut.UpdateScoreAsync(matchId, 1, 0);
    }

    [Fact]
    public async Task UpdateScoreAsyncTest_MatchDoesntExists_Throws()
    {
        await Assert.ThrowsAsync<Exception>(() => _sut.UpdateScoreAsync(Guid.NewGuid(), 1, 0));
    }

    [Theory]
    [InlineData(1, 1)]
    public async Task UpdateScoreAsyncTest_WrongScore_Throws(int newHomeScore, int newAwayScore)
    {
        await Assert.ThrowsAsync<Exception>(() => _sut.UpdateScoreAsync(Guid.NewGuid(), 1, 0));
    }


    [Fact]
    public void GetLiveSummaryTest()
    {
    }
}