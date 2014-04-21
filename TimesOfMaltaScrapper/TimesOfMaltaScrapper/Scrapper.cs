using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace TimesOfMaltaScrapper
{
    public class Scrapper
    {
        protected static string rssFeed = "http://www.timesofmalta.com/rss/news";

            public Scrapper()
        {
            WebClient times = new WebClient();

            byte[] myDataBuffer = times.DownloadData(rssFeed);

            string download = Encoding.ASCII.GetString(myDataBuffer);

            /*using (FileStream stream = new FileStream("C:\\Development\\Guts\\TimesOfMaltaScrapper\\TimesOfMaltaScrapper\\bin\\Debug\\" + DateTime.Now.ToString() + ".rss", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            System.IO.StreamWriter sw = new System.IO.StreamWriter(stream, Encoding.Default);
                            sw.Write(download);
                            sw.Flush();
                            //stream.SetLength(textToTranslate.Length);
                            stream.Close();
                        }*/

            File.WriteAllText("rss\\times" + DateTime.Now.ToString("yyyyMMdd") + ".rss", download);

            int guidID = download.IndexOf("<item>", 0);
            string guidSTR = "";
            string downloadTemp = "";
            while (true)
            {
                guidID = download.IndexOf("<link>", guidID + 1);
                guidSTR = download.Substring(guidID, download.IndexOf("</link>", guidID) - guidID);
                guidSTR = guidSTR.Replace("<link>", "");

                myDataBuffer = times.DownloadData(guidSTR);

                downloadTemp = Encoding.ASCII.GetString(myDataBuffer);
                downloadTemp = downloadTemp.Replace("/min/e.css", "http://www.timesofmalta.com/min/e.css");

                File.WriteAllText("articles\\" + guidSTR.Substring(guidSTR.LastIndexOf("/")) + ".html", downloadTemp);

                FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create("ftp://ftp.andrewbuttigieg.com" + guidSTR.Substring(guidSTR.LastIndexOf("/")) + ".html");
                ftp.Method = WebRequestMethods.Ftp.UploadFile;

                ftp.Credentials = new NetworkCredential("andrewbu_123", "Steff2010rew");
                ftp.UsePassive = true;
                ftp.UseBinary = true;
                ftp.KeepAlive = true;

                StreamReader sourceStream = new StreamReader("articles\\" + guidSTR.Substring(guidSTR.LastIndexOf("/")) + ".html");
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                ftp.ContentLength = fileContents.Length;

                Stream requestStream = ftp.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

            }
        
            return;
        }
    }
}
