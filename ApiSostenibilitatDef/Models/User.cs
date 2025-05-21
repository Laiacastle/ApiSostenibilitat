
using Microsoft.AspNetCore.Identity;
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
    [Table("User")]
    public class User : IdentityUser
    {
        
        [Required]
        public string Name { get; set; }
        public string Surname { get; set; }
        [Range(0, 1000, ErrorMessage = "Value for Weight must be greater than 0.")]
        public double Weight { get; set; }
        [Required]
        public ExerciciEnum Exercise { get; set; }
        [Required]
        public double HoursSleep { get; set; }
        [Required]
        [Range(0, 99, ErrorMessage = "Value for Age must be greater than 0.")]
        public int Age { get; set; }

        //Relacions
        [JsonIgnore]
        public List<Result> Results { get; set; } = new List<Result>();
        //[ForeignKey("UserId")]
        public Diet? Diet { get; set; } = null!;

    }
}
