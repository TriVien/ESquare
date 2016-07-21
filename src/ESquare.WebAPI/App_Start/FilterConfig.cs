using System.Web;
using System.Web.Mvc;
using ESquare.WebAPI.Infrastructure;

namespace ESquare.WebAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MultitenantAuthorizeAttribute());
        }
    }
}
