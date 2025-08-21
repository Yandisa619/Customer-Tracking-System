using System.ComponentModel.DataAnnotations;

namespace CustomerTrackingSystem.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        // Customer Name Field and Validations
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        // Address Field and Validations
        [Required(ErrorMessage = "Address is required")]
        [StringLength(70, ErrorMessage = "Address cannot exceed 70 characters")]
        public string Address { get; set; }

        // Telephone Field and Validations
        [Display(Name = "Telephone Number")]
        [StringLength(20, ErrorMessage = "Telephone number cannot exceed 20 characters")]
        public string TelephoneNumber { get; set; }

        // Contact Person Field and Validations 
        [Display(Name = "Contact Person")]
        [StringLength(50, ErrorMessage = "Contact person name cannot exceed 50 characters")]
        public string ContactPersonName { get; set; }

        // Customer Contact Email and Validations 
        [Display(Name = "Contact Email")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string ContactPersonEmail { get; set; }

        // VAT Number Field and Validations 
        [Display(Name = "VAT Number")]
        [StringLength(20, ErrorMessage = "VAT number cannot exceed 20 characters")]
        public string VATNumber { get; set; }
    }
}