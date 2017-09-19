using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace HotelManagementSystem.Models
{
    public class BookingDetails
    {
        static readonly string connectionString = "Server=localhost;Database=master;Trusted_connection = True;";
        public ResultSet resultData = new ResultSet();
        public string message = null;

        public void GetBookingDetails(string confirmationCode)
        {
            if (confirmationCode == null)
                return;

            string queryString = @"SELECT DISTINCT b.confirmationCode, b.checkInDate, b.checkOutDate, b.roomType, b.numOfRoom, g.name, r.price*b.numOfRoom as 'Total Amt. Payed($)'
	                                FROM master.[dbproject].[Booking] b, master.[dbproject].[Guest] g, master.[dbproject].[Room] r
	                                WHERE b.confirmationCode = @confirmationCode
		                            AND b.bookingOf = g.id
		                            AND b.roomType = r.roomType";

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@confirmationCode", confirmationCode);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Connection.Open();
                    if (param != null)
                        command.Parameters.AddRange(param);

                    using (SqlDataReader myReader = command.ExecuteReader())
                    {
                        if (!myReader.HasRows)
                        {
                            message = "No booking found for the given confirmation code";
                        }
                        else
                        {
                            while (myReader.Read())
                            {

                                resultData.confirmationCode = myReader.GetString(0);
                                resultData.checkinDate = myReader.GetDateTime(1);
                                resultData.checkoutDate = myReader.GetDateTime(2);
                                resultData.roomType = myReader.GetString(3);
                                resultData.roomCount = myReader.GetInt32(4);
                                resultData.guestName = myReader.GetString(5);
                                resultData.amtPaid = myReader.GetDouble(6);
                            }
                        }
                    }
                }

            }


        }

        public int GetRoomCountCheckedIn(string confirmationCode)
        {
            if (string.IsNullOrEmpty(confirmationCode))
                return 0;

            string queryString = @" SELECT COUNT(*) as 'Number of rooms checked-In for given booking'
                                    FROM [master].[dbproject].[CheckIn] ci JOIN [master].[dbproject].[Guest] g ON(ci.checkInBy = g.id)
										JOIN [master].[dbproject].[Booking] b ON(b.bookingOf = g.id AND b.checkInDate = ci.checkInDate)
										JOIN [master].[dbproject].[Room] r ON(ci.roomId = r.id)
										WHERE b.confirmationCode = @confirmationCode;";

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@confirmationCode", confirmationCode);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Connection.Open();
                    if (param != null)
                        command.Parameters.AddRange(param);

                    using (SqlDataReader myReader = command.ExecuteReader())
                    {
                        if (myReader.Read())
                        {

                            return myReader.GetInt32(0);
                        }

                    }
                }

            }

            return 0;
        }

    }

    public class ResultSet
    {
        public string confirmationCode { get; set; }
        public DateTime checkinDate { get; set; }
        public DateTime checkoutDate { get; set; }
        public string roomType { get; set; }
        public int roomCount { get; set; }
        public double amtPaid { get; set; }
        public string guestName { get; set; }
        public int roomCountCheckedIn { get; set; }
    }
}