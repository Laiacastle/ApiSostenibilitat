using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models
{
    [Table("Diet")]
    public class Diet
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Charcateristics {  get; set; }
        
        //Relacions
        public User UserId { get; set; }  

    }
}
