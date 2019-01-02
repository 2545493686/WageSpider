using LanQ;
using LanQ.HttpLibrary;
using LanQ.SpiderLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WageSpider
{
    class ChinnahrSpider : WageSpider
    {
        public ChinnahrSpider(string outDataPath) : base(outDataPath)
        {
        }

        public override void Run(Pattern[] patterns) //执行爬虫
        {
            Spider spider = new Spider(UrlList.ToArray(), DataFile);
            spider.GetText(patterns, 5000);
        }

        public override void AddUrlsFromFile(string path, params string[] additional)
        {
            string[] cityUrlList = GetCityUrlList(path);

            int getUrlIndex = 0;
            foreach (var item in cityUrlList)
            {
                Console.WriteLine(string.Format("get url index: {0} / {1}",
                    getUrlIndex++, cityUrlList.Length));

                string s = item;
                foreach (var add in additional)
                {
                    s += add;
                }
                AddUrl(s);
            }
        }

        private static string[] GetCityUrlList(string cityUrlPath)
        {
            TextFile urlFile = new TextFile(cityUrlPath);
            string[] url_City = urlFile.ReadToEnd().Split('\r');
            urlFile.Close();
            return url_City;
        }

        /// <summary>
        /// 输入一个城市的完整链接，其中页数用占位符代替，自动获取所有页
        /// </summary>
        /// <param name="urlFormat"></param>
        private void AddUrl(string cityUrlFormat) 
        {
            int number = GetMaxNumber(string.Format(cityUrlFormat, 1));
            if (number != 0)
            {
                LinearAddList(cityUrlFormat, number / 30 + 1);
            }
        }

        private void LinearAddList(string text, int unityNumber)
        {
            for (int i = 1; i <= unityNumber; i++)
            {
                UrlList.Add(string.Format(text, i));
            }
        }

        private static int GetMaxNumber(string url)
        {
            string source = Http.Get(url, 5000);
            if (source == string.Empty)
                return 0;

            Match match = Regex.Match(source, "共为您找到 .+? 条职位");
            string numberText = match.Value.Trim("共为您找到 <span> ", " </span> 条职位");
            return Convert.ToInt32(numberText);
        }

        /// <summary>
        /// 爬取城市首页列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override int GetUrlsToFile(string path)
        {
            List<string> urls = GetUrls();
            WriteToFile(urls.ToArray(), path);
            return urls.Count;
        }

        private static List<string> GetUrls()
        {
            List<string> retUrls = new List<string>();
            const char letter = 'a';
            int foundIndex = 0;
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    char fisrtLetter = GetNextLetter(letter, i);
                    char secondLetter = GetNextLetter(letter, j);

                    Console.WriteLine("i:{0}, j:{1}", fisrtLetter, secondLetter);

                    string url = string.Format(@"https://search.chinahr.com/{0}{1}/", fisrtLetter, secondLetter);
                    string source = Http.Get(url);

                    if (Regex.IsMatch(source, "共为您找到.*条职位"))
                    {
                        retUrls.Add(url);
                        Console.WriteLine(foundIndex++);
                    }
                }
            }
            return retUrls;
        }

        private static void WriteToFile(string[] texts, string path)
        {
            TextFile textFile = new TextFile(path);

            //textFile.WriteLine(GetShortDateTime());
            foreach (var item in texts)
            {
                textFile.WriteLine(item);
            }

            textFile.Close();
        }

        private static char GetNextLetter(char letter, int i)
        {
            return Convert.ToChar(Convert.ToInt32(letter) + i);
        }
    }
}
