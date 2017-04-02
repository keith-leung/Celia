using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SharpCC.UtilityFramework;
using SharpCC.UtilityFramework.Loggings;

namespace SharpCC.UtilityFramework.NavigationProps
{
    /// <summary>
    /// 解决掉Navigation Property循环引用使得Json反序列化失败的问题。
    /// </summary>
    public class NavigationLoopSolvedJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet
                && string.Compare(context.HttpContext.Request.HttpMethod, "Get", true) == 0)
            {
                return;
            }

            try
            {
                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;
                if (this.ContentEncoding != null)
                {
                    response.ContentEncoding = this.ContentEncoding;
                }
                if (null != this.Data)
                {
                    response.Write(JsonConvert.SerializeObject(this.Data, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                }
            }
            catch (Exception ee)
            {
                LogHelper.Debug(ee.Message + "\t\t" + ee.StackTrace);
                base.ExecuteResult(context);
            }
        }
    }
}
