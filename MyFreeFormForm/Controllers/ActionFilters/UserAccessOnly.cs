using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Data;

namespace MyFreeFormForm.Controllers.ActionFilters
{
    /// <summary>
    ///  
    /// </summary>
    public class UserAccessOnly : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute, Microsoft.AspNetCore.Mvc.Filters.IActionFilter
    {
        private readonly MyIdentityUsers _dal;
        public UserAccessOnly(MyIdentityUsers dal)
        {
            _dal = dal;
        }


        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            if (context.RouteData.Values.ContainsKey("id"))
            {
                int id = int.Parse((string)context.RouteData.Values["id"]);
                if (context.HttpContext.User != null)
                {
                    var username = context.HttpContext.User.Identity.Name;
                    if (username != null)
                    {
                        /*var myevent = _dal.Events.FirstOrDefault(x => x.Id == id);
                        if (myevent.User != null)
                        {
                            if (myevent.User.UserName != username)
                            {
                                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "NotFound" }));
                            }
                        }*/
                    }

                }
            }
        }






    }
}
