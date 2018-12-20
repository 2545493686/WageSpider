using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanQ.SpiderLibrary;
using LanQ.HttpLibrary;
using LanQ;
using System.Text.RegularExpressions;
using System.IO;

namespace WageSpider
{
    struct WageData
    {
        public string company;
        public string minWage;
        public string maxWage;
        public string city;
    }
    class Program
    {
        static void Main(string[] args)
        {

            #region 字符常量
            const string PATH_CITY_CHINAHR = "urls_chinahr.txt";

            const string PATH_DATA_UNITY_CHINAHR = "datas_unity_chinahr.txt";
            const string PATH_DATA_PHP_CHINAHR = "datas_php_chinahr.txt";
            const string PATH_DATA_ANDROID_CHINAHR = "datas_android_chinahr.txt";
            const string PATH_DATA_JAVA_CHINAHR = "datas_java_chinahr.txt";
            const string PATH_DATA_CPP_CHINAHR = "datas_cpp_chinahr.txt";
            const string PATH_DATA_HTML_CHINAHR = "datas_html_chinahr.txt";

            const string UNITY_SUFFIX = "job/pn{0}/pve_5356_5/?key=unity";
            const string ANDROID_SUFFIX = "job/pn{0}/pve_5356_5/?key=android";
            const string PHP_SUFFIX = "job/pn{0}/pve_5356_5/?key=php";
            const string JAVA_SUFFIX = "job/pn{0}/pve_5356_5/?key=java";
            const string CPP_SUFFIX = "job/pn{0}/pve_5356_5/?key=c++";
            const string HTML_SUFFIX = "job/pn{0}/pve_5356_5/?key=html";

            #endregion

            //获取城市列表
            //ChinnahrSpider.GetCityUrls(PATH_CITY_CHINAHR);

            //从文件读取城市列表

            WageSpider spider = new ChinnahrSpider(PathAddTime(PATH_DATA_HTML_CHINAHR));

            //spider.GetUrlsToFile(PATH_CITY_CHINAHR); //获取城市列表到文件
            spider.AddUrlsFromFile(PATH_CITY_CHINAHR, HTML_SUFFIX); //添加要跑的url

            #region 正则匹配规则和裁剪
            string[] patternes_chainahr = //正则规则
            {
                "\"job-company\" title='.+?'", //公司
                "\"job-salary\"> .+?</li", //薪资
                "\"job-address\"> .+?<span" //联系方式
            };
            string[] startTrim_chainahr = //前面剔除文本
            {
                "\"job-company\" title='",
                "\"job-salary\"> ",
                "\"job-address\"> "
            };
            string[] endTrim_chainahr =
            {
                "'", "</li", "<span"
            };
            #endregion

            //组织正则表达式和剪裁规则
            Pattern[] pattern_chainahr = GetPattern(patternes_chainahr, startTrim_chainahr, endTrim_chainahr);

            //运行爬虫
            spider.Run(pattern_chainahr);

#if false //对文字进行细分析，格式化
            string[] dataPathes =
            {
                PATH_DATA_UNITY_CHINAHR,
                PATH_DATA_PHP_CHINAHR,
                PATH_DATA_ANDROID_CHINAHR,
                PATH_DATA_JAVA_CHINAHR
            };

            string[] types =
            {
                "Unity", "PHP", "Android", "Java"
            };

            for(int i = 0; i < dataPathes.Length; i++)
            {
                string item = dataPathes[i];
                TextFile textFile = new TextFile(item);
                TextFile outFile = new TextFile("out_" + item);
                string[] contexts = textFile.ReadToEnd().Split('\n');
                for (int j = 0; j < contexts.Length - 2; j += 3)
                {
                    Console.WriteLine("{0}-{1}-{2}", types[i], j, contexts.Length);
                    if (contexts[j + 1].LastIndexOf(" 元/月") == -1)
                    {
                        continue;
                    }

                    string wage = contexts[j + 1].Trim("", " 元/月");
                    string[] wages = contexts[j + 1].Split('-');

                    WageData wageData = new WageData
                    {
                        company = contexts[j].TrimEnd('\r'),
                        minWage = wages.Length > 0 ? wages[0] : wage,
                        maxWage = wages.Length > 0 ? wages[1] : wage,
                        city = contexts[j + 2].Split('|')[0].Split('-')[0]
                    };
                    outFile.WriteLine(types[i] +
                        "-" + wageData.company +
                        "-" + wageData.city +
                        "-" + wageData.minWage +
                        "-" + wageData.maxWage);
                }
                outFile.Close();
                textFile.Close();
            }
#endif

            Console.ReadKey();
        }

        private static string PathAddTime(string path)
        {
            return GetShortTime() + "_" + path;
        }

        private static Pattern[] GetPattern(string[] patternes, string[] startTrim, string[] endTrim)
        {
            if (patternes.Length != startTrim.Length || patternes.Length != endTrim.Length)
                throw new Exception("INPUT ERROR, DIFFERENT ARRAY LENGTH!");

            Pattern[] ret = new Pattern[3];
            for (int i = 0; i < 3; i++)
            {
                ret[i] = new Pattern
                {
                    pattern = patternes[i],
                    startTrim = startTrim[i],
                    endTrim = endTrim[i]
                };
            }

            return ret;
        }

        private static string GetShortTime()
        {
            DateTime dateTime = DateTime.Now;
            return string.Format("{0}_{1}_{2}_{3}_{4}",
                dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute,
                dateTime.Second);
        }



    }
}
