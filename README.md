# nasa
earth images

There are 2 solutions in this repository. 

The first solution is using powershell scripts that downloads the images from NASA API 
and saved it to c:\temp folder. Before running the script, be sure the img_dates.txt and the img_downloader.ps1 is on the same folder.
Set the execution policy also to bypass by running the script below

Set-ExecutionPolicy -ExecutionPolicy bypass

The second solution is using asp.net core running on linux container. I used background services to download the images because downloading
is a very slow process. So when you run the application, it will display a blank page initially because the images not yet downloaded. Just 
click refresh (F5) and the album will start to appear. Click the album and the images will be displayed saved on that album.


