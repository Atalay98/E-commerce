using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce_Project.Models;

namespace ECommerce_Project.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(string name = "", int ID = 0)
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            List<Product> products = null;
            string titleName = "";
            if (ID != 0)
            {
                //get by category
                if (name == "category")
                {
                    products = m.Products.Where(x => x.CategoryID == ID).ToList();
                    titleName = m.Categories.FirstOrDefault(x => x.ID == ID).CategoryName + " ürünleri";
                }
                else//get by brand
                {
                    products = m.Products.Where(x => x.BrandID == ID).ToList();
                    titleName = m.Brands.FirstOrDefault(x => x.ID == ID).BrandName + " ürünleri";
                }
                //products = m.Products.ToList();
            }
            else
            {
                if (name == "") // get all products
                {
                    products = m.Products.ToList();
                }
                else // user search some products with giving name
                {
                    products = m.Products.Where(s => s.Name.Contains(name) || s.Description.Contains(name)).ToList();
                    titleName = "'" + name + "' ile ilgili ürünler";
                    ;
                }

            }
            List<Brand> bra = m.Brands.ToList();
            List<Category> cat = m.Categories.ToList();
            List<Favorite> fav = m.Favorites.Where(x => x.UserID == user.ID).ToList();
            Session ses = m.Sessions.FirstOrDefault(x => x.UserID == user.ID);
            ViewBag.brand = bra;
            ViewBag.category = cat;
            ViewBag.ses = ses;

            ViewBag.userfav = fav;//current user's favorites list
            if (products.Count == 0)
            {
                TempData["message"] = "Aradığınız kriterde ürün bulunamadı";
            }
            ViewBag.titleName = titleName;
            return View(products);
        }
        public ActionResult Detail(int id)
        {
            Model m = new Model();
            Product prod = m.Products.FirstOrDefault(x => x.ID == id);
            List<Review> review = m.Reviews.Where(x => x.ProductID == id).ToList();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Review userReview = m.Reviews.Where(x => x.ProductID == id && x.UserID == user.ID).FirstOrDefault();
            Session ses = m.Sessions.FirstOrDefault(x => x.UserID == user.ID);
            Cart_Product cp = m.Cart_Product.FirstOrDefault(x => x.SessionID == ses.ID && x.ProductID == id);

            int hasUserReview = 0;
            /*foreach(Review r in review)
            {
                if(r.UserID == user.ID)
                {
                    hasUserReview = 1;
                }
            }*/
            if (userReview != null)
            {
                hasUserReview = 1;
            }
            ViewBag.hasReview = hasUserReview;
            ViewBag.review = review;
            ViewBag.userReview = userReview;
            if (cp == null)
            {
                ViewBag.cp = 0;
            }
            else
            {
                ViewBag.cp = cp.ProductCount;
            }

            return View(prod);
        }
        [HttpPost]
        public ActionResult AddReview(Review form)
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Review hasReview = m.Reviews.FirstOrDefault(x => x.ProductID == form.ProductID && x.UserID == user.ID);
            if (hasReview == null) // user has no review so create new one
            {
                Review r = new Review();
                r.UserID = user.ID;
                r.ProductID = form.ProductID;
                r.Comment = form.Comment;
                r.Rating = form.Rating;
                r.Date = DateTime.Now;
                m.Reviews.Add(r);
            }
            else // user has review and update it
            {
                hasReview.Rating = form.Rating;
                hasReview.Comment = form.Comment;
                hasReview.Date = DateTime.Now;
            }

            m.SaveChanges();
            TempData["message"] = "Yorum kaydedildi";
            return RedirectToAction("Detail", new { id = form.ProductID });
        }
        [HttpPost]
        public int AddFav(int productID)
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Favorite fav = new Favorite();
            fav.ProductID = productID;
            fav.UserID = user.ID;
            m.Favorites.Add(fav);
            m.SaveChanges();
            return 1;
        }
        [HttpPost]
        public int RemoveFav(int productID)
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Favorite fav = m.Favorites.Where(x => x.ProductID == productID && x.UserID == user.ID).FirstOrDefault();
            m.Favorites.Remove(fav);
            m.SaveChanges();
            return 1;
        }
        [HttpPost]
        public ActionResult GetByCategory(int catid)
        {
            Model m = new Model();
            List<Product> productlist = m.Products.Where(x => x.CategoryID == catid).ToList();
            if (productlist == null)
            {
                TempData["message"] = "Bu kategoride ürün bulunamadı";
            }
            return RedirectToAction("Index", new { ID = catid });
        }
        [HttpPost]
        public ActionResult GetByBrand(int brandid)
        {
            Model m = new Model();
            List<Product> productlist = m.Products.Where(x => x.BrandID == brandid).ToList();
            if (productlist == null)
            {
                TempData["message"] = "Bu markada ürün bulunamadı";
            }
            return RedirectToAction("Index", productlist);
        }
        public JsonResult Search(string term)
        {
            Model m = new Model();
            List<string> products = m.Products.Where(s => (s.Name.Contains(term) || s.Description.Contains(term)) & s.isActive == true).Select(x => x.Name).ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchProducts(FormCollection form)
        {
            //Model m = new Model();
            //List<string> products = m.Products.Where(s => s.Name.Contains(form["term"])).Select(x => x.Name).ToList();
            return RedirectToAction("Index", new { name = form["term"] });
        }
        public ActionResult RemoveReview(int productid)
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Review r = m.Reviews.FirstOrDefault(x => x.ProductID == productid && x.UserID == user.ID);
            m.Reviews.Remove(r);
            m.SaveChanges();
            TempData["message"] = "Yorumunuz silindi";
            return RedirectToAction("Detail", new { id = productid });
        }
    }
}