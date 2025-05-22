using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models
{
    [Table("Result")]
    public class Result
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        
        public User User { get; set; }
        [Required]
        [ForeignKey("Game")]
        public int GameId { get; set; }
        
        public Game Game { get; set; } = null!;
        [Required]
        [ForeignKey("Diet")]
        public int DietId { get; set; }
        
        public Diet Diet { get; set; } = null!;
        
        [Required]
        
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 9999, ErrorMessage = "The result must be greater than 0!")]
        public int FiResult { get; set; }

        
    }
}
