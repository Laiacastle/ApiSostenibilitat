using ApiSostenibilitat.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Mapping
{
    public class IngredientMAP: ClassMap<Ingredient>
    {
        public IngredientMAP()
        {
            Table("Ingredient");
            Id(i => i.Id);
            Map(i => i.Name).Column("Name");
            Map(i => i.EatForms).Column("EatForms");
            Map(i => i.Fiber).Column("Fiber");
            Map(i => i.Calories).Column("Calories");
            Map(i => i.Vitamins).Column("Vitamins");

            HasMany(i=>i.Recipes)
                .KeyColumn("Ingredients")
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Fetch.Join();
        }
    }
}
