using System.Web;
using System.Web.Mvc;

namespace CLIWEB_BANQUITO_RESTFULL_DOTNET
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
