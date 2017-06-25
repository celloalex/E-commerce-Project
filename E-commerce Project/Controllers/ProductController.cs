using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_commerce_Project.Models;

namespace E_commerce_Project.Controllers
{
    public class ProductController : Controller
    {
        protected ECommerceDBEntities entities = new ECommerceDBEntities();

        protected override void Dispose(bool disposing)
        {
            entities.Dispose();
            base.Dispose(disposing);
        }
        // GET: Product
        public ActionResult Index(int? id)
        {
            if (!entities.Products.Any(x => x.ID == id))
            {
                return HttpNotFound("Product doesn't exist");
            }
            else
            {
                return View(entities.Products.First(x => x.ID == id));
            }
        }

        [HttpPost]
        public ActionResult Index(Product model, int? quantity)
        {
            //todo: add this product ot the current user's cart

            Purchase p = null;
            if (Request.Cookies.AllKeys.Contains("cart"))
            {
                int cartId = int.Parse(Request.Cookies["cart"].Value);
                p = entities.Purchases.FirstOrDefault(x => x.ID == cartId && x.CompletedDate == null);
            }
            if(p == null)
            {
                p = new Purchase();
                p.Customer = new Customer();
                entities.Purchases.Add(p);
                entities.SaveChanges();
                HttpCookie cookie = new HttpCookie("cart", p.ID.ToString());
                Response.SetCookie(cookie);
            }
            Purchase_Product pp = p.Purchase_Product.FirstOrDefault(x => x.ProductID == model.ID);
            if (pp == null)
            {
                pp = new Purchase_Product { ProductID = model.ID, Quantity = 0 };

                p.Purchase_Product.Add(pp);
            }

            pp.Quantity += quantity;
                
            entities.SaveChanges();
            

            return RedirectToAction("Index", "Cart");
        }
    }
}