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
                int purchaseId = int.Parse(cartCookie.Value);
                Purchase p = entities.Purchases.Single(x => x.ID == purchaseId);

                Braintree.TransactionRequest transaction = new Braintree.TransactionRequest();
                transaction.Amount = p.Purchase_Product.Sum(x => ((x.Quantity ?? 0) * (x.Product.Price ?? 0)));
                transaction.CustomerId = customerResult.Target.Id;
                transaction.PaymentMethodToken = customerResult.Target.CreditCards.First().Token;
                var transactionResult = await gateway.Transaction.SaleAsync(transaction);

                p.Customer.email = model.ContactEmail;
                p.Customer.Name = model.CreditCardName;
                p.CompletedDate = DateTime.UtcNow;
                entities.SaveChanges();

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
                    Body = "Your order has been placed " + p.Customer.Name + "! You will get shipping information shortly."
            });

           
                //entities.SaveChanges();
                this.Response.SetCookie(new HttpCookie("cart") { Expires = DateTime.UtcNow });
                return RedirectToAction("Index", "Home", new { id = p.ID });
            }
            return View();
        }

        //TODO TODO TODO This still needs work and formating (needs to look presentable)
        //This is the body and structure of the email that the customer will receive
        private string CreateReceiptEmail(Purchase p)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<p>Thank you for placing your order at Raspberry Pi store " + (p.Customer.Name) + ".</p>");
            builder.Append("<h2>Order Details:</h2>");
            builder.Append("<table>");
            builder.Append("<thead><tr><th></th><th>Name</th><th>Description</th><th>Quantity</th><th>Unit Price</th><th>Total Price</th></tr></thead>");
            builder.Append("<tbody>");

            foreach (var product in p.Purchase_Product)
            {

                //test comment
                builder.Append("<tr>");

                builder.Append("<td>");
                //builder.AppendLine();
                builder.Append("</td>");

                builder.Append("<td>");
                builder.Append(product.Product.Name);
                builder.Append("</td>");

                builder.Append("<td>");
                builder.Append(product.Product.Review);
                builder.Append("</td>");

                builder.Append("<td>");
                builder.Append(product.Product.Quantity);
                builder.Append("</td>");

                builder.Append("<td>");
                builder.Append((product.Product.Price ?? 0).ToString("c"));
                builder.Append("</td>");

                builder.Append("<td>");
                builder.Append(((product.Product.Price ?? 0) * (product.Quantity ?? 0)).ToString("c"));
                builder.Append("</td>");


                builder.Append("</tr>");
            }
            
            builder.Append("</tbody><tfoot><tr><td colspan=\"5\">Total</td><td>");
            builder.Append(p.Purchase_Product.Sum(x => (x.Product.Price ?? 0) * x.Quantity ?? 0).ToString("c"));
            builder.Append("</td></tr></tfoot></table>");
            return builder.ToString();
        }
    }
}
