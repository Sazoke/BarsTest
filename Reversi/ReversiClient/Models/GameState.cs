using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReversiClient.Models
{
    [Table("Saves")]
    public class GameState
    {
        public string Map { get; set; }
        [Key]
        public DateTime Time { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}