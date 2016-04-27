using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Mvc.JQuery.DataTables;

[assembly: PreApplicationStartMethod(typeof(Hearts4Kids.RegisterDataTablesModelBinder), "Start")]

namespace Hearts4Kids {
    public static class RegisterDataTablesModelBinder {
        public static void Start() {
            if (!ModelBinders.Binders.ContainsKey(typeof(DataTablesParam)))
                ModelBinders.Binders.Add(typeof(DataTablesParam), new Mvc.JQuery.DataTables.DataTablesModelBinder());
        }
    }
}
