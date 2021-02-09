using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace jaslab6
{
    public class NHibernateHelper
    {
        private static ISessionFactory _factory;
        public static ISession OpenSession(bool execute)
        {
            if (_factory == null)
            {
                Func<string, string> env = Environment.GetEnvironmentVariable;
                _factory = Fluently.Configure()
                    .Database(PostgreSQLConfiguration
                        .PostgreSQL82.ConnectionString(c => c
                            .Host(env("db_host"))
                            .Port(int.Parse(env("db_port")))
                            .Database(env("db_name"))
                            .Username(env("db_user"))
                            .Password(env("db_password"))
                        )
                    )
                    .Mappings(m => m.FluentMappings.Add<CabinMap>().Add<PassengerMap>())
                    .ExposeConfiguration(c =>
                    {
                        c.Properties.Add("hbm2ddl.keywords", "none");
                        new SchemaExport(c).Create(false, execute);
                    })
                    .BuildSessionFactory();
            }

            return _factory.OpenSession();
        }
        
    }
}