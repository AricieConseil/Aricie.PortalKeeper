using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Behaviors;
using System.Web.Moles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Web;
using System.Net;

namespace Aricie.DNN.Test.TestFramework.Services.Mocks
{
    public class HttpContextMocks
    {

        public static void Mock()
        {

            BHttpContext.SetCurrent();
           
            MHttpRequest request = new MHttpRequest();
            MHttpResponse response = new MHttpResponse();
            MHttpCookieCollection cookies = new MHttpCookieCollection();

            response.CookiesGet = () => cookies;
            response.OnCookieAddHttpCookie = (cookie) => Nothing();
            response.RequestGet = () => request;
            cookies.GetString = (key) => null;
            
            Dictionary<object, object> items = new Dictionary<object, object>();
            Dictionary<string, string> reqItems = new Dictionary<string, string>();

            request.ItemGetString = (key) => reqItems.ContainsKey(key) ? reqItems[key] : null;


            new MHttpContext(BHttpContext.Current)
            {
                
                ItemsGet = () => items,
                RequestGet = () => request,
                ResponseGet = () => response
            };


            MHttpUtility.ParseQueryStringString = ParseQueryString;
            MHttpUtility.UrlEncodeString = (url) => Uri.EscapeDataString(url).Replace("%20", "+");

        }

        private static NameValueCollection ParseQueryString(string qs)
        {
            NameValueCollection toReturn = new NameValueCollection();

            if (!string.IsNullOrEmpty(qs))
            {
                foreach (string pair in qs.TrimStart('?').Split('&'))
                {
                    toReturn.Add(pair.Split('=')[0], pair.Split('=')[1]);
                }
            }


            return toReturn;
        }

        private static void Nothing() { }
    }
}
