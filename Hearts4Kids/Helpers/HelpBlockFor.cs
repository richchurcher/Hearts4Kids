using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Hearts4Kids.Helpers
{
    public static class MvcHtmlHelpers
    {
        public static MvcHtmlString HelpBlockFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var description = metadata.Description;
            if (string.IsNullOrEmpty(description)) { return MvcHtmlString.Empty; }
            return MvcHtmlString.Create(string.Format(@"<p class='help-block col-md-10 col-md-push-2'>{0}</p>", description));
        }
    }
}