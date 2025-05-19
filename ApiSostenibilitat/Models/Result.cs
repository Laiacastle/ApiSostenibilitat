using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models
{
    [Table("Result")]
    public class Result
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public int IdDiet { get; set; }
        [Key]
        [Required]
        public DateTime Date { get; set; }

        
    }
}
