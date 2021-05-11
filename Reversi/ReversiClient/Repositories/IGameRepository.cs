using System.Collections.Generic;
using ReversiClient.Models;

namespace ReversiClient.Repositories
{
    public interface IGameRepository
    {
        void SaveHistory(int playerScore, int enemyScore);
        List<MatchRecord> GetHistory();
        void SaveMatch(string map, string name);
        List<GameState> GetSaves();
    }
}