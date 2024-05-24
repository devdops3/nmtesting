# Start IIS
Write-Output "Starting IIS..."
Start-Service -Name 'W3SVC' -ErrorAction Stop
Write-Output "IIS started."
