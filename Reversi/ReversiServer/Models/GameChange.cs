using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Reversi.Models
{
    public class GameChange
    {
        public GameChange(){}
        public GameChange(int[] change, bool gameEnd = false, int playerNumber = -1)
        {
            Change = change;
            GameEnd = gameEnd;
            PlayerNumber = playerNumber;
        }

        [JsonPropertyName("change")]
        public int[] Change { get; set; }
        [JsonPropertyName("playerNumber")]
        public int PlayerNumber { get; set; }
        [JsonPropertyName("gameEnd")]
        public bool GameEnd { get; set; }
    }
}