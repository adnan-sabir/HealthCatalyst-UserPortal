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

namespace HealthCatalyst.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private Mock<IService<User>> _userServiceMock;
        private HomeController objController;
        private Mock<HomeController> _homeControllerMock;
        private List<User> listUser;
        private List<UserViewModel> listUserViewModel;

        [TestInitialize]
        public void Initialize()
        {
            this._userServiceMock = new Mock<IService<User>>();
            this.objController = new HomeController(_userServiceMock.Object);
            this._homeControllerMock = new Mock<HomeController>();
            listUser = new List<User>() {
               new User() { Id = 1, FirstName = "Mark", LastName = "Steyn", Address = "1234 Ridge Dr", Age = 56, Interests = "Soccer" , PictureFile = "user1.png" },
               new User() { Id = 2, FirstName = "Steve", LastName = "Johnson", Address = "5678 Cedar Dr", Age = 41, Interests = "Football" , PictureFile = "" },
               new User() { Id = 3, FirstName = "Tom", LastName = "Hanks", Address = "4321 Bayview circle", Age = 27, Interests = "Fishing" , PictureFile = "user3.png" }
            };
            listUserViewModel = new List<UserViewModel>() {
               new UserViewModel() { Id = 1, FirstName = "Mark", LastName = "Steyn", Address = "1234 Ridge Dr", Age = 56, Interests = "Soccer" , PictureFile = "user1.png" },
               new UserViewModel() { Id = 2, FirstName = "Steve", LastName = "Johnson", Address = "5678 Cedar Dr", Age = 41, Interests = "Football" , PictureFile = "" },
               new UserViewModel() { Id = 3, FirstName = "Tom", LastName = "Hanks", Address = "4321 Bayview circle", Age = 27, Interests = "Fishing" , PictureFile = "user3.png" }
            };
        }

        [TestMethod]
        public void GetAllUsers_ReturnsUserList()
        {
            //Arrange
            _userServiceMock.Setup(x => x.GetAll()).Returns(listUser);

            //Act
            var jsonResult = objController.GetAllUsers() as JsonResult;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<Result>(serializer.Serialize(jsonResult.Data));

            //Assert
            Assert.IsNotNull(jsonResult.Data);
            Assert.AreEqual(result.d.Count(), 3);
            Assert.AreEqual(result.d.ToList()[0].FirstName, listUserViewModel[0].FirstName); //Mark
            Assert.AreEqual(result.d.ToList()[1].FirstName, listUserViewModel[1].FirstName); //Steve
            Assert.AreEqual(result.d.ToList()[2].FirstName, listUserViewModel[2].FirstName); //Tom
            Assert.AreEqual(result.d.ToList()[0].LastName, listUserViewModel[0].LastName); //Steyn
            Assert.AreEqual(result.d.ToList()[1].LastName, listUserViewModel[1].LastName); //Johnson
            Assert.AreEqual(result.d.ToList()[2].LastName, listUserViewModel[2].LastName); //Hanks

            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.StatusText, "OK");
        }

        public class Result
        {
            public IEnumerable<UserViewModel> d;
            public int StatusCode;
            public string StatusText;
        }

        [TestMethod]
        public void GetAllUsers_ReturnsNoUserList()
        {
            //Arrange
            _userServiceMock.Setup(x => x.GetAll()).Returns(new List<User>());            

            //Act
            var result = (objController.GetAllUsers() as HttpStatusCodeResult);

            //Assert
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(result.StatusCode, 404);
            Assert.AreEqual(result.StatusDescription, "Users not found");
        }

        [TestMethod]
        public void GetSearchedUsers_ReturnsUserList()
        {
            //Arrange
            _userServiceMock.Setup(x => x.GetAll()).Returns(listUser);

            //Act
            var jsonResult = objController.GetSearchedUsers("tom") as JsonResult;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<Result>(serializer.Serialize(jsonResult.Data));

            //Assert
            Assert.IsNotNull(jsonResult.Data);
            Assert.AreEqual(result.d.Count(), 1);
            Assert.AreEqual(result.d.ToList()[0].FirstName, listUserViewModel[2].FirstName); //Tom
            Assert.AreEqual(result.d.ToList()[0].LastName, listUserViewModel[2].LastName); //Hanks

            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.StatusText, "OK");
        }

        [TestMethod]
        public void ValidUserPost_ReturnsUpdatedUserList()
        {
            //Arrange
            UserViewModel vm = new UserViewModel() { Id = 4, FirstName = "TestFirst", LastName = "TestLast", Address = "Test Address", Age = 16, Interests = "Test Interests", PictureFile = "Test.png" };
            listUserViewModel.Add(vm);
            User u = new User() { Id = 4, FirstName = "TestFirst", LastName = "TestLast", Address = "Test Address", Age = 16, Interests = "Test Interests", PictureFile = "Test.png" };
            listUser.Add(u);

            Mock<ControllerContext> cc = new Mock<ControllerContext>();
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("Test.png");
            cc.Setup(d => d.HttpContext.Request.Files[0]).Returns(file1.Object);
            objController.ControllerContext = cc.Object;
            objController.Skip = true;
            _userServiceMock.Setup(x => x.Add(u));
            _userServiceMock.Setup(x => x.GetAll()).Returns(listUser);            

            //Act
            var jsonResult = objController.Post(vm) as JsonResult;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<Result>(serializer.Serialize(jsonResult.Data));

            //Assert 
            Assert.IsNotNull(jsonResult.Data);
            Assert.AreEqual(result.d.Count(), 4);
            Assert.AreEqual(result.d.ToList()[3].FirstName, listUserViewModel[3].FirstName); //Tom
            Assert.AreEqual(result.d.ToList()[3].LastName, listUserViewModel[3].LastName); //Hanks

            Assert.AreEqual(result.StatusCode, 201);
            Assert.AreEqual(result.StatusText, "Created");
        }

        [TestMethod]
        public void InvalidUserPost_ReturnsBadRequestResponse()
        {
            //Arrange
            UserViewModel vm = new UserViewModel() { Id = 4, FirstName = "", LastName = "", Address = "Test Address", Age = 16, Interests = "Test Interests", PictureFile = "Test.png" };
            listUserViewModel.Add(vm);
            User u = new User() { Id = 4, FirstName = "", LastName = "", Address = "Test Address", Age = 16, Interests = "Test Interests", PictureFile = "Test.png" };
            listUser.Add(u);

            objController.ModelState.AddModelError("Error", "Missing/incorrect User data!");

            //Act
            var result = objController.Post(vm) as HttpStatusCodeResult;

            //Assert 
            _userServiceMock.Verify(m => m.Add(u), Times.Never);
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(result.StatusCode, 400);
            Assert.AreEqual(result.StatusDescription, "Missing/incorrect User data!");
        }

    }
}
