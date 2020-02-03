using System;

namespace BoardgamesStatisticsCounter.Data
{
    public class UserGame
    {
        public int Id { get; set; }
        
        public int GameId { get; set; }
        
        public int UserId { get; set; }
        
        public DateTime GameDateTime { get; set; }
        
        public string? Players { get; set; }
        
        public string? Score { get; set; }
        
        public string? Winner { get; set; }
    }
}