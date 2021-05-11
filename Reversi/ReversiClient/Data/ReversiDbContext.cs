using System.Data.Entity;
using ReversiClient.Models;

namespace ReversiClient.Data
{
    public class ReversiDbContext : DbContext
    {
        public ReversiDbContext(string connection)
            :base(connection)
        {
        }
        
        public DbSet<MatchRecord> History { get; set; }
        public DbSet<GameState> Saves { get; set; }
    }
}