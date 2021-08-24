using ECommerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerce_Project.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public int AddToCart(int productID, int quantity = 1)
        {
            Model m = new Model();
            string username = HttpContext.User.Identity.Name;
            User us = m.Users.FirstOrDefault(x => x.Username == username);
            Session userSession = m.Sessions.FirstOrDefault(x => x.UserID == us.ID);

            //add product to cart
            Product prod = m.Products.FirstOrDefault(x => x.ID == productID);
            Cart_Product isProductinCart = m.Cart_Product.FirstOrDefault(x => x.SessionID == userSession.ID && x.ProductID == prod.ID);
            //check same product in cart
            if (isProductinCart == null)
            {
                Cart_Product cart_Product = new Cart_Product()
                {
                    SessionID = userSession.ID,
                    ProductID = prod.ID,
                    ProductCount = quantity,
                    ProductPrice = prod.FinalPrice
                };
                //update total price in session
                userSession.TotalPrice += prod.FinalPrice * quantity;

                m.Cart_Product.Add(cart_Product);
            }
            else
            {
                isProductinCart.ProductCount += quantity;
                //update total price in session
                userSession.TotalPrice += prod.FinalPrice * quantity;

            }
            TempData["message"] = prod.Name + " sepetinize eklendi";
            m.SaveChanges();
            // return PartialView("ShoppingCart",userSession);
            return 1;
        }
        public Session GetCart()
        {
            Model m = new Model();
            string username = HttpContext.User.Identity.Name;
            //get current login user
            User user = m.Users.FirstOrDefault(x => x.Username == username);
            Session ses = m.Sessions.FirstOrDefault(x => x.UserID == user.ID);
            //List<Cart_Product> cart = m.Cart_Product.ToList();
            return ses;
        }
        public PartialViewResult ShoppingCart()
        {
            return PartialView(GetCart());
        }
        public ActionResult Checkout()
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Session ses = m.Sessions.FirstOrDefault(x => x.UserID == u.ID);
            // reset the cargoID when exiting or reloading checkout page
            ses.CargoID = null;
            m.SaveChanges();
            var test = (from x in m.Cargoes
                        select new
                        {
                            Text = x.CargoName + " --> " + x.Price + " ₺",
                            Value = x.ID.ToString()
                        }).ToList();
            ViewBag.cargo = new SelectList(test, "Value", "Text");
            List<Cargo> car = m.Cargoes.ToList();
            ViewBag.cargolist = car;
            return View(ses);
        }
        [HttpPost]
        public void SaveCargo(int cargoID)
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Session ses = m.Sessions.FirstOrDefault(x => x.UserID == u.ID);
            if(cargoID == 0)
            {
                ses.CargoID = null;
            }
            else
            {
                ses.CargoID = cargoID;
            }
            
            m.SaveChangesAsync();
        }
        [HttpPost]
        public int DeleteProduct(int id)
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Session ses = m.Sessions.FirstOrDefault(x => x.UserID == u.ID);
            Cart_Product cart_Product = ses.Cart_Product.FirstOrDefault(x => x.ProductID == id);
            Product prod = m.Products.FirstOrDefault(x => x.ID == id);
            //update price
            ses.TotalPrice -= prod.FinalPrice * cart_Product.ProductCount;
            //delete product from cart
            try
            {
                m.Cart_Product.Remove(cart_Product);
                m.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public ActionResult Payment()
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);

            return View(u);
        }
        [HttpPost]
        public ActionResult Payment(Payment p)
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Payment payment = m.Payments.Where(x => x.Address == p.Address & x.Ilce == p.Ilce & x.Sehir == p.Sehir & x.Type == p.Type).FirstOrDefault();
            if (payment == null)
            {
                m.Payments.Add(p);
                m.SaveChanges();
                payment = m.Payments.ToArray().LastOrDefault();
            }

            //craete order
            Session session = m.Sessions.FirstOrDefault(x => x.UserID == u.ID);
            Order order = new Order();
            order.TotalProductsPrice = session.TotalPrice;
            order.OrderDate = DateTime.Now;
            order.CargoID = (int)session.CargoID;
            order.UserID = u.ID;
            order.PaymentID = payment.ID;
            order.TotalPrice = session.TotalPrice + session.Cargo.Price;
            order.Status = "Kargoda";
            m.Orders.Add(order);
            m.SaveChanges();

            Order lastOrder = m.Orders.ToArray().LastOrDefault();
            foreach (Cart_Product cp in session.Cart_Product)
            {
                //add shopping cart items to order items
                Order_Product order_Product = new Order_Product();
                order_Product.OrderID = lastOrder.ID;
                order_Product.ProductID = cp.ProductID;
                order_Product.ProductCount = cp.ProductCount;
                order_Product.ProductPrice = cp.ProductPrice;
                m.Order_Product.Add(order_Product);

                //decrease product stock
                Product product = m.Products.FirstOrDefault(x => x.ID == cp.ProductID);
                product.Stock -= cp.ProductCount;
                m.SaveChanges();
            }
            //clear cart
            List<Cart_Product> cplist = m.Cart_Product.Where(x => x.SessionID == session.ID).ToList();
            m.Cart_Product.RemoveRange(cplist);
            m.SaveChanges();

            //clear session
            session.CargoID = null;
            session.TotalPrice = 0;
            m.SaveChanges();

            TempData["message"] = "Siparişiniz tamamlandı";
            return RedirectToAction("Orders", "Account");
        }
        [HttpPost]
        public void ProductCountUpdate(int productID, bool isIncrease)
        {
            Model m = new Model();
            User u = m.Users.FirstOrDefault(x => x.Username == HttpContext.User.Identity.Name);
            Session ses = m.Sessions.FirstOrDefault(x => x.UserID == u.ID);
            Cart_Product cart_Product = ses.Cart_Product.FirstOrDefault(x => x.ProductID == productID);
            if (isIncrease) // pressed + button from view. increase 1 product count in cart
            {
                cart_Product.ProductCount += 1;
                ses.TotalPrice += cart_Product.ProductPrice;
            }
            else // pressed - button from view. decrease 1 product count in cart
            {
                cart_Product.ProductCount -= 1;
                ses.TotalPrice -= cart_Product.ProductPrice;
            }
            m.SaveChangesAsync();
        }
    }
}