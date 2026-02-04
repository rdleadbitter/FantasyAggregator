using System;

namespace FantasyAggregatorApp.Models
{
    public class TeamPlayer
    {
        public int TeamPlayerId { get; set; }
        public int TeamId { get; set; }
        public int PlayerId { get; set; }
        public string RosterSlot { get; set; }
        public DateTime? AcquiredOn { get; set; }
    }
}
