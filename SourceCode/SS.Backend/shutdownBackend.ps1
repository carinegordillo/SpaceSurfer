# Run using the command below:
# powershell -ExecutionPolicy Bypass -File .\shutdownBackend.ps1

# Array of ports to shutdown
$ports = @(
    5270,
    5275,
    5048,
    5099,
    5005,
    5279,
    5080
)

foreach ($port in $ports) {
    # Get processes listening on the port
    $processes = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
    if ($processes) {
        Write-Host "Processes found listening on port ${port}:"
        foreach ($process in $processes) {
            $processId = $process.OwningProcess
            
            # Get process name if process exists
            $processName = if (Get-Process -Id $processId -ErrorAction SilentlyContinue) {
                (Get-Process -Id $processId).ProcessName
            } else {
                "Process Name Not Found"
            }
            
            Write-Host "  Process ID: $processId, Name: $processName"
            
            # Stop the process if it's not 'httpd'
            if ($processName -ne "httpd") {
                Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
                if ($?) {
                    Write-Host "  Process $processName (ID: $processId) stopped."
                } else {
                    Write-Host "  Unable to stop process $processName (ID: $processId)"
                }
            } else {
                Write-Host "  Skipping stopping 'httpd' (ID: $processId) as it may require administrative privileges."
            }
        }
    }
    else {
        Write-Host "No processes found listening on port $port"
    }
}
