using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using KenticoCloud.Delivery;

namespace DancingGoat.Helpers.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Generates an IMG tag for an image file.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="asset">The asset to display</param>
        /// <param name="title">Title</param>
        /// <param name="cssClass">CSS class</param>
        public static MvcHtmlString AssetImage(this HtmlHelper htmlHelper, Asset asset, string title = null, string cssClass = "", int? width = null, int? height = null)
        {
            if (asset == null)
            {
                return MvcHtmlString.Empty;
            }

            var image = new TagBuilder("img");
            image.MergeAttribute("src", asset.Url);
            image.AddCssClass(cssClass);
            string titleToUse = title ?? asset.Name;
            image.MergeAttribute("alt", titleToUse);
            image.MergeAttribute("title", titleToUse);

            if (width.HasValue)
            {
                image.MergeAttribute("width", width.ToString());
            }

            if (height.HasValue)
            {
                image.MergeAttribute("height", height.ToString());
            }

            return MvcHtmlString.Create(image.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString DateTimeFormatted(this HtmlHelper htmlHelper, DateTime? dateTime, string format)
        {
            if (!dateTime.HasValue || string.IsNullOrEmpty(format))
            {
                return MvcHtmlString.Empty;
            }

            return MvcHtmlString.Create(dateTime.Value.ToString(format));
        }

        /// <summary>
        /// Returns HTML input element with a label and validation fields for each property in the object that is represented by the System.Linq.Expressions.Expression expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends</param>
        /// <param name="expression">An expression that identifies the object that contains the properties to display</param>
        public static MvcHtmlString ValidatedEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var label = html.LabelFor(expression).ToString();
            var editor = html.EditorFor(expression).ToString();
            var message = html.ValidationMessageFor(expression).ToString();

            var generatedHtml = string.Format(@"
<div class=""form-group"">
    <div class=""form-group-label"">{0}</div>
    <div class=""form-group-input"">{1}</div>
    <div class=""message message-error"">{2}</div>
</div>", label, editor, message);

            return MvcHtmlString.Create(generatedHtml);
        }
    }
}