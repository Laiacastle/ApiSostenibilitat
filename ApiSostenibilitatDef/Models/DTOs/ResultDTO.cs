using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models.DTOs
{
    public class ResultDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int GameId { get; set; }
        public int DietId { get; set; }
        public DateTime Date { get; set; }
        public int FiResult {get;set;}
    }
}
