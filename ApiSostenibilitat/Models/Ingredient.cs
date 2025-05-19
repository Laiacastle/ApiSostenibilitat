using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models
{
    [Table("Ingredient")]
    public class Ingredient
    {
        [Required]
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }
        public List<string> EatForms { get; set; }
        [Required]
        public double Fiber {  get; set; }
        [Required]
        public double Calories {  get; set; }
        public List<Vitamin> Vitamins { get; set; } = new List<Vitamin>();

        //Relacions
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}
