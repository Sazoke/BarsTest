using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Reversi.Models;
using Reversi.Repositories;

namespace Reversi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        private static IGameRepository _gameRepository;
        private static int countGuests;

        public GameController()
        {
            _gameRepository ??= new GameRepository();
        }
        
        [HttpGet]
        public GameDto Index([FromQuery] int userId)
        {
            if (userId == -1)
                return new GameDto() {playerId = countGuests++};
            if (!_gameRepository.IsPlayer(userId))
            {
                if (_gameRepository.IsEnd())
                    _gameRepository = new GameRepository();
                if (!_gameRepository.HavePlace())
                    return new GameDto() {responseType = ResponseType.NoPlace};
                var id = _gameRepository.SetPlayer(userId);
                return new GameDto() {responseType = ResponseType.SetPlayer, playerId = id};
            }

            if (_gameRepository.IsEnd())
            {
                _gameRepository = null;
                return new GameDto() {responseType = ResponseType.GameEnd};
            }

            var playerId = _gameRepository.GetPlayer(userId);
            if (_gameRepository.HavePlace())
                return new GameDto()
                {
                    responseType = ResponseType.WaitPlayer,
                    playerId = playerId
                };

            if (!_gameRepository.HaveChange() || _gameRepository.IsYourChange(playerId))
                return new GameDto() {responseType = ResponseType.NoChange, playerId = playerId};
            
            var change = _gameRepository.GetChange();
            _gameRepository.SetChange(null);
            var result = new GameDto() {responseType = ResponseType.Change, playerId = playerId, gameChange = change};
            return result;
        }

        [HttpPost]
        public IActionResult ChangeGame([FromQuery]string json)
        {
            var gameChange = JsonSerializer.Deserialize<GameChange>(json);
            if (_gameRepository is null)
                return NotFound();
            
            if (gameChange != null && gameChange.GameEnd)
            {
                _gameRepository.End();
                return Ok();
            }

            _gameRepository.SetChange(gameChange);
            return Ok();
        }
    }
}