using System;
using System.Collections.Generic;



namespace WallpaperDownloader
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            WallpaperApplication app = new WallpaperApplication();
            app.run();
        }
    }
}
