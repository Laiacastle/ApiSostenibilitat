﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Models
{
    public class Vitamin
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    }
}
