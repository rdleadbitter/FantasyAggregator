namespace FantasyAggregatorApp.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string TeamAbbrev { get; set; }
        public bool Active { get; set; }
    }
}
