using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models.DTOs
{
    public class IngredientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> EatForms { get; set; }
        public double Fiber { get; set; }
        public double Calories { get; set; }
        public List<string> Vitamins { get; set; } = new List<string>();

        //Relacions
        public List<int> Recipes { get; set; } = new List<int>();
    }
}
