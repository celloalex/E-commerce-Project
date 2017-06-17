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
            HttpCookie cookie = new HttpCookie("cart", model.ID.ToString() + ", " + quantity.Value.ToString());
            Response.SetCookie(cookie);

            return RedirectToAction("Index", "Cart");
        }
    }
}