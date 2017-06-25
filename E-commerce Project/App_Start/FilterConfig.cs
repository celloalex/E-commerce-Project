using System.Web;
using System.Web.Mvc;
using E_commerce_Project;


namespace E_commerce_Project
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CartCalculatorAttribute()); //Cart calculator/tracker will now be run on every page!
        }
    }
}
