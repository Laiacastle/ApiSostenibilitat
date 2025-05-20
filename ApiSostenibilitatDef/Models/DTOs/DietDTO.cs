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
        public string Charcateristics { get; set; }
        //Relacions
        public string UserId { get; set; }
        public List<int> Recipes { get; set; } = new List<int>();
    }
}
