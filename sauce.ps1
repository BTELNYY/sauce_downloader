<# This form was created using POSHGUI.com  a free online gui designer for PowerShell
.NAME
    Untitled
#>

Add-Type -AssemblyName System.Windows.Forms
[System.Windows.Forms.Application]::EnableVisualStyles()
$newline = [System.Environment]::Newline
remove-item .\log.txt
function search {
    $stopwatch =  [system.diagnostics.stopwatch]::StartNew()
    $stopwatch.Start
    $code = $TextBox1.Text
    $WebResponse = Invoke-WebRequest "https://nhentai.to/g/$code" 
    $perfecturl = "https://nhentai.to/g/$code"
    $test = $WebResponse.Images | Select-Object data-src | Out-String -stream -width 60 | Select-String -Pattern "https"
    <#$test = $test -split "`r`n"
    add-content .\temp.txt "$test"
    $TextBox2.Text = $test
    $TextBox1.ReadOnly = $true
    $urls = $TextBox2.Text
    foreach($url in $urls){
        $url = $url -split "`r`n"
        write-host $url
        $FileName = Split-Path $url -Leaf
        Invoke-WebRequest [System.URI]$url -OutFile $FileName      
    }
    $TextBox1.ReadOnly = $false #>
     #open destination folder (and create it if needed)

# script parameters, feel free to change it 
$nrOfImages = 12

# create a WebClient instance that will handle Network communications 
$webClient = New-Object System.Net.WebClient

# load System.Web so we can use HttpUtility
Add-Type -AssemblyName System.Web

# URL encode our search query
$searchQuery = [System.Web.HttpUtility]::UrlEncode($searchFor)

$url = $perfecturl

# get the HTML from resulting search response
$webpage = $webclient.DownloadString($url)

# use a 'fancy' regular expression to finds Urls terminating with '.jpg' or '.png'
$regex = "[(http(s)?):\/\/(www\.)?a-z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-z0-9@:%_\+.~#?&//=]*)((.jpg(\/)?)|(.png(\/)?)){1}(?!([\w\/]+))"

$listImgUrls = $webpage | Select-String -pattern $regex -Allmatches | ForEach-Object {$_.Matches} | Select-Object $_.Value -Unique

# let's figure out if the folder we will use to store the downloaded images already exists
$downloadFolder = ".\$code"
if((Test-Path $downloadFolder) -eq $false) 
{
  add-content .\log.txt "Creating '$downloadFolder'..."

  New-Item -ItemType Directory -Path $downloadFolder | Out-Null
}


foreach($imgUrlString in $listImgUrls) 
{
  [Uri]$imgUri = New-Object System.Uri -ArgumentList $imgUrlString

  # this is a way to extract the image name from the Url
  $imgFile = [System.IO.Path]::GetFileName($imgUri.LocalPath)

  # build the full path to the target download location
  $imgSaveDestination = Join-Path $downloadFolder $imgFile

  Add-Content .\log.txt -value "Downloading '$imgUrlString' to '$imgSaveDestination'..."
  Invoke-WebRequest $imgUri -OutFile $imgSaveDestination
  
  $counter = $stopwatch | Select Elapsed
  Add-Content .\log.txt $counter
  $stopwatch.Stop
  $TextBox2.Text = get-content ".\log.txt"
  $Form.refresh
}
}

#the gui itself
$Form                            = New-Object system.Windows.Forms.Form
$Form.ClientSize                 = New-Object System.Drawing.Point(548,443)
$Form.text                       = "Sauce Finder"
$Form.TopMost                    = $false
$Form.FormBorderStyle            = 'Fixed3D'
$Form.maximizeBox                = $false
$Form.TopMost                    = $false

$TextBox1                        = New-Object system.Windows.Forms.TextBox
$TextBox1.multiline              = $false
$TextBox1.text                   = "Enter Code..."
$TextBox1.width                  = 200
$TextBox1.height                 = 20
$TextBox1.location               = New-Object System.Drawing.Point(9,64)
$TextBox1.Font                   = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$Label1                          = New-Object system.Windows.Forms.Label
$Label1.text                     = "BTELNYY`'s Sauce finder v 1.0"
$Label1.AutoSize                 = $true
$Label1.width                    = 25
$Label1.height                   = 10
$Label1.location                 = New-Object System.Drawing.Point(8,26)
$Label1.Font                     = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$CheckBox1                       = New-Object system.Windows.Forms.CheckBox
$CheckBox1.text                  = "nhentai"
$CheckBox1.AutoSize              = $false
$CheckBox1.width                 = 95
$CheckBox1.height                = 20
$CheckBox1.location              = New-Object System.Drawing.Point(13,100)
$CheckBox1.Font                  = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$CheckBox2                       = New-Object system.Windows.Forms.CheckBox
$CheckBox2.text                  = "reddit"
$CheckBox2.AutoSize              = $false
$CheckBox2.width                 = 60
$CheckBox2.height                = 20
$CheckBox2.location              = New-Object System.Drawing.Point(13,130)
$CheckBox2.Font                  = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$CheckBox3                       = New-Object system.Windows.Forms.CheckBox
$CheckBox3.text                  = "pornhub"
$CheckBox3.AutoSize              = $false
$CheckBox3.width                 = 95
$CheckBox3.height                = 20
$CheckBox3.location              = New-Object System.Drawing.Point(75,130)
$CheckBox3.Font                  = New-Object System.Drawing.Font('Microsoft Sans Serif',10)

$Button1                         = New-Object system.Windows.Forms.Button
$Button1.text                    = "Go."
$Button1.width                   = 60
$Button1.height                  = 30
$Button1.location                = New-Object System.Drawing.Point(487,411)
$Button1.Font                    = New-Object System.Drawing.Font('Microsoft Sans Serif',10)
$Button1.Add_Click({
  search 
})


$TextBox2                        = New-Object system.Windows.Forms.TextBox
$TextBox2.AutoSize               = $false 
$TextBox2.MultiLine              = $true
$TextBox2.Enabled                = $true
$TextBox2.Scrollbars             = "Vertical"
$TextBox2.ReadOnly               = $true
$TextBox2.width                  = 528
$TextBox2.height                 = 247
$TextBox2.location               = New-Object System.Drawing.Point(9,166)
$TextBox2.Font                   = New-Object System.Drawing.Font('Microsoft Sans Serif',10)
#shows the gui
$Form.controls.AddRange(@($TextBox1,$Label1,$Button1,$TextBox2))
#makes sure it doesnt return "cancel" when closed!
[void] $Form.ShowDialog()