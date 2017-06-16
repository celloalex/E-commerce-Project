using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using E_commerce_Project.Models;
using Braintree;

namespace E_commerce_Project.Controllers
{
    public class CheckoutController : Controller
    {
        protected ECommerceDBEntities entities = new ECommerceDBEntities();
        // GET: Checkout
        public ActionResult Index()
        {
            if (!Request.Cookies.AllKeys.Contains("cart"))//when referenceing cookie (case sensitive)
                return RedirectToAction("Index", "Cart");//when referencing the controller (not necessary) easier to distinguish
            CheckoutModel model = new CheckoutModel();
            return View(model);
        }

        // POST: Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(CheckoutModel model, string paymentMethodNonce)
        {
            //Check if the model-state is valid -- this will catch anytime someone hacks your client-side validation
            if (ModelState.IsValid)
            {
                var gateway = new Braintree.BraintreeGateway
                {
                    Environment = Braintree.Environment.SANDBOX,
                    MerchantId = ConfigurationManager.AppSettings["Braintree.MerchantId"],
                    PublicKey = ConfigurationManager.AppSettings["Braintree.PublicKey"],
                    PrivateKey = ConfigurationManager.AppSettings["Braintree.PrivateKey"]
                };

                Braintree.CustomerRequest customer = new Braintree.CustomerRequest();
                customer.Email = model.ContactEmail;
                customer.Phone = model.ContactPhone;
                customer.CreditCard = new Braintree.CreditCardRequest();
                customer.CreditCard.Number = model.CreditCardNumber;
                customer.CreditCard.CVV = model.CreditCardVerificationValue;
                customer.CreditCard.ExpirationMonth = model.CreditCardExpirationMonth.ToString().PadLeft(2, '0');
                customer.CreditCard.ExpirationYear = model.CreditCardExpirationYear.ToString();
                customer.CreditCard.CardholderName = model.CreditCardName;
                var customerResult = await gateway.Customer.CreateAsync(customer);

                HttpCookie cartCookie = Request.Cookies["cart"];
                //cartcookie comes in whtih "2,1" meaning productId 
                var cookieValues = cartCookie.Value.Split(',');
                int productId = int.Parse(cookieValues[0]);
                int quantity = int.Parse(cookieValues[1]);
                Product product = entities.Products.First(x => x.ID == productId);



                Braintree.TransactionRequest transaction = new Braintree.TransactionRequest();
                transaction.Amount = product.Price ?? 0;
                transaction.CustomerId = customerResult.Target.Id;
                transaction.PaymentMethodToken = customerResult.Target.CreditCards.First().Token;
                var transactionResult = await gateway.Transaction.SaleAsync(transaction);



                Purchase p = new Purchase
                {
                    Customer = new Models.Customer
                    {
                        email = model.ContactEmail
                    },
                    date = DateTime.UtcNow,
                    ProductID = product.ID,

                };


                entities.Purchases.Add(p);

                entities.SaveChanges();

                this.Response.SetCookie(new HttpCookie("cart") { Expires = DateTime.UtcNow });


                return RedirectToAction("Index", "Receipt", new { id = p.ID });
            };
            return View();

        }


        private static string CreateReceiptEmail(Purchase p)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<thead><tr><th></th><th>Name</th><th>Description</th><th>Unit Price</th><th>Quantity</th><th>Total Price</th></tr></thead>");
            builder.Append("<tbody>");
            builder.Append("<tr><td></td>");
            builder.Append("<td>");
            builder.Append(p.Product.Name);
            builder.Append("</td>");

            builder.Append("<td>");
            builder.Append(p.Product.Review);
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append((p.Product.Price ?? 0).ToString("c"));
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append(1);
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append((p.Product.Price ?? 0));
            builder.Append("</td>");

            builder.Append("</tr>");
            builder.Append("</tbody><tfoot><tr><td colspan=\"5\">Total</td><td>");
            // builder.Append(p.PurchaseProducts.Sum(x => (x.Product.Price ?? 0) * x.Quantity).ToString("c"));
            builder.Append("</td></tr></tfoot></table>");
            return builder.ToString();
        }

    }
}