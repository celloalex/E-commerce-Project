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
using SendGrid;
using SendGrid.Helpers.Mail;
using E_commerce_Project.Web;


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
                //This begins transaction 
                var gateway = new Braintree.BraintreeGateway
                {
                    Environment = Braintree.Environment.SANDBOX,
                    MerchantId = ConfigurationManager.AppSettings["Braintree.MerchantId"],
                    PublicKey = ConfigurationManager.AppSettings["Braintree.PublicKey"],
                    PrivateKey = ConfigurationManager.AppSettings["Braintree.PrivateKey"]
                };
                //gathers information for transaction to send to braintree
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
                var cookieValues = cartCookie.Value.Split(',');
                int productId = int.Parse(cookieValues[0]);
                int quantity = int.Parse(cookieValues[1]);
                Product product = entities.Products.First(x => x.ID == productId);

                Braintree.TransactionRequest transaction = new Braintree.TransactionRequest();
                transaction.Amount = product.Price ?? 0;
                transaction.CustomerId = customerResult.Target.Id;
                transaction.PaymentMethodToken = customerResult.Target.CreditCards.First().Token;
                var transactionResult = await gateway.Transaction.SaleAsync(transaction);
                
                //new purchase object 
                Purchase p = new Purchase
                {
                    Customer = new Models.Customer
                    {
                        email = model.ContactEmail,
                    },
                    Purchase_Product = new Purchase_Product[]
                    {
                        new Purchase_Product
                        {
                            ProductID = product.ID,
                            Quantity = quantity
                        }
                    }
                };

                //SendGrid Send Emails out to people that place orders
                SendGridEmailService service = new SendGridEmailService(ConfigurationManager.AppSettings["SendGrid.ApiKey"]);
                await service.SendAsync(new Microsoft.AspNet.Identity.IdentityMessage
                {
                    Subject = string.Format("Your Raspberry Pi order has been placed"),
                    Destination = p.Customer.email,
                    Body = CreateReceiptEmail(p)
                });

                //Send SMS Messages out to people when they place their orders
                TwilioSmsService sms = new TwilioSmsService(
                    ConfigurationManager.AppSettings["Twilio.AccountSid"],
                    ConfigurationManager.AppSettings["Twilio.AuthToken"],
                    ConfigurationManager.AppSettings["Twilio.FromNumber"]);
                await sms.SendAsync(new Microsoft.AspNet.Identity.IdentityMessage
                {
                    Subject = "",
                    Destination = model.ContactPhone,
                    Body = "Your order has been placed " + p.Customer.Name + "! You will get shipping information shortly."//Customer name not sending with text messages
            });

            entities.Purchases.Add(p);
                //entities.SaveChanges();
                this.Response.SetCookie(new HttpCookie("cart") { Expires = DateTime.UtcNow }); // TEST THIS make sure cart is empty following purchase
                return RedirectToAction("Index", "Home", new { id = p.ID });
            }
            return View();
        }

        //TODO TODO TODO This still needs work and formating (needs to look presentable)
        //This is the body and structure of the email that the customer will receive
        private string CreateReceiptEmail(Purchase p)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<thead><tr><th></th><th>name</th><th>description</th><th>unit price</th><th>quantity</th><th>total price</th></tr></thead>");
            builder.Append("<tbody>");
            builder.Append("<tr><td></td>");
            builder.Append("<td>");
            builder.Append(p.Purchase_Product);
            builder.Append("</td>");

            builder.Append("<td>");
            foreach (var product in p.Purchase_Product)
            {
                builder.Append(p.Purchase_Product);// This should be the name of the product that is being purchased this can be fixed when you have a cart that can hold multiples
            }
            builder.Append("</td>");
            builder.Append("<td>");
            foreach (var product in p.Purchase_Product)
            {
                //builder.Append((product.Product.Price ?? 0).ToString("c"));// This should be the name of the product that is being purchased this can be fixed when you have a cart that can hold multiples
            }
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append(1);
            builder.Append("</td>");
            builder.Append("<td>");
            foreach (var product in p.Purchase_Product)
            {
                //builder.Append((product.Product.Price ?? 0)); // This should be the name of the product that is being purchased this can be fixed when you have a cart that can hold multiples
            }
            builder.Append("</td>");

            builder.Append("</tr>");
            builder.Append("</tbody><tfoot><tr><td colspan=\"5\">total</td><td>");
            // builder.append(p.purchaseproducts.sum(x => (x.product.price ?? 0) * x.quantity).tostring("c"));
            builder.Append("</td></tr></tfoot></table>");
            return builder.ToString();
        }
    }
}
