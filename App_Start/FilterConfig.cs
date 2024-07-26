using System.Web;
using System.Web.Mvc;

namespace CallBackUtility
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {//Commenting this line out solved the problem and allowed me to use custom error pages for 500 errors
           // filters.Add(new HandleErrorAttribute());
        }
    }
}
