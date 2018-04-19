using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace cs_httpwebrequest_https
{
    class Program
    {
        static void pause()
        {
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        static private bool CheckValidationResult(object sender,X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;// Always accept
        }
        static private HttpWebResponse getUrlResponse(string url)
        {
            HttpWebResponse resp = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
            }

            //...
            resp = (HttpWebResponse)req.GetResponse();
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                String results = sr.ReadToEnd();
                Console.WriteLine(results);
                sr.Close();
            }
            
            //...
            return resp;
        }
        private static string PostUrl(string url, string postData)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request.ProtocolVersion = HttpVersion.Version11;
　　　　　　　　　// 這裡設置了協議類型。

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                request.KeepAlive = false;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }

            request.Method = "POST";//使用get方式發送數據
            request.ContentType = null;
            request.Referer = null;
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Accept = "*/*";

            byte[] data = Encoding.UTF8.GetBytes(postData);
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

			//獲取網頁響應結果
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
			//client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string result = string.Empty;
            using (StreamReader sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }
        static void Main(string[] args)
        {
            //*
            //語法01
            //資料來源:https://shunnien.github.io/2017/07/13/Accessing-HTTPS-URL-using-csharp/
            var url = "https://www.moi.gov.tw/";//台灣內政部網址
            string results;
            // 強制認為憑證都是通過的，特殊情況再使用
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            // 加入憑證驗證
            //request.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate());
            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                results = sr.ReadToEnd();
                sr.Close();
            }
            Console.WriteLine(results);
            pause();
            //*/

            //*
            //語法02
            getUrlResponse("https://www.moi.gov.tw/");
            pause();
            //*/

            //*
            //語法3
            string url1 = "https://www.moi.gov.tw/";
            string result = PostUrl(url1, "key=123");
            Console.WriteLine(result);
            //*/ 
            pause();
        }
    }

    //	資料來源:http://www.zendei.com/article/7723.html
    //	class ProgramTest
    //    {
    //        static void Main(string[] args)
    //        {
    //            string url = "https://www.test.com";
    //            string result = PostUrl(url, "key=123"); // key=4da4193e-384b-44d8-8a7f-2dd8b076d784
    //            Console.WriteLine(result);
    //            Console.WriteLine("OVER");
    //            Console.ReadLine();
    //        }
    //
    //        private static string PostUrl(string url, string postData)
    //        {
    //            HttpWebRequest request = null;
    //            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
    //            {
    //                request = WebRequest.Create(url) as HttpWebRequest;
    //                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
    //                request.ProtocolVersion = HttpVersion.Version11;
    //　　　　　　　　　// 這裡設置了協議類型。
    //                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
    //                request.KeepAlive = false;
    //                ServicePointManager.CheckCertificateRevocationList = true;
    //                ServicePointManager.DefaultConnectionLimit = 100;
    //                ServicePointManager.Expect100Continue = false;
    //            }
    //            else
    //            {
    //                request = (HttpWebRequest)WebRequest.Create(url);
    //            }
    //
    //            request.Method = "POST";    //使用get方式發送數據
    //            request.ContentType = null;
    //            request.Referer = null;
    //            request.AllowAutoRedirect = true;
    //            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
    //            request.Accept = "*/*";
    //
    //            byte[] data = Encoding.UTF8.GetBytes(postData);
    //            Stream newStream = request.GetRequestStream();
    //            newStream.Write(data, 0, data.Length);
    //            newStream.Close();
    //
    //            //獲取網頁響應結果
    //            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //            Stream stream = response.GetResponseStream();
    //            //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
    //            string result = string.Empty;
    //            using (StreamReader sr = new StreamReader(stream))
    //            {
    //                result = sr.ReadToEnd();
    //            }
    //
    //            return result;
    //        }
    //
    //        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    //        {
    //            return true; //總是接受  
    //        }
    //    }
    /*
        c# httpwebrequest https

        資料來源: https://www.crifan.com/access_https_type_url_in_csharp/

        using System.Net.Security;
        using System.Security;
        using System.Security.Cryptography;
        using System.Security.Cryptography.X509Certificates;
 
        private bool CheckValidationResult(object sender,
                X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;// Always accept
        }
     
        public HttpWebResponse getUrlResponse(string url)
        {
            HttpWebResponse resp = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
     
            if(url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
            }
     
            //...
            resp = (HttpWebResponse)req.GetResponse();
     
            //...
        } 
    */
}
