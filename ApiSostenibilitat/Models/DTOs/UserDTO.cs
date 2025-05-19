using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Surname { get; set; }
        public double Weight { get; set; }
        public ExerciciEnum Exercise { get; set; }
        public double HoursSleep { get; set; }
        public int Age { get; set; }
        //Relacions
        public List<string> Results { get; set; } = new List<string>();
        public int Diet { get; set; }

    }
}
