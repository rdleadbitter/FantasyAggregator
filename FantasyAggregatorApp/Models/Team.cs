namespace FantasyAggregatorApp.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public int PlatformId { get; set; }
        public string PlatformTeamId { get; set; }
        public string TeamName { get; set; }
        public string LeagueName { get; set; }
    }
}
