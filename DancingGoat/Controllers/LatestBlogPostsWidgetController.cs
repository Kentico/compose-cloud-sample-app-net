using System.Web.Mvc;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

using DancingGoat.Models;

using KenticoCloud.Delivery;
using Newtonsoft.Json.Linq;

namespace DancingGoat.Controllers
{
    public class LatestBlogPostsWidgetController : Controller
    {
        private const int DISPLAY_LIMIT = 4;
        private const string ORDER_ELEMENT = "elements.post_date";
        private const SortOrder ORDER_DIRECTION = SortOrder.Descending;

        private readonly DeliveryClient client = new DeliveryClient(ConfigurationManager.AppSettings["ProjectId"], ConfigurationManager.AppSettings["PreviewToken"]);

        [Route("Widgets/LatestBlogPostsWidget")]
        public async Task<ActionResult> Default()
        {
            var articles = await client.GetItemsAsync<Article>(new EqualsFilter("system.type", "article"),
                new OrderParameter(ORDER_ELEMENT, ORDER_DIRECTION),
                new ElementsParameter("teaser_image", "post_date", "summary"),
                new LimitParameter(DISPLAY_LIMIT));

            return PartialView(articles.Items);
        }
    }
}