using ECommerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ECommerce_Project.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        [Authorize(Roles = "Admin,Seller")]
        public ActionResult Index()
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            List<Product> pro = m.Products.ToList();
            List<Category> cat = m.Categories.ToList();
            List<Brand> bra = m.Brands.ToList();
            ViewBag.brand = bra;
            ViewBag.category = cat;
            ViewBag.user = u.ID;
            return View(pro);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Seller")]
        public void DeleteProduct(int id)
        {
            Model m = new Model();
            Product prod = m.Products.FirstOrDefault(x => x.ID == id);
            try
            {
                prod.isActive = false;
                m.SaveChanges();
            }
            catch (Exception)
            {
            }
        }
        [Authorize(Roles = "Admin, Seller")]
        public ActionResult AddProduct()
        {
            Model m = new Model();
            Product prod = new Product();
            List<Category> cat = m.Categories.ToList();
            List<Brand> bra = m.Brands.ToList();
            ViewBag.brand = bra;
            ViewBag.category = cat;
            var test = (from x in m.Campaigns
                        where x.Active == true
                        select new
                        {
                            Text = x.CampaignName + " --> %" + x.DiscountPercent + " indirim",
                            Value = x.ID.ToString()
                        }).ToList();
            ViewBag.campaign = new SelectList(test, "Value", "Text");
            return View(prod);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Seller")]
        public ActionResult AddProduct(Product u)
        {
            Model m = new Model();
            Product prod = m.Products.FirstOrDefault(x => x.ID == u.ID);
           
            if (prod == null)
            {
                u.CreationDate = DateTime.Now;
                u.isActive = true;

                //set owner id to product
                string cookieName = FormsAuthentication.FormsCookieName;
                string userName = HttpContext.User.Identity.Name;
                User us = m.Users.FirstOrDefault(x => x.Username == userName);
                u.OwnerID = us.ID;
                //if campaign not selected set it to null
                if (u.CampaignID == 0)
                {
                    u.CampaignID = null;
                    u.FinalPrice = u.Price;
                }
                else//if campaign select apply discount
                {
                    Campaign campaign = m.Campaigns.FirstOrDefault(x => x.ID == u.CampaignID);
                    u.CampaignID = campaign.ID;
                    u.FinalPrice = u.Price - (u.Price * campaign.DiscountPercent / 100);
                }

                m.Products.Add(u);
                m.SaveChanges();
            }
            else
            {
                prod.Name = u.Name;
                prod.Price = u.Price;
                //if campaign not selected set it to null
                if (u.CampaignID == 0)
                {
                    u.CampaignID = null;
                    u.FinalPrice = u.Price;
                }
                else//if campaign select apply discount
                {
                    Campaign campaign = m.Campaigns.FirstOrDefault(x => x.ID == u.CampaignID);
                    u.CampaignID = campaign.ID;
                    u.FinalPrice = u.Price - (u.Price * campaign.DiscountPercent / 100);
                }
                prod.Picture = u.Picture;
                prod.Stock = u.Stock;
                prod.Description = u.Description;
                prod.CampaignID = u.CampaignID;
                prod.FinalPrice = u.FinalPrice;
                m.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin, Seller")]
        public ActionResult UpdateProduct(int id)
        {
            Model m = new Model();
            Product prod = m.Products.FirstOrDefault(x => x.ID == id);
            List<Category> cat = m.Categories.ToList();
            List<Brand> bra = m.Brands.ToList();
            ViewBag.brand = bra;
            ViewBag.category = cat;
            var test = (from x in m.Campaigns
                        where x.Active == true
                        select new
                        {
                            Text = x.CampaignName + " --> %" + x.DiscountPercent + " indirim",
                            Value = x.ID.ToString()
                        }).ToList();
            ViewBag.campaign = new SelectList(test, "Value", "Text");
            return View("AddProduct", prod);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Users()
        {
            Model m = new Model();
            List<User> userlist = m.Users.ToList();
            List<Role> rolelist = m.Roles.ToList();
            ViewBag.role = rolelist;
            return View(userlist);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void BanUser(int id)
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.ID == id);
            try
            {
                user.IsActive = false;
                TempData["message"] = "Kullanıcı engellendi";
                m.SaveChanges();
            }
            catch (Exception)
            {
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void UnbanUser(int id)
        {
            Model m = new Model();
            User user = m.Users.FirstOrDefault(x => x.ID == id);
            try
            {
                user.IsActive = true;
                TempData["message"] = "Kullanıcı engeli kaldırıldı";
                m.SaveChanges();
            }
            catch (Exception)
            {
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Category()
        {
            Model m = new Model();
            List<Category> categorylist = m.Categories.ToList();
            List<Product> productlist = m.Products.ToList();
            List<Brand> brandlist = m.Brands.ToList();
            ViewBag.brand = brandlist;
            ViewBag.product = productlist;
            return View(categorylist);
        }
        /*[Authorize(Roles = "Admin")]
        public ActionResult AddCategory()
        {
            Category cat = new Category();
            return View(cat);
        }*/
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddCategory(Category u)
        {
            Model m = new Model();
            m.Categories.Add(u);
            m.SaveChangesAsync();
            return RedirectToAction("Category");
        }
        /*[Authorize(Roles = "Admin")]
        public ActionResult AddBrand()
        {
            Brand cat = new Brand();
            return View(cat);
        }*/
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddBrand(Brand u)
        {
            Model m = new Model();
            m.Brands.Add(u);
            m.SaveChangesAsync();
            return RedirectToAction("Category");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Orders()
        {
            Model m = new Model();
            List<Order> orders = m.Orders.ToList();
            return View(orders);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public int ChangeOrderStatus(int orderID, string status)
        {
            Model m = new Model();
            Order order = m.Orders.FirstOrDefault(x => x.ID == orderID);
            order.Status = status;
            if(status == "İptal") // if order is canceled, increase the product stock
            {
                foreach(Order_Product op in order.Order_Product)
                {
                    Product p = m.Products.FirstOrDefault(x => x.ID == op.ProductID);
                    p.Stock += op.ProductCount;
                }
            }
            m.SaveChanges();
            return 1;
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ShowFeedback()
        {
            Model m = new Model();
            List<Feedback> feedbacks = m.Feedbacks.ToList();
            return View(feedbacks);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Campaigns()
        {
            Model m = new Model();
            List<Campaign> campaigns = m.Campaigns.ToList();
            return View(campaigns);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddCampaign(Campaign c)
        {
            Model m = new Model();
            Campaign campaigns = m.Campaigns.FirstOrDefault(x=>x.ID == c.ID);
            if(campaigns == null)
            {
                c.Active = true;
                m.Campaigns.Add(c);
            }
            else
            {
                campaigns.CampaignName = c.CampaignName;
                campaigns.DiscountPercent = c.DiscountPercent;
            }
            m.SaveChanges();
            return RedirectToAction("Campaigns");
        }
    }
}