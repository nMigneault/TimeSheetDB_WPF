using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TimeSheetBD.View;

namespace TimeSheetBD.Model
{
    public class LoginDao
    {
        private MySqlConnection connection = ConnectionFactory.getConnection();
        public Employee connect(string login, string password)
        {
            Employee employee = null;
            try
            {
                connection.Open();
                string query = "SELECT employeeId, firstName, lastName, address, gender, login, password, role FROM Employee" +
                               " WHERE login = @login AND password = SHA1(@password)";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                MySqlDataReader cursor = command.ExecuteReader();
                if (cursor.Read())
                {
                    employee = new Employee()
                    {
                        EmployeeId = cursor.GetInt32(0),
                        FirstName = cursor.GetString(1),
                        LastName = cursor.GetString(2),
                        Address = cursor.GetString(3),
                        Gender = cursor.GetChar(4),
                        Login = cursor.GetString(5),
                        Password = cursor.GetString(6),
                        Role = cursor.GetString(7)
                    };
                }
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                connection.Close();
            }

            return employee;
        }
    }
}
