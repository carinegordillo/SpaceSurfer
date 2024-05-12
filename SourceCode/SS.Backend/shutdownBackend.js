const { exec } = require('child_process');
const { kill } = require('process');

function killProcess(processname, port) {
    const command = process.platform === 'win32' ? 
        `for /f "tokens=5" %a in ('netstat -aon ^| findstr :${port}') do taskkill /f /pid %a` :
        `lsof -ti:${port} | xargs kill -9`;

    exec(command, (error, stdout, stderr) => {
        if (error) {
            console.error(`Error: ${error.message}`);
            return;
        }
        if (stderr) {
            console.error(`Error: ${stderr}`);
            return;
        }
        console.log(`Process ${processname} on port ${port} has been terminated`);
    });
}

killProcess("PersonalOverviewAPI",5275);
killProcess("WaitlistAPI",5099);
killProcess("CompanyAPI",5279);
killProcess("SpaceBookingCenterAPI",5005);
killProcess("RegistrationAPI",5041);
killProcess("LoginAPI",5270);
killProcess("userProfileAPI",5048);
killProcess("TMH",8089);
killProcess("EmailConfirmationsAPI",5116);;
killProcess("Account Recovery",5176);
killProcess("Admin Account Recovery",5138);