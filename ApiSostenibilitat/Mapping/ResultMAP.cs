using ApiSostenibilitat.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Mapping
{
    public class ResultMAP: ClassMap<Result>
    {
        public ResultMAP()
        {
            Table("Result");
            Id(r => r.Date);
            References(r => r.UserId)
              .Column("Results")
              .Not.LazyLoad()
              .Fetch.Join();
            References(r => r.GameId)
              .Column("Results")
              .Not.LazyLoad()
              .Fetch.Join();
            References(r => r.IdDiet)
                .Column("Results")
                .Not.LazyLoad()
                .Fetch.Join();
        }
    }
}
