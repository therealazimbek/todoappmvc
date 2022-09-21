using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using ToDoAppFinal.Models.ViewModels;

namespace ToDoAppFinal.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            _urlHelperFactory = helperFactory ?? throw new ArgumentNullException(nameof(helperFactory));
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");

                if (!string.IsNullOrEmpty(ViewContext.HttpContext.Request.Query["showCompletedTasks"].ToString())
                    && !ViewContext.HttpContext.Request.Query["showCompletedTasks"].ToString().Contains("!"))
                {
                    tag.Attributes["href"] = urlHelper.Action(PageAction, new { page = i, showCompletedTasks = true });

                }
                else if (!string.IsNullOrEmpty(ViewContext.HttpContext.Request.Query["showHidden"])
                    && !ViewContext.HttpContext.Request.Query["showHidden"].ToString().Contains("!"))
                {
                    tag.Attributes["href"] = urlHelper.Action(PageAction, new { page = i, showHidden = true });
                }
                else
                {
                    tag.Attributes["href"] = urlHelper.Action(PageAction, new { page = i });
                }

                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage
                        ? PageClassSelected : PageClassNormal);
                }

                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }

            output.Content.AppendHtml(result.InnerHtml);
        }
    }

}
