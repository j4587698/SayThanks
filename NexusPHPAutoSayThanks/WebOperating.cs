using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.IO.Compression;

namespace NexusPHPAutoSayThanks
{
    class WebOperating
    {
        public static string GetMode(string url, string referer, string cookieStr = "", string domain = "")
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.Method = "GET";
            request.Referer = referer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11";
            if (cookieStr != "")
            {
                NameValueCollection collHeader =
                        new NameValueCollection();
                collHeader.Add("Cookie", cookieStr);
                int iCount = collHeader.Count;
                string key;
                string keyvalue;

                for (int i = 0; i < iCount; i++)
                {
                    key = collHeader.Keys[i];
                    keyvalue = collHeader[i];
                    request.Headers.Add(key, keyvalue);
                }
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return GetResponseBody(response);
        }

        public static string[] PostMode(string url, string referer, string data, string cookieStr = "", string domain = "")
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.Method = "POST";
            request.Referer = referer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11";
            if (cookieStr != "")
            {
                NameValueCollection collHeader =
                        new NameValueCollection();
                collHeader.Add("Cookie", cookieStr);
                int iCount = collHeader.Count;
                string key;
                string keyvalue;

                for (int i = 0; i < iCount; i++)
                {
                    key = collHeader.Keys[i];
                    keyvalue = collHeader[i];
                    request.Headers.Add(key, keyvalue);
                }
            }
            byte[] bs = Encoding.ASCII.GetBytes(data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bs.Length;
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException wbex)
            {
                response = (HttpWebResponse)wbex.Response;
                string returnstr = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return new string[] { ((int)wbex.Status).ToString(), returnstr };
            }


            return new string[] { "200", GetResponseBody(response)};
        }

        private static string GetResponseBody(HttpWebResponse response)
        {
            string responseBody = string.Empty;
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
            {
                using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            return responseBody;
        }
    }
}
