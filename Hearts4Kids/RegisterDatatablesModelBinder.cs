using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Mvc.JQuery.Datatables;

[assembly: PreApplicationStartMethod(typeof(Hearts4Kids.RegisterDatatablesModelBinder), "Start")]

namespace Hearts4Kids {
    public static class RegisterDatatablesModelBinder {
        public static void Start() {
            if (!ModelBinders.Binders.ContainsKey(typeof(DataTablesParam)))
                ModelBinders.Binders.Add(typeof(DataTablesParam), new Mvc.JQuery.Datatables.DataTablesModelBinder());
        }
    }
}
