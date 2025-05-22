using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models.DTOs
{
    public class GameDTO
    {
        public int Id { get; set; }
        public int MinRes { get; set; }
        public int MaxRes { get; set; }
        public string Name { get; set; }

        //relacions
        public List<int> Results { get; set; } = new List<int>();
    }
}
