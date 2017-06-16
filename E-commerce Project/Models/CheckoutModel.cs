using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_commerce_Project.Models
{
    public class CheckoutModel
    {

        public CheckoutModel()
        {
            this.BillingAddress = new AddressModel();
            this.ShippingAddress = new AddressModel();
        }
        public AddressModel BillingAddress { get; set; }
        public AddressModel ShippingAddress { get; set; }


        public string ShippingToLine { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string ContactPhone { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string ContactEmail { get; set; }
        [Required]
        [CreditCard]
        [Display(Name = "Credit Card Number")]
        public string CreditCardNumber { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(4)]
        [Display(Name = "CCV")]
        public string CreditCardVerificationValue { get; set; }
        [Required]
        [Display(Name = "Expiration Month")]
        public int? CreditCardExpirationMonth { get; set; }
        [Required]
        [Range(2017, 2027)]
        [Display(Name = "Expiration Year")]
        public int? CreditCardExpirationYear { get; set; }
        [Required]
        [Display(Name = "Name on Card")]
        public string CreditCardName { get; set; }
    }
}