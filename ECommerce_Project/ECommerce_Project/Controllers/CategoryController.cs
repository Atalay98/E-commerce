using ECommerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerce_Project.Controllers
{
    [Authorize(Roles ="Customer, Seller")]
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            Model m = new Model();
            List<Category> cate = m.Categories.ToList();
            List<Product> prod = m.Products.ToList();
            ViewBag.products = prod;
            return View(cate);
        }
        public PartialViewResult Categories()
        {
            Model m = new Model();
            List<Category> cat = m.Categories.ToList();
            List<Brand> brand = m.Brands.ToList();
            ViewBag.brand = brand;
            return PartialView(cat);
        }
    }
}