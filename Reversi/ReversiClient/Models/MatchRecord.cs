using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReversiClient.Models
{
    [Table("History")]
    public class MatchRecord
    {
        public int EnemyScore { get; set; }
        public int PlayerScore { get; set; }
        [Key]
        public DateTime Date { get; set; }
    }
}