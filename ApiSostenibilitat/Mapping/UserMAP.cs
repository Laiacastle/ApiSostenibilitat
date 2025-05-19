using ApiSostenibilitat.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSostenibilitat.Mapping
{
    public class UserMAP: ClassMap<User>
    {
        public UserMAP()
        {
            Table("User");
            Id(u => u.Id);
            Map(u => u.Surname).Column("Surname");
            Map(u => u.Email).Column("Email");
            Map(u => u.Weight).Column("Weight");
            Map(u => u.Exercise).Column("Exercise");
            Map(u => u.HoursSleep).Column("HorusSleep");
            Map(u => u.Age).Column("Age");

            HasMany(u=>u.Results)
                .KeyColumn("UserId")
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Fetch.Join();

            References(u=>u.Diet)
                .Column("UserId")
                .Not.LazyLoad()
                .Fetch.Join();
        }
    }
}
