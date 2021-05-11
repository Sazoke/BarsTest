using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ReversiClient.Models;
using ReversiClient.Repositories;
using Point = System.Drawing.Point;

namespace ReversiClient
{
    public class GameWithBot : IGame
    {
        private readonly IGameRepository _repository;
        private Game _game;
        private MainWindow _window;
        private GameState _loadMap;
        
        public GameWithBot(MainWindow window, IGameRepository repository, GameState loadMap = null)
        {
            _window = window;
            _repository = repository;
            _loadMap = loadMap;
        }
        
        public void Play()
        {
            _game = new Game(8, 0);
            if(_loadMap != null)
                _game.SetMap(_loadMap);
            void Step(object sender, RoutedEventArgs e)
            {
                if (!_game.CanPlay()) return;
                var number = int.Parse(((Button) sender).Name.Substring(1));
                var point = new Point(number % _game.Length, number / _game.Length);
                var step = _game.Step(point);
                if (step.Item1 is null) return;
                _window.SetPlaces(step.Item1, step.Item2);
                if (_game.IsEnd()) End();
                BotStep();
                if (_game.IsEnd()) End();
            }

            _window.SetMap(_game.Length, Step);
            _window.SetPlaces(_game.GetMap());
        }

        private void BotStep()
        {
            if (!_game.CanPlay(1)) return;
            var rnd = new Random();
            var list = _game.GetMap().Where(p => p.Item2 == -1).Select(p => p.Item1).ToList();

            var number = rnd.Next(0, list.Count - 1);
            var step = _game.Step(list[number], true);
            while (step.Item1 is null)
            {
                list.RemoveAt(number);
                number = rnd.Next(0, list.Count - 1);
                step = _game.Step(list[number], true);
            }
            _window.SetPlaces(step.Item1, step.Item2);
        }

        public void SaveGame(string name)
        {
            _repository.SaveMatch(_game.ToString(), name);
        }

        public void End()
        {
            var result = _game.GetResult();
            _repository.SaveHistory(result.playerResult, result.enemyResult);
            _window.SetButtons();
        }
    }
}