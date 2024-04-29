# Run using the command below:

#   powershell -ExecutionPolicy Bypass -File .\startupBackend.ps1

# Array of relative paths to your WebAPI directories
$apiDirectories = @(
    "LoginAPI",
    "RegistrationAPI",
    "CompanyAPI",
    "SpaceBookingCenterAPI",
    "WaitlistApi",
    "PersonalOverview",
    "userProfileAPI"
)

# Get the current directory of the script
$currentDirectory = Split-Path -Path $MyInvocation.MyCommand.Path

Write-Host "Current directory: $currentDirectory"

# Loop through each API directory
foreach ($directory in $apiDirectories) {
    $fullDirectory = Join-Path -Path $currentDirectory -ChildPath $directory  # Create full path to the directory
    Write-Host "Checking for projects in $fullDirectory..."
    
    # Get all .csproj files in the directory
    $projects = Get-ChildItem -Path $fullDirectory -Filter "*.csproj"
    
    # Check if any .csproj files are found
    if ($projects.Count -gt 0) {
        foreach ($project in $projects) {
            $fullPath = $project.FullName
            
            # Encapsulate paths with spaces in quotes
            if ($fullPath.Contains(" ")) {
                $fullPath = '"' + $fullPath + '"'
            }
            
            Write-Host "Starting $fullPath..."
            Start-Process "dotnet" -ArgumentList "run", "--project", $fullPath
            
            # Wait for 3 seconds
            Start-Sleep -Seconds 3
        }
    }
    else {
        Write-Host "No project files found in $fullDirectory"
    }
}
