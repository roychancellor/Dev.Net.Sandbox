# Set target directory
$serverLetter = "A"
$targetDir = "C:\dev.net\Sandbox\_AppData\Server$($serverLetter)"

# Create directory if it doesn't exist
if (-not (Test-Path $targetDir)) {
    New-Item -Path $targetDir -ItemType Directory
}

# Start time is today at midnight
$startTime = [datetime]::Today
# End time is today at 23:59:59
$endTime = $startTime.AddDays(1).AddSeconds(-1)

# Time increment: 10 minutes
$increment = [timespan]::FromMinutes(10)

# Counter for file names
$fileIndex = 1

# Initialize last timestamp for JSON entry
$previousTimestamp = $startTime

# Loop through time range
$currentTime = $startTime
while ($currentTime -le $endTime) {
    # File name
    $fileName = "LogFile$($serverLetter)$($fileIndex).json"
    $filePath = Join-Path $targetDir $fileName

    # Create a list to store JSON entries as individual strings
    $jsonEntries = @()

    # For each file, generate five log entries
    for ($entryIndex = 1; $entryIndex -le 5; $entryIndex++) {
        # Increment timestamp for this log entry
        $entryTimestamp = $previousTimestamp.AddMinutes($entryIndex * 2) # Increment each by 2 minutes
        
        # Create a log entry object
        $logEntry = @{
            Id        = "IBCID1234-$($serverLetter)$($fileIndex)-$($entryIndex)"
            Message   = "FileA$($fileIndex)-Entry$($entryIndex),ABC$($entryIndex)"
            Timestamp = $entryTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
            Level     = "INFO"
            User      = "USR123"
        }

        # Convert the log entry to a single-line JSON and add it to the list
        $jsonEntries += ($logEntry | ConvertTo-Json -Depth 3 -Compress)
    }

    # Write each JSON entry on a new line (without additional formatting)
    $jsonEntries | ForEach-Object { $_.Trim() } | Out-File -FilePath $filePath -Encoding UTF8

    # Set LastWriteTime for the file
    (Get-Item $filePath).LastWriteTime = $currentTime

    # Increment to the next time slot
    $previousTimestamp = $currentTime
    $currentTime = $currentTime.Add($increment)
    $fileIndex++
}

Write-Host "Created $($fileIndex - 1) log files with multiple JSON entries in $targetDir"
