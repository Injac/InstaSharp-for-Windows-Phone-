using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Threading;
using Spring.Collections.Specialized;
using Spring.Util;

namespace InstaSharp {
    

    public static class HttpClient {


        private static ManualResetEvent allDone = new ManualResetEvent(false);

        private static HttpWebRequest request;

        private static WebRequest webRequest;

        private static HttpWebResponse response;

        private static string requestResult = string.Empty;

        public static  string GET(string uri) {
            try {

                request = (HttpWebRequest) WebRequest.Create(uri);
                request.Method = "GET";

                
                
              request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
                allDone.Reset();
                allDone.WaitOne();
                 

                
                return ReadResponse(response.GetResponseStream());
            }
            catch (WebException ex) {
                return ReadResponse(ex.Response.GetResponseStream());
            }
        }


        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                request = (HttpWebRequest)asynchronousResult.AsyncState;
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                allDone.Set();
            }
            catch (Exception e)
            {
               //Debug.WriteLine("Got Exception in GetResponseCallback: " + e.Message);
            }
        }



        public static string POST(string url) {
            return POST(url, new Dictionary<string, string>());
        }

        public static string POST(string url, IDictionary<string, string> args) {
            try {
               
                WebClient client = new WebClient();

                client.Encoding = Encoding.UTF8;

                
                StringBuilder formattedParams = new StringBuilder();

                IDictionary<string, string> parameters = new Dictionary<string, string>();

                  foreach (var arg in args) {
                    parameters.Add(arg.Key, arg.Value);
                    formattedParams.AppendFormat("{0}={1}&", arg.Key, HttpUtility.UrlEncode(arg.Value));
                  }

                

                //Formatted URI 
               // Uri baseUri = new Uri(url + "?" + formattedParams.ToString().Substring(0, formattedParams.Length - 1));

                  client.Headers[HttpRequestHeader.ContentLength] = formattedParams.Length.ToString();  

                client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_UploadStringCompleted);
                 
               client.UploadStringAsync(new Uri(url),  "POST",formattedParams.ToString());
                allDone.Reset();
               allDone.WaitOne();


                return requestResult;

                
            }
            catch (WebException ex) {
                return ReadResponse(ex.Response.GetResponseStream());
               
            }
        }

        static void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            requestResult = e.Result;
            allDone.Set();
        }

        public static string DELETE(string uri) {
            var request = HttpWebRequest.Create(uri);
            request.Method = "DELETE";

            return DELETE(uri, new Dictionary<string, string>());

            // return ReadResponse(request.GetResponse().GetResponseStream());
        }

        public static string DELETE(string uri, IDictionary<string, string> args) {
            try {
                WebClient client = new WebClient();

                client.Encoding = Encoding.UTF8;

                client.Headers["Connection"] = "Keep-Alive";
                StringBuilder formattedParams = new StringBuilder();

                IDictionary<string, string> parameters = new Dictionary<string, string>();

                foreach (var arg in args)
                {
                    parameters.Add(arg.Key, arg.Value);
                    formattedParams.AppendFormat("{0}={{{1}}}", arg.Key, arg.Key);
                }

                //Formatted URI 
                Uri baseUri = new Uri(uri);

                client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_UploadStringCompletedDelete);

                client.UploadStringAsync(baseUri, "DELETE", string.Empty);
                allDone.Reset();
                allDone.WaitOne();
                
                return requestResult;
            }
            catch (WebException ex) {
                return ReadResponse(ex.Response.GetResponseStream());
            }
        }

        static void client_UploadStringCompletedDelete(object sender, UploadStringCompletedEventArgs e)
        {
            requestResult = e.Result;
            allDone.Set();
        }


        private static string ReadResponse(Stream response) {
            StreamReader reader = new StreamReader(response);
            string line;
            StringBuilder result = new StringBuilder();
            while ((line = reader.ReadLine()) != null) {
                result.Append(line);
            }
            return result.ToString();
        }
    }
}
