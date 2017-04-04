using System.Threading.Tasks;
using System.Web.Mvc;

namespace DancingGoat.Controllers
{
    public class LandingPageController : ControllerBase
    {
        public async Task<ActionResult> Empty(string urlSlug)
        {
            return await ShowPage(urlSlug, "empty_page", "Empty");
        }


        public async Task<ActionResult> View(string urlSlug)
        {
            return await ShowPage(urlSlug, "landing_page");
        }
    }
}