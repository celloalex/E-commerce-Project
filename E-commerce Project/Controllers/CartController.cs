using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_commerce_Project.Models;
//test comment
namespace E_commerce_Project.Controllers
{
    public class CartController : Controller
    {
        protected ECommerceDBEntities entities = new ECommerceDBEntities();

        protected override void Dispose(bool disposing)
        {
            entities.Dispose();
            base.Dispose(disposing);
        }

        // GET: Cart
        public ActionResult Index()
        {
            if (Request.Cookies.AllKeys.Contains("cart"))
            {
                HttpCookie cartCookie = Request.Cookies["cart"];
                //cartcookie comes in whtih "2,1" meaning productId 
                var cookieValues = cartCookie.Value.Split(',');
                int productId = int.Parse(cookieValues[0]);
                //int quantity = int.Parse(cookieValues[1]);
                var products = entities.Products.Where(x => x.ID == productId);
                return View(products);
            }

            return View();
        }

        //post: Cart
        [HttpPost]
        public ActionResult Index(Product[] model, int? quantity)
        {
            HttpCookie cartCookie = Request.Cookies["cart"];
            var cookieValues = cartCookie.Value.Split(',');
            int productId = int.Parse(cookieValues[0]);
            cartCookie.Value = productId + "," + quantity.Value;

            //if the user changes the number in the cart to zero this will expire the cookie so it no longer loads
            if (quantity == null || quantity.Value < 1)
            {
                cartCookie.Expires = DateTime.UtcNow;
            }

            Response.SetCookie(cartCookie);
            return RedirectToAction("Index");
        }
    }
}