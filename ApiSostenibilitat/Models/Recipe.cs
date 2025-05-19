using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models
{
    [Table("Recipe")]
    public class Recipe
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        //relacions
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<Diet> Diets { get; set; } = new List<Diet>();
    }
}
