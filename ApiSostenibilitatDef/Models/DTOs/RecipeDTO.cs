using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models.DTOs
{
    public class RecipeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //Relacions
        public List<string> Ingredients { get; set; } = new List<string>();
        public List<int> Diets { get; set; } = new List<int>();
    }
}
