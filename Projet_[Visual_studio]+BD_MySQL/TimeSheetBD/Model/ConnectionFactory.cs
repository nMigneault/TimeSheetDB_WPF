using MySql.Data.MySqlClient;
using System;

namespace TimeSheetBD.Model
{
    public class ConnectionFactory
    {
        private static MySqlConnection connection = null;
        private static string connexionString = @"server=localhost;userid=root;database=TimeSheetDB";
        public static MySqlConnection getConnection()
        {
            try
            {
                connection = new MySqlConnection(connexionString);
            }
            catch (MySqlException e)
            {
                throw new Exception(e.Message);
            }
            return connection;
        }
        public void closeConnection()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }
    }
}
