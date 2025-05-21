using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models
{
    [Table("Diet")]
    public class Diet
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Characteristics {  get; set; }

        //Relacions
        [ForeignKey("User")] //fk de USer
        public string? UserId {  get; set; }
        public User? User { get; set; }  
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
        [JsonIgnore]
        public List<Result> Results { get; set; } = new List<Result>();
    }
}
