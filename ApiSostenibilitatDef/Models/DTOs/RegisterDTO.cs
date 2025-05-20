using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models.DTOs
{
    public class RegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public string Name { get; set; }
        public string Surname { get; set; }
        public double Weight { get; set; }
        public string Exercise { get; set; }
        public double HoursSleep { get; set; }
        public int Age { get; set; }

        
    }
}
