using System;
using System.Collections.Generic;
using System.Linq;
using ReversiClient.Data;
using ReversiClient.Models;

namespace ReversiClient.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ReversiDbContext _context;

        public GameRepository(ReversiDbContext context)
        {
            _context = context;
        }

        public void SaveHistory(int playerScore, int enemyScore)
        {
            _context.History.Add(new MatchRecord()
                {Date = DateTime.Now, EnemyScore = enemyScore, PlayerScore = playerScore});
            _context.SaveChanges();
        }

        public List<MatchRecord> GetHistory()
        {
            return _context.History.ToList();
        }

        public void SaveMatch(string map, string name)
        {
            _context.Saves.Add(new GameState() {Map = map, Name = name, Time = DateTime.Now});
            _context.SaveChanges();
        }

        public List<GameState> GetSaves()
        {
            return _context.Saves.ToList();
        }
    }
}