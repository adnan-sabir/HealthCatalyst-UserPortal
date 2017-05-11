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

        public HomeController(IService<User> userService)
        {
            this._userService = userService;
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
                if (results.Count() > 0)
                {
                    //returns status code 200
                    return Json(new { d = results, StatusCode = (int)HttpStatusCode.OK, StatusText = "OK" }, JsonRequestBehavior.AllowGet);
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
                     users.Where(u => u.FirstName.ToLower().Contains(searchText.ToLower()) || u.LastName.ToLower().Contains(searchText.ToLower())).ToList();

                return Json(new { d = filteredUsers, StatusCode = (int)HttpStatusCode.OK, StatusText = "OK" }, JsonRequestBehavior.AllowGet);
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
                    user.PictureFile = UploadPostedFile(Request.Files[0]);

                    //Map ViewModel to domain model
                    Mapper.Initialize(m => {
                        m.CreateMap<UserViewModel, User>();
                    });
                    var userToAdd = Mapper.Map<UserViewModel, User>(user);

                    //Add to DB
                    this._userService.Add(userToAdd);
                    var results = GetUserList().ToList();

                    //returns status code 201
                    return Json(new { d = results, StatusCode = (int)HttpStatusCode.Created, StatusText = "Created" });
                }
                else
                {
                    //returns status code 400
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Missing/incorrect User data!");
                }
            }
            catch (Exception ex)
            {
                //returns status code 500
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error occured while adding new user: " + ex);
            }
        }

        public bool Skip;

        [NonAction]
        public string UploadPostedFile(HttpPostedFileBase PostedFile)
        {
            //Upload file.......................
            HttpPostedFileBase file = PostedFile;
            var fileName = PostedFile.FileName;
            var ext = Path.GetExtension(fileName);
            string name = Path.GetFileNameWithoutExtension(PostedFile.FileName);
            string myfile = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ext;
            
            if (!Skip)
            {
                //Save file
                string targetFolder = Server.MapPath("~/Content/Uploads");
                string targetPath = Path.Combine(targetFolder, myfile);
                file.SaveAs(targetPath);
            }

            return myfile;
        }

    }
}