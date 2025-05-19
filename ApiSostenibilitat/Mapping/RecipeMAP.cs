using ApiSostenibilitat.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Mapping
{
    public class RecipeMAP: ClassMap<Recipe>
    {
        public RecipeMAP()
        {
            Table("Recipe");
            Id(r => r.Id);
            Map(r => r.Name).Column("Name");
            Map(r =>r.Description).Column("Description");
            HasMany(r=>r.Ingredients)
                .KeyColumn("Recipes")
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Fetch.Join();

            HasMany(r => r.Diets)
                .KeyColumn("Recipes")
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Fetch.Join();
        }
    }
}
