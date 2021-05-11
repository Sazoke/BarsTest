using System.Linq;
using Reversi.Models;

namespace Reversi.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly int[] _playerIds;
        private GameChange _gameChange;
        private bool _isEnd;

        public GameRepository()
        {
            _playerIds = new[] {-1, -1};
        }

        public bool IsPlayer(int id) =>  _playerIds.Where(player => player != -1).Any(player => player == id);
        
        public int SetPlayer(int id)
        {
            for (var i = 0; i < _playerIds.Length; i++)
                if (_playerIds[i] == -1)
                {
                    _playerIds[i] = id;
                    return i;
                }

            return -1;
        }

        public int GetPlayer(int id)
        {
            for (var i = 0; i < _playerIds.Length; i++)
                if (_playerIds[i].Equals(id))
                    return i;
            return -1;
        }

        public bool IsEnd() => _isEnd;

        public void End() => _isEnd = true;

        public bool HavePlace() => _playerIds.Any(p => p == -1);

        public bool HaveChange() => _gameChange is not null;

        public bool IsYourChange(int id) => _gameChange.PlayerNumber.Equals(id);

        public GameChange GetChange() => _gameChange;

        public void SetChange(GameChange gameChange) => _gameChange = gameChange;
    }
}