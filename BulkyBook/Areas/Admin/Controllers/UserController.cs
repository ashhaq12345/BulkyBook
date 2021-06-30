using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_User_Comp)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var obj = _db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (obj != null)
            {
                if (obj.LockoutEnd != null && obj.LockoutEnd > DateTime.Now)
                {
                    obj.LockoutEnd = DateTime.Now;
                }
                else
                {
                    obj.LockoutEnd = DateTime.Now.AddYears(1000);
                }
                _db.SaveChanges();
                return Json(new { success = true, message = "Successfully Changed" });

            }
            return Json(new { success = false, message = "error while locking unclikng" });
        }



        #endregion
    }
}