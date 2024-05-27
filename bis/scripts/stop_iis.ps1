# Stop IIS
Write-Output "Stopping IIS..."
Stop-Service -Name 'W3SVC' -Force -ErrorAction Stop
Write-Output "IIS stopped."
