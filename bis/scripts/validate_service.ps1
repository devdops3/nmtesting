# Validate that the service is running
Write-Output "Validating IIS service..."
$statusCode = (Invoke-WebRequest -Uri "http://localhost" -UseBasicParsing -DisableKeepAlive).StatusCode
if ($statusCode -eq 200) {
    Write-Output "Validation successful: Service is up and running."
} else {
    Write-Output "Validation failed: Service did not respond with a 200 status."
    Exit 1
}
