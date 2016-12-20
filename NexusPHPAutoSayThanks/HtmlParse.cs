using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;


namespace NexusPHPAutoSayThanks
{
    public class HtmlParse
    {

        public delegate void ShowLog(string msg);

        public static event ShowLog ShowLogEvent;

        public static bool IsRunning = true;

        public static void GetAllItems(string url, string cookie)
        {
            string host = new Uri(url).Host;
            do
            {
                string html = WebOperating.GetMode(url, host, cookie, host);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var root = doc.DocumentNode;
                var torrentnodes = root.SelectNodes("//table[@class='torrentname']");
                foreach (var torrent in torrentnodes)
                {
                    if (!IsRunning)
                    {
                        break;
                    }
                    var torrentUrl = torrent.SelectSingleNode(torrent.XPath + "/tr/td[1]/a").GetAttributeValue("href", "");
                    torrentUrl = HttpUtility.HtmlDecode(torrentUrl);
                    WriteLog("开始处理:" + torrentUrl);
                    Console.WriteLine(torrentUrl);
                    GetDetail(new Uri(new Uri(url), torrentUrl).ToString(), cookie);
                    Thread.Sleep(10000);
                }
                var nextPage = root.SelectSingleNode("//b[@title='Alt+Pagedown']");
                if (nextPage != null)
                {
                    var href = nextPage.ParentNode.GetAttributeValue("href", "");
                    if (href != "")
                    {
                        href = HttpUtility.HtmlDecode(href);
                        url = new Uri(new Uri(url), href).ToString();
                    }
                    else
                    {
                        url = string.Empty;
                    }
                }
                else
                {
                    url = string.Empty;
                }
                Console.WriteLine(url);
                WriteLog("翻到下一页：" + url);
            } while (IsRunning && url != string.Empty);
            
        }

        public static void GetDetail(string url, string cookie)
        {
            string host = new Uri(url).Host;
            string html = WebOperating.GetMode(url, host, cookie, host);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var root = doc.DocumentNode;
            var thanksButton = root.SelectSingleNode("//input[@id='saythanks']");
            if (thanksButton.GetAttributeValue("disabled", "") != "")
            {
                Console.WriteLine("已经说过谢谢");
                WriteLog("已经说过谢谢：" + url);
            }
            else
            {
                Console.WriteLine("没有说过谢谢");
                SayThanks(url, cookie);
            }
        }

        public static void SayThanks(string url, string cookie)
        {
            string host = new Uri(url).Host;
            var id = ParseUrl(url).Get("id");
            string[] returns = WebOperating.PostMode(string.Format("{0}://{1}/thanks.php", new Uri(url).Scheme, host), url,"id=" + id ,cookie, host);
            if (returns[0] == "200")
            {
                Console.WriteLine("说谢谢成功");
                WriteLog("说谢谢成功：" + url);
            }
            else
            {
                Console.WriteLine(string.Format("{0}:{1}", returns[0], returns[1]));
                WriteLog("说谢谢失败：" + string.Format("{0}:{1}", returns[0], returns[1]));
            }
        }

        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        public static NameValueCollection ParseUrl(string url)
        {
            if (url == null)
                return null;
            var nvc = new NameValueCollection();
            if (url == "")
                return null;
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                return null;
            }
            if (questionMarkIndex == url.Length - 1)
                return null;
            string ps = url.Substring(questionMarkIndex + 1);
            // 开始分析参数对  
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
            return nvc;
        }

        private static void WriteLog(string msg)
        {
            if (ShowLogEvent != null)
            {
                ShowLogEvent(msg);
            }
        }
    }
}
