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
        public double MinRes { get; set; }
        public double MaxRes { get; set; }
        public string Name { get; set; }

        //relacions
        public List<double> Results { get; set; } = new List<double>();
    }
}
