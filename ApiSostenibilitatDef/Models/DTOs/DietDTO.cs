using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models.DTOs
{
    public class DietDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Characteristics { get; set; }
        //Relacions
        public string? UserId { get; set; }
        public List<int> Recipes { get; set; } = new List<int>();
        public List<DateTime> Results { get; set; } = new List<DateTime>();
    }
}
