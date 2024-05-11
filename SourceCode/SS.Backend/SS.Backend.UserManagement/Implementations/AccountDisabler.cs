using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SS.Backend.SharedNamespace;
using SS.Backend.Services.LoggingService;


namespace SS.Backend.UserManagement
{
    public class AccountDisabler : IAccountDisabler
    {

        private readonly IUserManagementDao _userManagementDao;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;
        public AccountDisabler(IUserManagementDao userManagementDao, ILogger logger)
        {
            _userManagementDao = userManagementDao;
            _logger = logger;
            logEntry = logBuilder.Build();
        }


        public async Task<Response> DisableAccount(string username){

            string userhash = await _userManagementDao.GetHashByEmail(username);

            Response result = await _userManagementDao.GeneralModifier("hashedUsername", userhash, "IsActive", "no", "dbo.activeAccount");

            if (result.HasError == false){
                result.ErrorMessage += "- Updated account status to diasbled successful -";
            }
            else{
                 result.ErrorMessage += "- Could not update account status to disabled - ";

            }

            //logging
            if (result.HasError == false)
            {
                logEntry = logBuilder.Info().DataStore().Description($"Disabled account successfully.").User(userhash).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Error disabling account.").User(userhash).Build();
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return result;
        }

    }
}