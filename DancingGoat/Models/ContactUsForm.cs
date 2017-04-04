using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models
{
    public class ContactUsForm
    {
        [Required(ErrorMessage = "You must provide your name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must provide your email."), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must include a message.")]
        public string Message { get; set; }
    }
}