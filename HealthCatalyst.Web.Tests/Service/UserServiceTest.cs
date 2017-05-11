using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthCatalyst.Web;
using HealthCatalyst.Web.Controllers;
using Moq;
using HealthCatalyst.Service.UserService;
using HealthCatalyst.Domain.Data;
using HealthCatalyst.Web.Models;
using System.Web;
using System.Net;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.IO;
using HealthCatalyst.DataAccess.Repository;

namespace HealthCatalyst.Web.Tests.Service
{
    [TestClass]
    public class UserServiceTest
    {
        private Mock<IRepository<User>> _userRepositoryMock;
        private IService<User> _userService;
        private List<User> listUser;

        [TestInitialize]
        public void Initialize()
        {
            _userRepositoryMock = new Mock<IRepository<User>>();
            this._userService = new UserService(_userRepositoryMock.Object);
            listUser = new List<User>() {
               new User() { Id = 1, FirstName = "Mark", LastName = "Steyn", Address = "1234 Ridge Dr", Age = 56, Interests = "Soccer" , PictureFile = "user1.png" },
               new User() { Id = 2, FirstName = "Steve", LastName = "Johnson", Address = "5678 Cedar Dr", Age = 41, Interests = "Football" , PictureFile = "" },
               new User() { Id = 3, FirstName = "Tom", LastName = "Hanks", Address = "4321 Bayview circle", Age = 27, Interests = "Fishing" , PictureFile = "user3.png" }
            };
        }

        [TestMethod]
        public void Service_GetAllUsers_ReturnsUserList()
        {
            //Arrange
            _userRepositoryMock.Setup(x => x.GetAll()).Returns(listUser);

            //Act
            var results = _userService.GetAll() as IEnumerable<User>;

            //Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 3);
        }

        [TestMethod]
        public void Service_AddNewUser()
        {
            //Arrange
            User u = new User() { Id = 4, FirstName = "TestFirst", LastName = "TestLast", Address = "Test Address", Age = 16, Interests = "Test Interests", PictureFile = "Test.png" };
            _userRepositoryMock.Setup(m => m.Add(u));

            //Act
            _userService.Add(u);

            //Assert
            _userRepositoryMock.Verify(m => m.Add(u), Times.Once);
        }
    }

}

