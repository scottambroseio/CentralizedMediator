﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using CentralizedMediator.Core;

namespace CentralizedMediator.Tests
{
    [TestClass]
    public class Tests
    {
        [ClassInitialize]
        public static void Setup(TestContext ctx)
        {
            // Register specific mediators
            var centralizedMediator = Mediator.Instance;
            centralizedMediator.AddMediator(typeof(IRepositoryMediator<Entity>), RepositoryMediator<Entity>.Instance);
        }

        [TestMethod]
        public void Repository_Add_Generates_Event()
        {
            IEntity addedEntity = null;
            var entity = new Entity() { Id = 0 };

            var mediator = Mediator.Instance.GetMediator<IRepositoryMediator<Entity>>();
            var repo = new Repository<Entity>(mediator);

            mediator.EntityAdded += (s, e) => addedEntity = e.AddedEntity;

            repo.Add(entity);

            Assert.AreEqual(entity, addedEntity);
        }

        [TestMethod]
        public void Repository_Delete_Generates_Event()
        {
            IEntity deletedEntity = null;
            var entity = new Entity() { Id = 0 };

            var mediator = Mediator.Instance.GetMediator<IRepositoryMediator<Entity>>();
            var repo = new Repository<Entity>(mediator);

            mediator.EntityDeleted += (s, e) => deletedEntity = e.DeletedEntity;
            repo.Add(entity);

            repo.Delete(entity);

            Assert.AreEqual(entity, deletedEntity);
        }

        [TestMethod]
        public void Repository_Get_Generates_Event()
        {
            IEntity retrievedEntity = null;
            var entity = new Entity() { Id = 0 };

            var mediator = Mediator.Instance.GetMediator<IRepositoryMediator<Entity>>();
            var repo = new Repository<Entity>(mediator);

            mediator.EntityRetrieved += (s, e) => retrievedEntity = e.RetrievedEntity;
            repo.Add(entity);
            repo.Get(0);

            Assert.AreEqual(entity, retrievedEntity);
        }

        [TestMethod]
        public void Can_Subscribe_To_Multiple_Repositories()
        {
            var entity = new Entity();
            var invocationCount = 0;
            var mediator = Mediator.Instance.GetMediator<IRepositoryMediator<Entity>>();
            var repo1 = new Repository<Entity>(mediator);
            var repo2 = new Repository<Entity>(mediator);

            mediator.EntityAdded += (s, e) => invocationCount++;

            repo1.Add(entity);
            repo1.Add(entity);
            repo2.Add(entity);

            Assert.AreEqual(3, invocationCount);
        }

        [TestMethod]
        public void CacheHelper_Adds_New_Entites_To_Cache()
        {
            var mediator = Mediator.Instance.GetMediator<IRepositoryMediator<Entity>>();
            using (var cacheHelper = new FakeCacheHelper<Entity>(mediator))
            {
                cacheHelper.InitializeListeners();

                var entity = new Entity() { Id = 0 };
                var repo = new Repository<Entity>(mediator);

                repo.Add(entity);

                Assert.AreEqual(1, cacheHelper.Count);
            }
        }

        [TestMethod]
        public void CacheHelper_Removes_Entites_From_Cache()
        {
            var mediator = Mediator.Instance.GetMediator<IRepositoryMediator<Entity>>();
            using (var cacheHelper = new FakeCacheHelper<Entity>(mediator))
            {
                cacheHelper.InitializeListeners();

                var entity = new Entity() { Id = 0 };
                var repo = new Repository<Entity>(mediator);

                repo.Add(entity);
                Assert.AreEqual(1, cacheHelper.Count);

                repo.Delete(entity);
                Assert.AreEqual(0, cacheHelper.Count);
            }
        }
    }
}
