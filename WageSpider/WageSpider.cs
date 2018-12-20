using LanQ;
using LanQ.SpiderLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WageSpider
{
    abstract class WageSpider
    {
        public List<string> UrlList { get; } = new List<string>();
        protected TextFile DataFile { get; }

        protected WageSpider(string dataPath)
        {
            DataFile = new TextFile(dataPath); //初始化文件类
            //DataFile.WriteLine(GetShortDateTime());
        }

        public abstract void Run(Pattern[] patterns); //运行

        public abstract void AddUrlsFromFile(string path, params string[] additional); //从文件增加url

        public abstract int GetUrlsToFile(string path); //获取url，写到文件

        public virtual void Close()
        {
            DataFile.Close(); //关闭文件流
        }
    }
}
