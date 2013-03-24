WallpaperDownloader
===================

The WallpaperDownloader is a small C# application that allows to download the current National Geographic photo of the day (see here: http://photography.nationalgeographic.com/photography/).

Unlike 'real' wallpaper download applications like WallpaperChanger (http://wallpaperchanger.codeplex.com/), this application does not come with a graphical interface but downloads the current photo
of the day and then quits.

This comes with the advantage of silence during its run along with a number of retries that allows you to run this application during system start up even 
when the LAN/ Wifi connection is not yet up.
As soon as a network connection is available, the current photo of the day will be downloaded automatically and set as the desktop background. All photos of the
day will be stored unter 'My pictures' in a new sub directory called 'Photos of the day'.