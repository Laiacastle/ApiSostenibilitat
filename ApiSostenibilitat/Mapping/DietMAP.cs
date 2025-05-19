
using ApiSostenibilitat.Models;
using FluentNHibernate.Mapping;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Mapping
{
    public class DietMAP: ClassMap<Diet>
    {
        public DietMAP()
        {
            Table("Diet");
            Id(x => x.Id);
            Map(n => n.Characteristics).Column("Characteristics");
            References(x => x.UserId)
               .Column("UserId")
               .Not.LazyLoad()
               .Fetch.Join();

            HasMany(d=>d.Recipes)
                .KeyColumn("Diets")
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Fetch.Join();

        }

    }
}
