using Microsoft.AspNet.Identity.EntityFramework;
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
    [Table("User")]
    public class User : IdentityUser
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Surname { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Range(0, 1000, ErrorMessage = "Value for Weight must be bigger than 0.")]
        public double Weight { get; set; }
        [Required]
        public ExerciciEnum Exercise { get; set; }
        [Required]
        public double HoursSleep { get; set; }
        [Required]
        public int Age { get; set; }

        //Relacions
        public List<Result> Results { get; set; } = new List<Result>();
        public Diet Diet { get; set; }

    }
}
