using E_commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_commerce_Project
{
    public class CartCalculatorAttribute : FilterAttribute, IActionFilter
    {
        //This happens after the controller method is called creates cookie for the user to track what is in their cart
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Controller.ViewBag.CartItemCount = 0;
            
            if (filterContext.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("cart"))
            {
                using (ECommerceDBEntities e = new ECommerceDBEntities())
                {
                    HttpCookie cartCookie = filterContext.RequestContext.HttpContext.Request.Cookies["cart"];
                    var purchaseId = int.Parse(cartCookie.Value);
                    int quantity = e.Purchases.Single(x => x.ID == purchaseId).Purchase_Product.Sum(x => (x.Quantity ?? 0));
                    filterContext.Controller.ViewBag.CartItemCount = quantity;
                }
            }
        }

        //This happens before the controller method is called
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }
    }
}