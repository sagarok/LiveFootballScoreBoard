namespace Scoreboard.Models
{
    internal record MatchInfo
    {
        public MatchInfo(string homeName, string awayName, DateTimeOffset startedAt)
        {
            Id = Guid.NewGuid();
            HomeName = homeName;
            AwayName = awayName;
            StartedAt = startedAt;
        }

        public Guid Id { get; }
        public string HomeName { get; }
        public string AwayName { get; }
        public DateTimeOffset StartedAt { get; }

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}
