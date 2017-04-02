using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpCC.UtilityFramework.Loggings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework
{
    public static class ApiHelper
    {
        public static string GetResponse(string url, string Authorization, string JSONData = "")
        {
            string rXml = string.Empty;
            try
            {
                HttpWebRequest myHttpWebRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
                myHttpWebRequest.KeepAlive = false;
                myHttpWebRequest.Method = "POST";
                myHttpWebRequest.AllowAutoRedirect = false;
                myHttpWebRequest.Headers.Add("Authorization", Authorization);
                //myHttpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)";

                myHttpWebRequest.Timeout = 30000;
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                //byte[] bs = Encoding.ASCII.GetBytes(url);     

                if (string.IsNullOrEmpty(JSONData))
                {
                    myHttpWebRequest.ContentLength = 0;
                }
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(JSONData);
                    myHttpWebRequest.ContentLength = bytes.Length; //远程服务器返回错误: (411) 所需的长度
                    myHttpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
                }

                using (HttpWebResponse res = (HttpWebResponse)myHttpWebRequest.GetResponse())
                {
                    if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.PartialContent)//返回为200或206
                    {
                        string dd = res.ContentEncoding;
                        Stream strem = res.GetResponseStream();
                        StreamReader r = new StreamReader(strem);
                        rXml = r.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                rXml = new JObject(new JProperty("status", 403), new JProperty("error_msg", ex.Message)).ToString();
                LogHelper.Error("GetResponse errors:", ex);
            }
            return rXml;
        }
    }
}
