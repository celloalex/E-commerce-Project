﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_commerce_Project.Models;

namespace E_commerce_Project.Controllers
{
    public class CategoryController : Controller
    {
        protected ECommerceDBEntities entities = new ECommerceDBEntities();

        protected override void Dispose(bool disposing)
        {
            entities.Dispose();
            base.Dispose(disposing);
        }

        // GET: Category
        public ActionResult Index(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Category = "All Products";
                return View(entities.Products);
            }
            else
            {
                ViewBag.Category = id;
                return View(entities.Products.Where(x => x.ProductTypeName == id));
            }
        }
    }
}