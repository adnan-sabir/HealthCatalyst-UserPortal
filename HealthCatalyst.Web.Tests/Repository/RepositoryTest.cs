using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthCatalyst.DataAccess;
using HealthCatalyst.Domain.Data;
using HealthCatalyst.DataAccess.Repository;
using Moq;
using System.Data.Entity;

namespace HealthCatalyst.Web.Tests.Repository
{
    [TestClass]
    public class RepositoryTest
    {
        private Mock<IContext> _userContextMock;
        private IRepository<User> _userRepository;
        private List<User> listUser;

        [TestInitialize]
        public void Initialize()
        {
            _userContextMock = new Mock<IContext>();
            this._userRepository = new Repository<User>(_userContextMock.Object);
            listUser = new List<User>() {
               new User() { Id = 1, FirstName = "Mark", LastName = "Steyn", Address = "1234 Ridge Dr", Age = 56, Interests = "Soccer" , PictureFile = "user1.png" },
               new User() { Id = 2, FirstName = "Steve", LastName = "Johnson", Address = "5678 Cedar Dr", Age = 41, Interests = "Football" , PictureFile = "" },
               new User() { Id = 3, FirstName = "Tom", LastName = "Hanks", Address = "4321 Bayview circle", Age = 27, Interests = "Fishing" , PictureFile = "user3.png" }
            };
        }

        Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> list) where T : class, new()
        {
            IQueryable<T> queryableList = list.AsQueryable();
            Mock<DbSet<T>> dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(queryableList.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryableList.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryableList.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(queryableList.GetEnumerator());
            dbSetMock.Setup(x => x.Create()).Returns(new T());

            return dbSetMock;
        }

        [TestMethod]
        public void DataAccess_GetAllUsers_ReturnsUserList()
        {
            //Arrange
            var users = MockDbSet(listUser);
            _userContextMock.Setup(x => x.Set<User>()).Returns(users.Object);

            //Act
            var results = _userRepository.GetAll() as IEnumerable<User>;

            //Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 3);
            Assert.AreEqual(results.ToList()[0].FirstName, listUser[0].FirstName); //Mark
            Assert.AreEqual(results.ToList()[1].FirstName, listUser[1].FirstName); //Steve
            Assert.AreEqual(results.ToList()[2].FirstName, listUser[2].FirstName); //Tom
        }

        [TestMethod]
        public void DataAccess_AddNewUser()
        {
            //Arrange
            User u = new User() { Id = 4, FirstName = "TestFirst", LastName = "TestLast", Address = "Test Address", Age = 16, Interests = "Test Interests", PictureFile = "Test.png" };
            _userContextMock.Setup(m => m.Set<User>().Add(u));

            //Act
            _userRepository.Add(u);

            //Assert
            _userContextMock.Verify(m => m.Set<User>().Add(u), Times.Once);
            _userContextMock.Verify(m => m.SaveChanges(), Times.Once);
        }
    }
}
