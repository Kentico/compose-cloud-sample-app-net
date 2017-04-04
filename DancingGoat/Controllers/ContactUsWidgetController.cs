using System.Net.Mail;
using System.Web.Mvc;
using System.Threading.Tasks;

using DancingGoat.Controllers;

using DancingGoat.Models;

namespace DancingGoat.Controllers
{
    public class ContactUsWidgetController : Controller
    {
        [Route("Widgets/ContactUsWidget")]
        public ActionResult Default()
        {
            return PartialView(new ContactUsForm());
        }

        public async Task<ActionResult> SendMessage(ContactUsForm contactForm)
        {
            var body = "<p>The following is a message from {0} ({1})</p><p>{2}</p>";
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress("destination@localhost.com"));
            message.From = new MailAddress(contactForm.Email);
            message.Subject = "Contact Form Message";
            message.Body = string.Format(body, contactForm.Name, contactForm.Email, contactForm.Message);
            message.IsBodyHtml = true;

            using (SmtpClient client = new SmtpClient())
            {
                client.Host = "localhost";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                await client.SendMailAsync(message);
            }

            return RedirectToAction("Index", "CoffeeBrewerySeminars");
        }
    }
}