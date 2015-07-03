using System;
using System.Collections.Generic;
using System.Linq;
using Hearts4Kids.Domain;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Hearts4Kids.Services
{
    public class FundraisingServices
    {
        public static async Task<IEnumerable<SelectListItem>> GetMembers()
        {
            using (var db = new Hearts4KidsEntities())
            {
                return await (from u in db.UserBios
                                select new SelectListItem
                                {
                                    Value = u.Id.ToString(),
                                    Text = u.FirstName + " " + u.Surname
                                }).ToListAsync();
            }
        }
    }
}