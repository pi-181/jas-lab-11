using jaslab6;
using NUnit.Framework;

namespace lab6Test
{
    public class TestPassengerDAO : TestGenericDAO<Passenger>
    {

        public TestPassengerDAO()
        {
            Session = NHibernateHelper.OpenSession(true);
            Dao = new NHibernateDAOFactory(Session).getPassengerDAO();
        }

        protected override void createEntities()
        {
            entity1 = new Passenger {FirstName = "Vladimir", LastName = "Ivanov", Sex = "M"};
            entity2 = new Passenger {FirstName = "Denis", LastName = "Kycin", Sex = "M"};
            entity3 = new Passenger {FirstName = "Dmitriy", LastName = "Panfilov", Sex = "M"};
        }

        protected override void checkAllPropertiesDiffer(Passenger entityToCheck1, Passenger entityToCheck2)
        {
            Assert.AreNotEqual(entityToCheck1.FirstName, entityToCheck2.FirstName, "Values must be different");
            Assert.AreNotEqual(entityToCheck1.LastName, entityToCheck2.LastName, "Values must be different");
        }

        protected override void checkAllPropertiesEqual(Passenger entityToCheck1, Passenger entityToCheck2)
        {
            Assert.AreEqual(entityToCheck1.FirstName, entityToCheck2.FirstName, "Values must be equal");
            Assert.AreEqual(entityToCheck1.LastName, entityToCheck2.LastName, "Values must be equal");
            Assert.AreEqual(entityToCheck1.Sex, entityToCheck2.Sex, "Values must be equal");
        }

        [Test]
        public void TestGetByIdPassenger()
        {
            TestGetByIdGeneric();
        }

        [Test]
        public void TestGetAllPassenger()
        {
            TestGetAllGeneric();
        }

        [Test]
        public void TestDeletePassenger()
        {
            TestDeleteGeneric();
        }

    }
}