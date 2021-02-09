using System;
using jaslab6;
using NUnit.Framework;

namespace lab6Test
{
    public class TestCabinDAO : TestGenericDAO<Cabin>
    {
        private readonly ICabinDAO _cabinDao;
        private readonly IPassengerDAO _passDao;
        
        private Passenger _passenger1;
        private Passenger _passenger2;
        private Passenger _passenger3;

        public TestCabinDAO()
        {
            Session = NHibernateHelper.OpenSession(true);
            
            var factory = new NHibernateDAOFactory(Session);
            Dao = _cabinDao = factory.getCabinDAO();
            _passDao = factory.getPassengerDAO();
        }

        protected override void createEntities()
        {
            entity1 = new Cabin {CabinName = "Cabin №1", Square = 50, ClassName = "S"};
            entity2 = new Cabin {CabinName = "Cabin №2", Square = 20, ClassName = "B"};
            entity3 = new Cabin {CabinName = "Cabin №3", Square = 10, ClassName = "E"};
        }

        protected override void checkAllPropertiesDiffer(Cabin entityToCheck1, Cabin entityToCheck2)
        {
            const string message = "Values must be different";
            Assert.AreNotEqual(entityToCheck1.CabinName, entityToCheck2.CabinName, message);
            Assert.AreNotEqual(entityToCheck1.Square, entityToCheck2.Square, message);
            Assert.AreNotEqual(entityToCheck1.ClassName, entityToCheck2.ClassName, message);
        }

        protected override void checkAllPropertiesEqual(Cabin entityToCheck1, Cabin entityToCheck2)
        {
            const string message = "Values must be equal";
            Assert.AreEqual(entityToCheck1.CabinName, entityToCheck2.CabinName, message);
            Assert.AreEqual(entityToCheck1.Square, entityToCheck2.Square, message);
            Assert.AreEqual(entityToCheck1.ClassName, entityToCheck2.ClassName, message);
        }

        [Test]
        public void TestGetByIdCabin() => TestGetByIdGeneric();
        

        [Test]
        public void TestGetAllCabin() => TestGetAllGeneric();
        

        [Test]
        public void TestDeleteCabin() => TestDeleteGeneric();
        

        [Test]
        public void TestGetCabinByName()
        {
            Cabin cabin1 = _cabinDao.GetCabinByName(entity1.CabinName);
            Cabin cabin2 = _cabinDao.GetCabinByName(entity2.CabinName);
            Cabin cabin3 = _cabinDao.GetCabinByName(entity3.CabinName);
            
            Assert.IsNotNull(cabin1, "Service method getCabinByName should return cabin if successful");
            Assert.IsNotNull(cabin2, "Service method getCabinByName should return cabin if successful");
            Assert.IsNotNull(cabin3, "Service method getCabinByName should return cabin if successful");
            
            checkAllPropertiesEqual(cabin1, entity1);
            checkAllPropertiesEqual(cabin2, entity2);
            checkAllPropertiesEqual(cabin3, entity3);
        }

        [Test]
        public void TestGetAllStudentOfCabin()
        {
            createEntitiesForPassenger();
            
            Assert.IsNotNull(_passenger1, "Please, create object for passenger1");
            Assert.IsNotNull(_passenger2, "Please, create object for passenger2");
            Assert.IsNotNull(_passenger3, "Please, create object for passenger3");

            entity1.Passengers.Add(_passenger1);
            _passenger1.Cabin = entity1;
            
            entity1.Passengers.Add(_passenger2);
            _passenger2.Cabin = entity1;
            
            entity1.Passengers.Add(_passenger3);
            _passenger3.Cabin = entity1;

            try
            {
                Dao.SaveOrUpdate(entity1);
                var savedObject = getPersistentObject(entity1);
                Assert.IsNotNull(savedObject, "DAO method saveOrUpdate should return entity if successful");
                checkAllPropertiesEqual(savedObject, entity1);
                entity1 = savedObject;
            }
            catch (Exception e)
            {
                Assert.Fail($"Fail to save entity1: {e.Message}");
            }

            var passengerList = _passDao.GetPassengerByCabin(entity1.Id);
            Assert.IsNotNull(passengerList, "List can't be null");
            Assert.IsTrue(passengerList.Count == 3, "Count of passengers in the list must be 3");
            
            checkAllPropertiesEqualForPassenger(passengerList[0], _passenger1);
            checkAllPropertiesEqualForPassenger(passengerList[1], _passenger2);
            checkAllPropertiesEqualForPassenger(passengerList[2], _passenger3);
        }

        [Test]
        public void TestDelCabinByName()
        {
            try
            {
                _cabinDao.RemoveCabinByName(entity2.CabinName);
            }
            catch (Exception e)
            {
                Assert.Fail($"Deletion should be successful of entity2: {e.Message}");
            }

            // Checking if other two entities can be still found
            getEntityGeneric(entity1.Id, entity1);
            getEntityGeneric(entity3.Id, entity3);

            // Checking if entity2 can not be found
            try
            {
                var foundCabin = Dao.GetById(entity2.Id);
                Assert.IsNull(foundCabin, "After deletion entity should not be found with name " + entity2.CabinName);
            }
            catch (Exception e)
            {
                Assert.Fail($"Should return null if finding the deleted entity: {e.Message}");
            }

            // Checking if other two entities can still be found in getAll list
            var list = getListOfAllEntities();
            Assert.IsTrue(list.Contains(entity1), "After dao method GetAll list should contain entity1");
            Assert.IsTrue(list.Contains(entity3), "After dao method GetAll list should contain entity3");
        }

        protected void createEntitiesForPassenger()
        {
            _passenger1 = new Passenger {FirstName = "Vladimir", LastName = "Ivanov", Sex = "M"};
            _passenger2 = new Passenger {FirstName = "Denis", LastName = "Kycin", Sex = "M"};
            _passenger3 = new Passenger {FirstName = "Dmitriy", LastName = "Panfilov", Sex = "M"};
        }

        protected void checkAllPropertiesEqualForPassenger(Passenger entityToCheck1, Passenger entityToCheck2)
        {
            Assert.AreEqual(entityToCheck1.FirstName, entityToCheck2.FirstName, "Values must be equal");
            Assert.AreEqual(entityToCheck1.LastName, entityToCheck2.LastName, "Values must be equal");
            Assert.AreEqual(entityToCheck1.Sex, entityToCheck2.Sex, "Values must be equal");
        }
    }
}