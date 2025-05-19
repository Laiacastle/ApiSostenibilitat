using ApiSostenibilitat.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Mapping
{
    public class GameMAP: ClassMap<Game>
    {
        public GameMAP()
        {
            Table("Game");
            Id(x => x.Id);
            Map(n => n.MinRes).Column("MinRes");
            Map(n => n.MaxRes).Column("MaxRes");
            Map(n => n.Name).Column("Name");
            HasMany(n=>n.Results)
                .KeyColumn("GameId")
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Fetch.Join();
        }
    }
}
