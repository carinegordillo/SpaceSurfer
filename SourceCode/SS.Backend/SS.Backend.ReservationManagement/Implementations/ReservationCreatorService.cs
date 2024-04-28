using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using SS.Backend.Waitlist;
using SS.Backend.Services.LoggingService;
using Microsoft.Data.SqlClient;
using System.Data;
using System;

/* 
objects needed :
user reservation model - give detials about the user reservtaions 
compnay profile - give details about the company profile 
space

*/

namespace SS.Backend.ReservationManagement{


    public class ReservationCreatorService : IReservationCreatorService
    {
        private IReservationManagementRepository _reservationManagementRepository;
        private readonly WaitlistService _waitlist;

        private LogEntryBuilder logBuilder = new LogEntryBuilder();

        private LogEntry logEntry;

        public ReservationCreatorService(IReservationManagementRepository reservationManagementRepository, WaitlistService waitlist)
        {
            _reservationManagementRepository = reservationManagementRepository;
            _waitlist = waitlist;
        }


        public async Task<Response> CreateReservationWithAutoIDAsync(string tableName, UserReservationsModel userReservationsModel){
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
                        {
                            { "companyID", userReservationsModel.CompanyID },
                            { "floorPlanID", userReservationsModel.FloorPlanID},
                            { "spaceID", userReservationsModel.SpaceID },
                            { "reservationStartTime", userReservationsModel.ReservationStartTime },
                            { "reservationEndTime", userReservationsModel.ReservationEndTime },
                            { "status", userReservationsModel.Status.ToString()},
                            {"userHash", userReservationsModel.UserHash}
                        };

            var InsertReservationCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await _reservationManagementRepository.ExecuteInsertIntoReservationsTable(InsertReservationCommand);


            if (response.HasError == false){
                response.ErrorMessage += "- CreateReservationWithAutoIDAsync - command successful - ";

                logEntry = logBuilder.Info().DataStore().Description($"Successfully created Auto ID reservation at {userReservationsModel.CompanyID}").User(userReservationsModel.UserHash).Build();

                

                //Waitlist Service
                int compid = userReservationsModel.CompanyID;
                int floorid = userReservationsModel.FloorPlanID;
                string spaceid = userReservationsModel.SpaceID;
                DateTime start = userReservationsModel.ReservationStartTime;
                DateTime end = userReservationsModel.ReservationEndTime;
                int resId = await _waitlist.GetReservationID(tableName, compid, floorid, spaceid, start, end);
                await _waitlist.InsertApprovedUser(userReservationsModel.UserHash, resId);
                if (response.HasError == false)
                {
                    response.ErrorMessage += "Successfully added to waitlist table.";
                }
                else
                {
                    response.ErrorMessage += $"Error adding to waitlist table.";
                }
            }
            else{
                logEntry = logBuilder.Error().DataStore().Description($"Failed to create Auto ID reservation at {userReservationsModel.CompanyID}").User(userReservationsModel.UserHash).Build();
                response.ErrorMessage += $"- CreateReservationWithAutoIDAsync - command : {InsertReservationCommand.CommandText} not successful - \n ErrorMessage {response.ErrorMessage} \n HasError {response.HasError} ";

            }

            _reservationManagementRepository.LogTask(logEntry); 
            return response;


        }

        public async Task<Response> CreateReservationWithManualIDAsync(string tableName, UserReservationsModel userReservationsModel){
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();
           
            

            var parameters = new Dictionary<string, object>
                        {
                            { "reservationID", userReservationsModel.ReservationID},
                            { "companyID", userReservationsModel.CompanyID },
                            { "floorPlanID", userReservationsModel.FloorPlanID},
                            { "spaceID", userReservationsModel.SpaceID },
                            { "reservationStartTime", userReservationsModel.ReservationStartTime },
                            { "reservationEndTime", userReservationsModel.ReservationEndTime },
                            { "status", userReservationsModel.Status.ToString()},
                             {"userHash", userReservationsModel.UserHash}
                        };

            var InsertReservationCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();
            
            Console.WriteLine(InsertReservationCommand.CommandText);

            response = await _reservationManagementRepository.ExecuteInsertIntoReservationsTable(InsertReservationCommand);

            if (response.HasError == false){
                response.HasError = false;
                logEntry = logBuilder.Info().DataStore().Description($"Successfully created Manual ID reservation ({userReservationsModel.ReservationID}) at {userReservationsModel.CompanyID}").User(userReservationsModel.UserHash).Build();

                _reservationManagementRepository.LogTask(logEntry); 
    
            }
            else{
                response.HasError = true;
                logEntry = logBuilder.Info().DataStore().Description($"Failed to create Manual ID reservation ({userReservationsModel.ReservationID}) at {userReservationsModel.CompanyID}").User(userReservationsModel.UserHash).Build();

                response.ErrorMessage = $"- CreateReservationWithManualIDAsync - command : {InsertReservationCommand.CommandText} not successful - ";
                

            }
            _reservationManagementRepository.LogTask(logEntry);

            return response;

        }

    }
}
