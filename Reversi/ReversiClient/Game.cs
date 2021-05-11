using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using ReversiClient.Models;

namespace ReversiClient
{
    public class Game
    {
        private readonly int _length;
        private Place[][] _map;
        private int _playerNumber;
        private int _currentStep;

        public int Length => _length;
        
        public Place this[int y, int x] => _map[y][x];

        public Game(int length, int id)
        {
            _currentStep = 0;
            _length = length;
            _playerNumber = id;
            if(_length % 2 == 1)
                throw new Exception("Length must be multiple of 2");
            
            _map = new Place[_length][];
            for (var i = 0; i < _map.Length; i++)
                _map[i] = new Place[_length];

            for (var i = 0; i < 2; i++)
                for (var j = 0; j < 2; j++)
                    _map[_length / 2 - i][_length / 2 - j] = (Place)((i + j) % 2 + 1);
        }
        
        public List<(Point, int)> GetMap()
        {
            var result = new List<(Point, int)>();
            for (var i = 0; i < _length; i++)
                for (var j = 0; j < _length; j++)
                    result.Add((new Point(j, i), (int)_map[i][j] - 1));
            return result;
        }
        
        public int GetNumber() => _playerNumber;
        
        public bool CanPlay(int playerNumber = -1)
        {
            if (playerNumber == -1)
                playerNumber = _playerNumber;
            for (var i = 0; i < _length; i++)
                for (var j = 0; j < _length; j++)
                    if (_map[i][j].Equals(Place.Empty) &&
                        GetWays(new Point(j, i), playerNumber).FirstOrDefault() != null)
                        return true;

            return false;
        }

        public (List<Point>, int) Step(Point point, bool anotherPLayer = false)
        {
            if (!anotherPLayer)
                if (!_playerNumber.Equals(_currentStep % 2) || !_map[point.Y][point.X].Equals(Place.Empty))
                    return (null, 0);
            
            var playerNumber = anotherPLayer ? 1 - _playerNumber : _playerNumber;
            var ways = GetWays(point, playerNumber).ToList();
            if(ways.Count == 0)
                return (null, 0);
            
            SetChange(ways, playerNumber);
            _currentStep++;
            return (ways, playerNumber);
        }

        private void SetChange(List<Point> way, int playerNumber)
        {
            foreach (var point in way)
                _map[point.Y][point.X] = (Place) (playerNumber + 1);
        }

        public bool IsEnd() => !(CanPlay() || CanPlay(1 - _playerNumber));

        private IEnumerable<Point> GetWays(Point point, int playerNumber)
        {
            for (var i = -1; i < 2; i++)
                for (var j = -1; j < 2; j++)
                    if (point.Y + i >= 0 && point.X + j >= 0 &&
                        point.Y + i < _length && point.X + j < _length &&
                        _map[point.Y + i][point.X + j].Equals((Place) (2 - playerNumber)))
                    {
                        var line = new List<Point>() {point};
                        var k = 1;
                        while (k * i + point.Y >= 0 &&
                               k * i + point.Y < _length &&
                               k * j + point.X >= 0 &&
                               k * j + point.X < _length &&
                               _map[k * i + point.Y][k * j + point.X] != Place.Empty)
                        {
                            line.Add(new Point(k * j + point.X, k * i + point.Y));
                            k++;
                        }

                        var last = line.LastOrDefault();
                        while (line.Count > 0 && _map[last.Y][last.X] != (Place) (playerNumber + 1))
                        {
                            line.RemoveAt(line.Count - 1);
                            last = line.LastOrDefault();
                        }
                        

                        if (line.Count < 1) continue;
                        line.RemoveAt(line.Count - 1);
                        foreach (var newPoint in line)
                            yield return newPoint;
                    }
        }

        public (int playerResult, int enemyResult) GetResult()
        {
            var playerResult = 0;
            var enemyResult = 0;
            foreach (var line in _map)
            {
                foreach (var place in line)
                {
                    if ((int) place == _playerNumber + 1)
                        playerResult++;
                    else if ((int) place == 2 - _playerNumber)
                        enemyResult++;
                }
            }

            return (playerResult, enemyResult);
        }

        public override string ToString()
        {
            var result = JsonSerializer.Serialize(_map);
            return result;
        }

        public void SetMap(GameState map)
        {
            _map = JsonSerializer.Deserialize<Place[][]>(map.Map);
        }
    }
}