using abhinav_core.datafolder;
using abhinav_core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace abhinav_core.Controllers
{
    public class HomeController : Controller
    {
        abhiContext dbobj = new abhiContext();
        logmodalClass mobj = new logmodalClass();
        Login1 tobj = new Login1();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login1()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login1(logmodalClass mobj)
        {
            var userLogin1 = dbobj.Login1s.Where(a => a.Email == mobj.Email).FirstOrDefault();
            if (userLogin1 == null)
            {
                TempData["invalid"] = "Invalid Email !!";
            }
            else
            {
                if (userLogin1.Email == mobj.Email && userLogin1.Pass == mobj.Pass)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, userLogin1.Name),
                                        new Claim(ClaimTypes.Email, userLogin1.Email) };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties);


                    HttpContext.Session.SetString("Name", userLogin1.Name);
                  // HttpContext.Session.GetString("Name");

                    return RedirectToAction("show");
                }
                else
                {
                    TempData["wrong"] = "Wrong Password !!";
                    return View();
                }
            }


            return View();
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login1");
        }


          [Authorize]
        public IActionResult show()
        {
           
            List<logmodalClass> lobj = new List<logmodalClass>();

            var res = dbobj.Login1s.ToList();
            foreach (var item in res)
            {
                lobj.Add(new logmodalClass
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Pass = item.Pass
                  
                });
            }
            return View(lobj);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(logmodalClass mobj )
        {
          
            
            tobj.Id = mobj.Id;
            tobj.Name = mobj.Name;
            tobj.Email = mobj.Email;
            tobj.Pass = mobj.Pass;
           

            if (mobj.Id == 0)
            {
                dbobj.Login1s.Add(tobj);
                dbobj.SaveChanges();
            }
            else
            {
                dbobj.Entry(tobj).State = EntityState.Modified;
                dbobj.SaveChanges();
            }

            return RedirectToAction("show");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            
            var edititem = dbobj.Login1s.Where(a => a.Id == id).First();
            mobj.Id = edititem.Id;
            mobj.Name = edititem.Name;
            mobj.Email = edititem.Email;
            mobj.Pass   = edititem.Pass;
           

            return View("Add", mobj);
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
           
            var deleteitem = dbobj.Login1s.Where(a => a.Id == id).First();
            dbobj.Login1s.Remove(deleteitem);
            dbobj.SaveChanges();
            return RedirectToAction("show");
        }



        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
