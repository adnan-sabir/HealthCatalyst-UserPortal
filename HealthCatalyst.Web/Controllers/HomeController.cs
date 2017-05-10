using HealthCatalyst.Service.UserService;
using HealthCatalyst.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HealthCatalyst.Domain.Data;
using AutoMapper;
using HealthCatalyst.Web.Models;
using System.Net;
using System.Threading;
using System.IO;

namespace HealthCatalyst.Web.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        private IService<User> _userService;
        //private IRepository<User> _userRepository = new Repository();

        public HomeController(IService<User> userService)
        {
            this._userService = userService;
        }

        public HomeController()
        {
            this._userService = new UserService();
        }

        [Route("Index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("User/All")]
        public ActionResult GetAllUsers()
        {
            try
            {
                var results = GetUserList();
                if (results != null)
                {
                    //returns status code 200
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(results, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //returns status code 404
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Users not found");
                }
            }
            catch (Exception ex)
            {
                //returns status code 500
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error occured while getting all user: " + ex);
            }
        }

        private IEnumerable<UserViewModel> GetUserList()
        {
            var users = this._userService.GetAll();

            string targetFolder = "../../Content/Uploads";
            string emptyFile = "empty.jpg";

            Mapper.Initialize(u => {
                u.CreateMap<User, UserViewModel>()
                    .AfterMap((s, d) => d.PictureFile = Path.Combine(targetFolder, string.IsNullOrEmpty(s.PictureFile) ? emptyFile : s.PictureFile));
            });
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users);
        }

        [HttpGet]
        [Route("User/Search")]
        public ActionResult GetSearchedUsers(string searchText = "")
        {
            try
            {
                Thread.Sleep(2000);
                var users = GetUserList();
                var filteredUsers = String.IsNullOrEmpty(searchText) ? users :
                     users.Where(u => u.FirstName.ToLower().Contains(searchText) || u.LastName.ToLower().Contains(searchText)).ToList();

                if (filteredUsers != null)
                {
                    //returns status code 200
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(filteredUsers, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //returns status code 404
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "User not found with search text: " + searchText);
                }
            }
            catch (Exception ex)
            {
                //returns status code 500
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error occured while getting user's data: " + ex);
            }
        }

        [HttpPost]
        [Route("User/Add")]
        public ActionResult Post(UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {                  
                    //Upload file.......................
                    HttpPostedFileBase file = Request.Files[0];
                    var fileName = Request.Files[0].FileName; 
                    var ext = Path.GetExtension(fileName); 
                    string name = Path.GetFileNameWithoutExtension(Request.Files[0].FileName); 
                    string myfile = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ext;
                    //Save file
                    string targetFolder = Server.MapPath("~/Content/Uploads");
                    string targetPath = Path.Combine(targetFolder, myfile);
                    file.SaveAs(targetPath);
                    
                    user.PictureFile = myfile;

                    //Map ViewModel to domain model
                    Mapper.Initialize(m => {
                        m.CreateMap<UserViewModel, User>();
                    });
                    var userToAdd = Mapper.Map<UserViewModel, User>(user);

                    //Add to DB
                    this._userService.Add(userToAdd);
                    var results = GetUserList().ToList();

                    //returns status code 201
                    Response.StatusCode = (int)HttpStatusCode.Created;
                    return Json( new { d = results, statusText = "Created" });
                }
                else
                {
                    //returns status code 400
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Missing/incorrect User data!");
                }
            }
            catch (Exception ex)
            {
                //returns status code 400
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error occured while adding new user: " + ex);
                //return Json(new { Success = false, Message = ex.Message });
            }
        }

    }
}