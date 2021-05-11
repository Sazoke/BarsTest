using System.Text.Json.Serialization;

namespace ReversiClient.Models
{
    public class GameDto
    {
        public ResponseType responseType { get; set; }
        public GameChange gameChange { get; set; }
        public int playerId { get; set; }
    }
}