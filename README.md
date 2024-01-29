# Live Football ScoreBoard library
Live Football World Cup Scoreboard library that allows to create, update and finish matches as well as to show all ongoing matches and their scores.

## Assumptions made
This is a first stage of development. Implementstion made using in-memory storage.  
There are plans of further development and support of external storage providers.  
Can be used in a multi-threaded environment.

## Public interface
The in-memory implementation of scoreboard implements a common interface:
```C#
public interface IScoreboardService
{
    public Task<Guid> StartMatchAsync(string homeTeam, string awayTeam, CancellationToken cancellation = default);
    public Task FinishMatchAsync(Guid matchId, CancellationToken cancellation = default);
    public Task UpdateScoreAsync(Guid matchId, int homeScore, int awayScore, CancellationToken cancellation = default);
    public Task<IEnumerable<SummaryItem>> GetLiveSummaryAsync(CancellationToken cancellation = default);
}
```

## Usage
Instantiating a service, providing a _System.TimeProvider_ implementation:
```C#
var scoreService = new InMemoryScoreboardService(TimeProvider.System);
```

Starting a match:
```C#
var matchId = await _sut.StartMatchAsync("home team", "away team");
```
If a match starts successfully, you will get its ID (matchId) as a result.
Later you will use that ID to manage the match state.

Updating a match's score:
```C#
await _sut.UpdateScoreAsync(matchId, 1, 0);
```
here you provide an ID of the match, home team's score and away team's score.

Getting a live summary:
```C#
var result = await _sut.GetLiveSummaryAsync();
```
that will provide summaries of all ongoing matches, ordered by their total score and a start time.

Finishing a match, providing a match Id:
```C#
await _sut.FinishMatchAsync(matchId);
```
