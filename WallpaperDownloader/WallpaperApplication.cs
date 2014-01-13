using System;
using System.Collections.Generic;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Net;
using System.IO;


namespace WallpaperDownloader
{
    public class WallpaperApplication
    {
        public int MaxTries { get; set; }
        public int DelayInitialSeconds { get; set; }
        public int DelayBetweenTriesSeconds { get; set; }

        public WallpaperApplication()
        {
            this.MaxTries = 2;
            this.DelayBetweenTriesSeconds = 15;
            this.DelayInitialSeconds = 15;
        }


        private string _downloadNationalGeographicPhotoOfTheDayIndexFile()
        {
            string retval = "";
            try
            {
                using (WebClient Client = new WebClient())
                {
                    byte[] data = Client.DownloadData("http://photography.nationalgeographic.com/photography/photo-of-the-day");
                    System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    retval = enc.GetString(data);
                }
            }
            catch (System.Net.WebException)
            {
                throw new Exception("Error fetching index file");
            }
            catch (Exception)
            {
                return "";
            }

            return retval;
        } // !_downloadNationalGeographicPhotoOfTheDayIndexFile()


        private string _downloadBingPhotoOfTheDayIndexFile()
        {
            string retval = "";
            try
            {
                using (WebClient Client = new WebClient())
                {
                    byte[] data = Client.DownloadData("http://www.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=1&mkt=en-US");
                    System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    retval = enc.GetString(data);
                }
            }
            catch (System.Net.WebException)
            {
                throw new Exception("Error fetching index file");
            }
            catch (Exception)
            {
                return "";
            }

            return retval;
        } // !_downloadBingPhotoOfTheDayIndexFile()


        private string _generateTargetPath()
        {
            int counter = 0;
            string datestring = DateTime.Today.ToString("yyyy-MM-dd");
            string userprofile = Environment.GetEnvironmentVariable("USERPROFILE");
            string targetDirectory = System.IO.Path.Combine(userprofile, @"Pictures\Photos of the day");
            if (!System.IO.Directory.Exists(targetDirectory))
            {
                System.IO.Directory.CreateDirectory(targetDirectory);
            }

            string counterStr = "";
            if (counter > 0)
            {
                counterStr = String.Format("_{0}", counter);
            }
            
            return String.Format("{0}\\{1}{2}.jpg", targetDirectory, datestring, counterStr);
        } // !_generateTargetPath()


        public void run()
        {
            System.Threading.Thread.Sleep(this.DelayInitialSeconds * 1000); // initial sleep

            if (!_tryDownloadNationalGeographicWallpaper())
            {
                _tryDownloadBingWallpaper();
            }
        } // !run()


        private bool _tryDownloadNationalGeographicWallpaper()
        {
            // try a defined number of times to download the index file
            string content = "";
            int numTries = 0;
            while (numTries < this.MaxTries)
            {
                try
                {
                    content = _downloadNationalGeographicPhotoOfTheDayIndexFile();
                }
                catch
                {
                    numTries++;
                    System.Threading.Thread.Sleep(this.DelayBetweenTriesSeconds * 1000);
                }

                if (content.Length > 0)
                {
                    break;
                }
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);
            HtmlNode node = doc.DocumentNode.QuerySelector("div.download_link > a");
            if (node != null)
            {
                string url = node.Attributes["href"].Value;
                if (url.Length > 0)
                {
                    string targetPath = _generateTargetPath();
                    using (WebClient Client = new WebClient())
                    {
                        Client.DownloadFile(url, targetPath);
                        Wallpaper.Set(targetPath, Wallpaper.Style.Stretched);
                        return true;
                    }
                }
            }

            return false;
        } // !_tryDownloadNationalGeographicWallpaper()


        private bool _tryDownloadBingWallpaper()
        {
            // try a defined number of times to download the index file
            string content = "";
            int numTries = 0;
            while (numTries < this.MaxTries)
            {
                try
                {
                    content = _downloadBingPhotoOfTheDayIndexFile();
                }
                catch
                {
                    numTries++;
                    System.Threading.Thread.Sleep(this.DelayBetweenTriesSeconds * 1000);
                }

                if (content.Length > 0)
                {
                    break;
                }
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);
            HtmlNode node = doc.DocumentNode.QuerySelector("url");
            if (node != null)
            {
                string url = String.Format("http://www.bing.com/{0}", node.InnerText);
                if (url.Length > 0)
                {
                    string targetPath = _generateTargetPath();
                    using (WebClient Client = new WebClient())
                    {
                        Client.DownloadFile(url, targetPath);
                        Wallpaper.Set(targetPath, Wallpaper.Style.Stretched);
                        return true;
                    }
                }
            }

            return false;
        } // !_tryDownloadBingWallpaper()
    }
}
