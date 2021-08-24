using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ECommerce_Project.Models;

namespace ECommerce_Project.Controllers
{
    public class SecurityController : Controller
    {
        // GET: Security
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(User us)
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == us.Username && x.Password == us.Password);
            if (u != null)//if username & password correct
            {
                if (u.IsActive == true) // if user is active
                {
                    FormsAuthentication.SetAuthCookie(u.Username, false);
                    if (u.RoleID == 2)
                    {
                        return RedirectToAction("Index", "Product");
                    }
                    return RedirectToAction("Index", "Admin");
                }
                else // if user is not active
                {
                    ViewBag.mesaj = "Kullanıcı admin tarafından engellendi";
                    return View();
                }
            }
            else
            {
                ViewBag.mesaj = "Kullanıcı adı veya parola hatalı";
                return View();
            }

        }

        public ActionResult LogOut()
        {
            /*string cookieName = FormsAuthentication.FormsCookieName;
            string userName = HttpContext.User.Identity.Name;
            HttpCookie c = HttpContext.Request.Cookies[cookieName];
            FormsAuthenticationTicket t = FormsAuthentication.Decrypt(c.Value);
            string un = t.Name;*/

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}