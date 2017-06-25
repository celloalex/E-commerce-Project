using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_commerce_Project.Models;
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
                int purchaseId = int.Parse(cartCookie.Value);
                var purchase = entities.Purchases.Single(x => x.ID == purchaseId);
                return View(purchase);
            }
            return View();
        }

        //post: Cart
        [HttpPost]
        public ActionResult Index(Purchase model)
        {
            var fromDb = entities.Purchases.Single(x => x.ID == model.ID);
            foreach (var updatedProduct in model.Purchase_Product)
            {
                var productInDb = fromDb.Purchase_Product.FirstOrDefault(x => x.ProductID == updatedProduct.ProductID);
                productInDb.Quantity = updatedProduct.Quantity ?? 0; 

            }
            entities.Purchase_Product.RemoveRange(fromDb.Purchase_Product.Where(x => x.Quantity == 0));
            entities.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

