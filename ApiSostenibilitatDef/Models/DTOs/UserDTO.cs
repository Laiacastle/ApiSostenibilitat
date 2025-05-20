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
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty ;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        
        public double Weight { get; set; }
        public string Exercise { get; set; } = string.Empty;
        public double HoursSleep { get; set; }
        public int Age { get; set; }
        //Relacions
        public List<double> Results { get; set; } = new List<double>();
        public string? Diet { get; set; }

    }
}
