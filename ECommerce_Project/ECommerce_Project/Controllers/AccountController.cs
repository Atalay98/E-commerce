using ECommerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ECommerce_Project.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(FormCollection form)
        {
            Model m = new Model();
            User newUser = new User();
            newUser.Username = form["username"].ToString();
            newUser.Name = form["Name"].ToString();
            newUser.Surname = form["Surname"].ToString();
            newUser.Mail = form["email"].ToString();
            newUser.Password = form["password"].ToString();
            string role = form["role"].ToString();
            if (role == "customer")
            {
                newUser.RoleID = 2;
            }
            if (role == "seller")
            {
                newUser.RoleID = 3;
            }
            newUser.IsActive = true;
            newUser.RegisterDate = DateTime.Now;
            try
            {
                m.Users.Add(newUser);
                m.SaveChanges();

                //create new session
                User test = m.Users.FirstOrDefault(x => x.Username == newUser.Username);
                Session newSession = new Session
                {
                    UserID = test.ID,
                    TotalPrice = 0,
                    CreationDate = DateTime.Now
                };
                m.Sessions.Add(newSession);
                m.SaveChanges();

                return RedirectToAction("Login", "Security");
            }
            catch (Exception)
            {
                return Register();
            }
        }
        [Authorize]
        public ActionResult UserProfile()
        {
            Model m = new Model();
            string username = HttpContext.User.Identity.Name;
            User user = m.Users.FirstOrDefault(x => x.Username == username);
            List<ilceler> bra = m.ilcelers.ToList();
            List<iller> cat = m.illers.ToList();
            ViewBag.ilce = bra;
            ViewBag.il = cat;
            return View(user);

        }
        [Authorize]
        public ActionResult EditProfile()
        {
            Model m = new Model();
            string username = HttpContext.User.Identity.Name;
            User user = m.Users.FirstOrDefault(x => x.Username == username);
            List<ilceler> bra = m.ilcelers.ToList();
            List<iller> cat = m.illers.ToList();
            ViewBag.ilce = bra;
            ViewBag.il = cat;
            return View(user);

        }
        [Authorize]
        [HttpPost]
        public ActionResult EditProfile(User u)
        {
            Model m = new Model();
            User updateUser = m.Users.FirstOrDefault(x => x.ID == u.ID);
            updateUser.Birthday = u.Birthday;
            updateUser.Gender = u.Gender;
            updateUser.Telephone = u.Telephone;
            updateUser.Mail = u.Mail;
            updateUser.IlID = u.IlID;
            updateUser.IlceID = u.IlceID;
            m.SaveChanges();

            return RedirectToAction("UserProfile");

        }
        [AllowAnonymous]
        public JsonResult checkUsernameTaken(string name)
        {
            Model m = new Model();
            User user = m.Users.Where(x => x.Username == name).FirstOrDefault();
            if (user != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult ilceGetir(int ilId)
        {
            Model m = new Model();
            //var test = m.ilcelers.Where(x => x.sehir == ilId).ToList();
            if (ilId == 0)
            {
                var test = (from x in m.ilcelers
                            select new
                            {
                                Text = x.ilce,
                                Value = x.id.ToString()
                            }).ToList();
                return Json(test, JsonRequestBehavior.AllowGet);
            }
            var deneme = (from x in m.ilcelers
                          join y in m.illers on x.sehir equals y.id
                          where x.sehir == ilId
                          select new
                          {
                              Text = x.ilce,
                              Value = x.id.ToString()
                          }).ToList();

            return Json(deneme, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Customer")]
        public ActionResult Orders()
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            List<Order> orders = m.Orders.Where(x => x.UserID == u.ID).ToList();
            return View(orders);
        }
        [Authorize(Roles = "Customer")]
        public ActionResult OrderDetail(int id)
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Order order = m.Orders.FirstOrDefault(x => x.ID == id);
            return View(order);
        }
        [Authorize(Roles = "Customer")]
        public ActionResult Favorites()
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            List<Favorite> favList = m.Favorites.Where(x => x.UserID == user.ID).ToList();
            List<Product> products = new List<Product>();
            foreach(Favorite f in favList)
            {
                products.Add(m.Products.FirstOrDefault(x => x.ID == f.ProductID));
            }
            return View(products);
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public int DeleteFav(int id)
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Favorite favorite = m.Favorites.FirstOrDefault(x => x.ProductID == id && x.UserID == user.ID);
            m.Favorites.Remove(favorite);
            m.SaveChanges();
            return 1;
        }
        [Authorize(Roles = "Customer, Seller")]
        public ActionResult SendFeedback()
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Feedback f = new Feedback();
            ViewBag.user = user;
            return View(f);
        }
        [HttpPost]
        [Authorize(Roles = "Customer, Seller")]
        public ActionResult SendFeedback(Feedback f)
        {
            Model m = new Model();
            f.Date = DateTime.Now;
            m.Feedbacks.Add(f);
            m.SaveChanges();
            TempData["message"] = "Mesajınız iletildi";
            if (f.User.Role.RoleName == "Customer")
            {
                return RedirectToAction("Index", "Product");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            
        }
    }
}