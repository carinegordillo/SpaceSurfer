# #!/bin/bash
# Start LoginAPI
dotnet run --project ../SS.Backend/LoginAPI/AuthAPI.csproj &
pid1=$!

# Start RegistrationAPI
dotnet run --project ../SS.Backend/RegistrationAPI/Registration.csproj &
pid2=$!

# Start CompanyAPI
dotnet run --project ../SS.Backend/CompanyAPI/CompanyAPI.csproj &
pid3=$!

# Start SpaceBookingCenterAPI
dotnet run --project ../SS.Backend/SpaceBookingCenterAPI/SpaceBookingCenterAPI.csproj &
pid4=$!

# Start WaitlistAPI
dotnet run --project ../SS.Backend/WaitlistApi/WaitlistApi.csproj &
pid5=$!

# Start PersonalOverviewAPI
dotnet run --project ../SS.Backend/PersonalOverview/PersonalOverviewAPI.csproj &
pid6=$!

# Start userProfileAPI
dotnet run --project ../SS.Backend/userProfileAPI/userProfileAPI.csproj &
pid7=$!


dotnet run --project ../SS.Backend/TaskManagerHubAPI/TaskManagerHubAPI.csproj &
pid8=$!

# Start ConfirmationAPI
dotnet run --project ../SS.Backend/EmailConfirmationAPI/EmailConfirmationAPI.csproj &
pid9=$!


# dotnet run --project ../SS.Backend/SS.ConsoleApp/SS.ConsoleApp.csproj &
# pid9=$!

wait  $pid1 $pid2 $pid3 $pid4 $pid5 $pid6 $pid7 $pid8 $pid9
disown
