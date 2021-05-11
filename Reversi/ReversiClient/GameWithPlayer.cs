using System.Timers;
using System.Windows;
using System.Windows.Controls;
using ReversiClient.Models;
using ReversiClient.Repositories;
using Point = System.Drawing.Point;

namespace ReversiClient
{
    public class GameWithPlayer : IGame
    {
        private static Server _server;
        private Game _game;
        private Timer _timer;
        private ClientStatus _status;
        private MainWindow _window;
        private readonly IGameRepository _repository;

        public GameWithPlayer(MainWindow window, IGameRepository repository)
        {
            _window = window;
            _repository = repository;
        }


        public void Play()
        {
            _timer = new Timer(5000);
            _timer.Elapsed += (sender, args) =>
            {
                try
                {
                    _server = new Server();
                    CheckResponse();
                    _timer.Stop();
                    _timer = new Timer(1000);
                    _timer.Elapsed += (sender, args) => CheckResponse();
                    _timer.Start();
                }
                catch
                {
                    _window.WriteMessage("Server is not found");
                }
            };
            _timer.Start();
            
        }

        public void End()
        {
            _server.SetUpdate(new GameChange {GameEnd = true});
            _server = null;
            var result = _game.GetResult();
            _game = null;
            _repository.SaveHistory(result.playerResult, result.enemyResult);
            _window.SetButtons();
        }

        private void CheckResponse()
        {
            if(_status == ClientStatus.End)
                return;
            var gameDto = _server.GetUpdate();
            switch (gameDto.responseType)
            {
                case ResponseType.WaitPlayer:
                    if (!_status.Equals(ClientStatus.Wait))
                    {
                        _status = ClientStatus.Wait;
                        _window.WriteMessage("Wait another player");
                    }
                    break;
                case ResponseType.NoPlace:
                    if (!_status.Equals(ClientStatus.NoPlace))
                    {
                        _status = ClientStatus.Wait;
                        _window.WriteMessage("Wait free place");
                    }
                    break;
                case ResponseType.NoChange:
                    if (!_status.Equals(ClientStatus.InGame))
                    {
                        _status = ClientStatus.InGame;

                        void Step(object sender, RoutedEventArgs e)
                        {
                            if (!_game.CanPlay()) return;
                            var number = int.Parse(((Button) sender).Name.Substring(1));
                            var point = new Point(number % _game.Length, number / _game.Length);
                            var step = _game.Step(point);
                            if (step.Item1 is null) return;
                            _window.SetPlaces(step.Item1, step.Item2);
                            _server.SetUpdate(new GameChange(new[] {point.X, point.Y}, playerNumber: _game.GetNumber()));
                            if (_game.IsEnd()) _server.SetUpdate(new GameChange {GameEnd = true});
                        }

                        _window.SetMap(_game.Length, Step);
                        _window.SetPlaces(_game.GetMap());
                    }
                    break;
                case ResponseType.SetPlayer:
                    _game = new Game(8, gameDto.playerId);
                    _status = ClientStatus.Wait;
                    _window.WriteMessage("Wait another player");
                    break;
                case ResponseType.Change:
                    var step = _game.Step(new Point(gameDto.gameChange.Change[0], gameDto.gameChange.Change[1]), true);
                    _window.SetPlaces(step.Item1, step.Item2);
                    break;
                case ResponseType.GameEnd:
                    if(_status == ClientStatus.Wait || _status == ClientStatus.NoPlace)
                        break;
                    _status = ClientStatus.End;
                    _timer.Stop();
                    End();
                    break;
            }
        }
        
    }
}