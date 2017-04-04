using DancingGoat.Models;
using KenticoCloud.Delivery;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DancingGoat.Controllers
{
    public class ControllerBase : AsyncController
    {
        protected readonly DeliveryClient client = new DeliveryClient(ConfigurationManager.AppSettings["ProjectId"], ConfigurationManager.AppSettings["PreviewToken"]);

        public ControllerBase()
        {
            client.CodeFirstModelProvider.TypeProvider = new CustomTypeProvider();
        }

        protected async Task<ActionResult> ShowPage(string urlSlug, string type, string viewName = null)
        {
            if (urlSlug == null)
            {
                return Redirect("/");
            }

            try
            {
                var response =
                    await client.GetItemsAsync(new[] {
                        new EqualsFilter("system.type", type),
                        new EqualsFilter("elements.url", urlSlug)
                    });

                var item = response.Items.FirstOrDefault();
                if (item == null)
                {
                    return Redirect("/");
                }

                return View(viewName, item);
            }
            catch (DeliveryException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return Redirect("/");
                }

                throw;
            }
        }
    }
}