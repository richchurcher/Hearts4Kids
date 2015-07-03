using Hearts4Kids.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hearts4Kids.Helpers
{
    public static class EnumHelpers
    {
        public static IEnumerable<SelectListItem> AsListItem<T>(T selected = default(T)) where T : struct, IComparable, IConvertible, IFormattable
        {
            var t = typeof(T);
            if (!t.IsEnum)
            {
                throw new ArgumentException("The generic type parameter must be an enum");
            }
            foreach (var e in Enum.GetValues(t))
            {
                yield return new SelectListItem { Text = e.ToString().SplitCamelCase(), Value = ((int)e).ToString(), Selected = e.Equals(selected) };
            }
        }
    }
}