using System;
using System.Collections.Generic;
using jaslab6;
using NHibernate;
using NHibernate.Criterion;
using NUnit.Framework;

namespace lab6Test
{
    [TestFixture]
    public abstract class TestGenericDAO<T> where T : EntityBase
    {
        protected static ISession Session { get; set; }

        protected IGenericDAO<T> Dao { get; set; }
        protected T entity1;
        protected T entity2;
        protected T entity3;

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            Session.Close();
        }

        [SetUp]
        public void TestInitialize()
        {
            Assert.IsNotNull(Dao, "Please, provide IGenericDAO implementation in constructor");
            createEntities();
            
            Assert.IsNotNull(entity1, "Please, create object for entity1");
            Assert.IsNotNull(entity2, "Please, create object for entity2");
            Assert.IsNotNull(entity3, "Please, create object for entity3");
            
            checkAllPropertiesDiffer(entity1, entity2);
            checkAllPropertiesDiffer(entity1, entity3);
            checkAllPropertiesDiffer(entity2, entity3);

            saveEntitiesGeneric();
        }

        [TearDown]
        public void TestCleanup()
        {
            try
            {
                if ((entity1 = Dao.GetById(entity1.Id)) != null)
                    Dao.Delete(entity1);
            }
            catch (Exception e)
            {
                Assert.Fail($"Problem in cleanup method: {e.Message}");
            }

            try
            {
                if ((entity2 = Dao.GetById(entity2.Id)) != null)
                    Dao.Delete(entity2);
            }
            catch (Exception e)
            {
                Assert.Fail($"Problem in cleanup method: {e.Message}");
            }

            try
            {
                if ((entity3 = Dao.GetById(entity3.Id)) != null)
                    Dao.Delete(entity3);
            }
            catch (Exception e)
            {
                Assert.Fail($"Problem in cleanup method: {e.Message}");
            }

            entity1 = null;
            entity2 = null;
            entity3 = null;
        }

        [Test]
        public void TestGetByIdGeneric()
        {
            // Should not find with inexistent id
            try
            {
                var id = (int) DateTime.Now.ToFileTime();
                var foundObject = Dao.GetById(id);
                Assert.IsNull(foundObject, "Should return null if id is inexistent");
            }
            catch (Exception e)
            {
                Assert.Fail($"Should return null if object not found: {e.Message}");
            }

            // Getting all three entities
            getEntityGeneric(entity1.Id, entity1);
            getEntityGeneric(entity2.Id, entity2);
            getEntityGeneric(entity3.Id, entity3);
        }

        [Test]
        public void TestGetAllGeneric()
        {
            var list = getListOfAllEntities();
            Assert.IsTrue(list.Contains(entity1), "After dao method GetAll list should contain entity1");
            Assert.IsTrue(list.Contains(entity2), "After dao method GetAll list should contain entity2");
            Assert.IsTrue(list.Contains(entity3), "After dao method GetAll list should contain entity3");
        }

        [Test]
        public void TestDeleteGeneric()
        {
            try
            {
                Dao.Delete(null);
                Assert.Fail("Should not delete entity will null id");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Deleting second entity
            try
            {
                Dao.Delete(entity2);
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
                var foundEntity = Dao.GetById(entity2.Id);
                Assert.IsNull(foundEntity, "After deletion entity should not be found with id " + entity2.Id);
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

        protected abstract void createEntities();

        protected abstract void checkAllPropertiesDiffer(T entityToCheck1, T entityToCheck2);

        protected abstract void checkAllPropertiesEqual(T entityToCheck1, T entityToCheck2);

        protected void saveEntitiesGeneric()
        {
            try
            {
                Dao.SaveOrUpdate(entity1);
                T savedObject = getPersistentObject(entity1);

                Assert.IsNotNull(savedObject, "DAO method saveOrUpdate should return entity if successful");
                checkAllPropertiesEqual(savedObject, entity1);
                entity1 = savedObject;
            }
            catch (Exception e)
            {
                Assert.Fail($"Fail to save entity1: {e.Message}");
            }

            try
            {
                Dao.SaveOrUpdate(entity2);
                T savedObject = getPersistentObject(entity2);

                Assert.IsNotNull(savedObject, "DAO method saveOrUpdate should return entity if successful");
                checkAllPropertiesEqual(savedObject, entity2);
                entity2 = savedObject;
            }
            catch (Exception e)
            {
                Assert.Fail($"Fail to save entity2: {e.Message}");
            }

            try
            {
                Dao.SaveOrUpdate(entity3);
                T savedObject = getPersistentObject(entity3);
                Assert.IsNotNull(savedObject, "DAO method saveOrUpdate should return entity if successful");
                checkAllPropertiesEqual(savedObject, entity3);
            }
            catch (Exception e)
            {
                Assert.Fail($"Fail to save entity3: {e.Message}");
            }
        }

        protected T getPersistentObject(T nonPersistentObject)
        {
            var criteria = Session.CreateCriteria(typeof(T)).Add(Example.Create(nonPersistentObject));
            var list = criteria.List<T>();
            Assert.IsTrue(list.Count >= 1, "Count must be equal or more than 1");
            return list[0];
        }

        protected void getEntityGeneric(int id, T entity)
        {
            try
            {
                var foundEntity = Dao.GetById(id);
                Assert.IsNotNull(foundEntity, "Service method getEntity should return entity if successful");
                checkAllPropertiesEqual(foundEntity, entity);
            }
            catch (Exception e)
            {
                Assert.Fail($"Failed to get entity with id {id}: {e.Message}");
            }
        }

        protected IList<T> getListOfAllEntities()
        {
            IList<T> list = null;

            // Should get not null and not empty list
            try
            {
                list = Dao.GetAll();
            }
            catch (Exception e)
            {
                Assert.Fail($"Should be able to get all entities that were added before: {e.Message}");
            }

            Assert.IsNotNull(list, "DAO method GetAll should return list of entities if successful");
            Assert.IsFalse(list.Count == 0, "DAO method should return not empty list if successful");
            return list;
        }
    }
}