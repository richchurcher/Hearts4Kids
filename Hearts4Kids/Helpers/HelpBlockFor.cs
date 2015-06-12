using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Hearts4Kids.Helpers
{
    public static class MvcHtmlHelpers
    {
        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var description = metadata.Description;
            if (string.IsNullOrEmpty(description)) { return MvcHtmlString.Empty; }
            return MvcHtmlString.Create(description);
        }
    }
}