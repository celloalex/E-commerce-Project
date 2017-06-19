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
                HttpCookie cartCookie = filterContext.RequestContext.HttpContext.Request.Cookies["cart"];
                var cookieValues = cartCookie.Value.Split(',');
                int quantity = int.Parse(cookieValues[1]);
                filterContext.Controller.ViewBag.CartItemCount = quantity;
            }
        }

        //This happens before the controller method is called
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }
    }
}