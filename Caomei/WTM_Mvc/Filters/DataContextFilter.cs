using Caomei.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Caomei.Mvc.Filters
{
    public class DataContextFilter : ActionFilterAttribute
    {
        public static Func<ActionExecutingContext, string> _csfunc;

        public DataContextFilter()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as IBaseController;
            if (controller == null)
            {
                base.OnActionExecuting(context);
                return;
            }
            context.SetWtmContext();
            string cs = "";
            DBTypeEnum? dbtype = null;
            ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;
            var fixcontroller = ad.ControllerTypeInfo.GetCustomAttributes(typeof(FixConnectionAttribute), false).Cast<FixConnectionAttribute>().FirstOrDefault();
            var fixaction = ad.MethodInfo.GetCustomAttributes(typeof(FixConnectionAttribute), false).Cast<FixConnectionAttribute>().FirstOrDefault();
            var ispost = ad.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();
            string mode = "Read";

            if (fixcontroller != null || fixaction != null)
            {
                cs = fixaction?.CsName ?? fixcontroller?.CsName;
                var op = fixcontroller?.Operation ?? fixaction?.Operation;
                if (op != null)
                {
                    switch (op.Value)
                    {
                        case DBOperationEnum.Read:
                            mode = "Read";
                            break;

                        case DBOperationEnum.Write:
                            mode = "Write";
                            break;

                        default:
                            break;
                    }
                }
                dbtype = fixcontroller?.DbType ?? fixaction?.DbType;
                cs = Utils.GetCS(cs, mode, controller.Wtm.ConfigInfo);
            }
            else
            {
                cs = _csfunc?.Invoke(context);
                if (string.IsNullOrEmpty(cs))
                {
                    if (ispost != null)
                    {
                        mode = "Write";
                    }
                    cs = context.HttpContext.Request.Query["DONOTUSECSName"];
                    cs = Utils.GetCS(cs, mode, controller.Wtm.ConfigInfo);
                }
            }

            controller.Wtm.CurrentCS = cs;
            controller.Wtm.CurrentDbType = dbtype;
            base.OnActionExecuting(context);
        }
    }
}