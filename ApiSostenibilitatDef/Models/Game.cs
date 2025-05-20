using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models
{
    [Table("Game")]
    public class Game
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(0, 1000000,
        ErrorMessage = "Value for MinRes must be greater than 0.")]
        public double MinRes { get; set; }

        [Required]
        [Range(0.1, 999999,
        ErrorMessage = "Value for MaxRes must be greater than 0.1")]
        public double MaxRes { get; set; }
        [Required]
        public string Name {  get; set; }

        //relacions
        public List<Result> Results { get; set; } = new List<Result>();
    }
}
