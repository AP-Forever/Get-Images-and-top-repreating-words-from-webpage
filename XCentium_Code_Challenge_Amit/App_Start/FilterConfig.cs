using System.Web;
using System.Web.Mvc;

namespace XCentium_Code_Challenge_Amit
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
