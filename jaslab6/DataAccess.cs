using System.Collections.Generic;
using System.Linq;
using NHibernate;

namespace jaslab6
{
    public interface IGenericDAO<T>
    {
        void SaveOrUpdate(T item);

        T GetById(int id);

        IList<T> GetAll();

        void Delete(T item);

    }
    
    public class GenericDAO<T> : IGenericDAO<T>
    { 
        protected ISession session;

        public GenericDAO() { }

        public GenericDAO(ISession session)
        {
            this.session = session;
        }

        public void SaveOrUpdate(T item)
        {
            var transaction = session.BeginTransaction();
            session.SaveOrUpdate(item);
            transaction.Commit();
            session.Flush();
        }

        public T GetById(int id)
        {
            return session.Get<T>(id);
        }

        public IList<T> GetAll()
        {
            return session
                .CreateCriteria(typeof(T))
                .List<T>();
        }

        public void Delete(T item)
        {
            var transaction = session.BeginTransaction();
            session.Delete(item);
            transaction.Commit();
            session.Flush();
        }
    }

    public interface ICabinDAO : IGenericDAO<Cabin>
    {
        Cabin GetCabinByName(string name);
        void RemoveCabinByName(string name);
    }
    
    public interface IPassengerDAO : IGenericDAO<Passenger>
    {
        IList<Passenger> GetPassengerByCabin(int cabinId);
    }

    public class CabinDAO : GenericDAO<Cabin>, ICabinDAO
    {
        public CabinDAO(ISession session) : base(session)
        {
            
        }
        
        public Cabin GetCabinByName(string name)
        {
            return session.CreateSQLQuery(@"SELECT * FROM cabins WHERE cabin_name LIKE :cab_name")
                .AddEntity("Cabin", typeof(Cabin))
                .SetParameter("cab_name", name)
                .List<Cabin>()
                .FirstOrDefault();
        }

        public void RemoveCabinByName(string name)
        {
            Delete(GetCabinByName(name));
        }
    }
    
    public class PassengerDAO : GenericDAO<Passenger>, IPassengerDAO
    {
        public PassengerDAO(ISession session) : base(session)
        {
            
        }

        public IList<Passenger> GetPassengerByCabin(int cabin_id)
        {
            return session.CreateSQLQuery("SELECT * FROM passengers WHERE cabin_id = " + cabin_id)
                .AddEntity("Passenger", typeof(Passenger))
                .List<Passenger>();
        }
    }

    public abstract class DAOFactory
    {
        public abstract ICabinDAO getCabinDAO();
        public abstract IPassengerDAO getPassengerDAO();
    }
    
    public class NHibernateDAOFactory : DAOFactory
    {
        /** NHibernate sessionFactory */
        protected ISession session = null;

        public NHibernateDAOFactory(ISession session)
        {
            this.session = session;
        }

        public override ICabinDAO getCabinDAO() 
        {
            return new CabinDAO(session);
        }

        public override IPassengerDAO getPassengerDAO()
        {
            return new PassengerDAO(session);
        }
    }
    
}