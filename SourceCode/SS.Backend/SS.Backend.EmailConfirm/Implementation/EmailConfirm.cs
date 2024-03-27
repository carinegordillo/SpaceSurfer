using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using SS.Backend.Services.LogginSerivces;
using SS.Backend.Services.CalendarCreator;
using SS.Backend.Security;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace SS.Backend.EmailConfirm
{
    public class EmailConfirm : IEmailConfirm
    {
        private readonly IEmailConfirmDAO _emailDAO;

        public EmailConfirm(IEmailConfirmDAO emailDAO)
        {
            _emailDAO = emailDAO;
        }

        public async Task<Response> SendConfirmation(int reservationID)
        {
            Response response = new Response();
            var reservationInfo = await _emailDAO.GetReservationInfo(reservationID);
            
            var calendarFilePath = "SSReservation.ics";
            


        }
    }
}