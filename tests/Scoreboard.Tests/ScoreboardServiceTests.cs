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
        var result = await _sut.StartMatchAsync("home", "away");

        Assert.NotEqual(result, Guid.Empty);
    }

    [Fact]
    public async Task StartMatch_MatchInProgress_Fail()
    {
        await _sut.StartMatchAsync("home", "away");

        var result = await _sut.StartMatchAsync("home", "away");

        Assert.NotEqual(result, Guid.Empty);
    }


    [Fact]
    public void FinishMatchAsyncTest()
    {
    }

    [Fact]
    public void UpdateScoreAsyncTest()
    {
    }
    
    [Fact]
    public void GetLiveSummaryTest()
    {
    }
}