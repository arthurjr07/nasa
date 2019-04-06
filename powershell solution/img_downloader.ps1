$api_key = "Oau7uZZxM7OcAeFFKqUpQovjDpHDN1xHRS3QZSGx"

$dates = Get-Content "$PSScriptRoot/img_dates.txt"

New-Item -ItemType Directory -Force -Path C:\temp

foreach($d in $dates)
{
    $earth_date = ([datetime]$d).ToString("yyyy-MM-dd");
    $NASA_IMG_API = "https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?earth_date=$earth_date&api_key=$api_key"
    $a = Invoke-WebRequest -Uri $NASA_IMG_API -Method GET
 
    $photos = ($a.Content | ConvertFrom-Json).photos

    New-Item -ItemType Directory -Force -Path "C:\temp\$earth_date"

    foreach($photo in $photos)
    {
        $filename = $photo.id
        write-host "Downloading $filename . . ."
        Invoke-WebRequest $photo.img_src -OutFile "C:\temp\$earth_date\$filename.jpg"
    }
}


