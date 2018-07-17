$startTime = $(Get-Date)

$dockerInfo = (docker info).split()
$containerType = $dockerInfo[$dockerInfo.indexOf("OSType:") + 1]

if ($containerType -eq "linux") {
    $command = "docker-compose build"
}
else {
    $command = "docker-compose -f docker-compose.yml -f docker-compose.windows.yml build"
}

Write-Host $command
Invoke-Expression $command

$(Get-Date) - $startTime

# "Beep" from: http://jeffwouters.nl/index.php/2012/03/get-your-geek-on-with-powershell-and-some-music/
[console]::beep(900, 400) 
[console]::beep(1000, 400) 
[console]::beep(800, 400) 
[console]::beep(400, 400) 
[console]::beep(600, 1600)
