using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TimeSheetBD.Model
{
    public class EmployeeDao
    {
        private MySqlConnection connection = ConnectionFactory.getConnection();
        public EmployeeDao()
        { }
        public void create(Employee employee)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO Employee(employeeId, firstName, lastName, address, gender, login, password, role) " +
                               "VALUES(@employeeId, @firstName, @lastName, @address, @gender, @login, SHA1(@password), @role)";

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@employeeId", employee.EmployeeId);
                command.Parameters.AddWithValue("@firstName", employee.FirstName);
                command.Parameters.AddWithValue("@lastName", employee.LastName);
                command.Parameters.AddWithValue("@address", employee.Address);
                command.Parameters.AddWithValue("@gender", employee.Gender);
                command.Parameters.AddWithValue("@login", employee.Login);
                command.Parameters.AddWithValue("@password", employee.Password);
                command.Parameters.AddWithValue("@role", employee.Role);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                connection.Close();
            }
            
        }
        public Employee findById(int employeeId)
        {
            Employee employee = null;
            try
            {
                connection.Open();
                string query = "SELECT employeeId, firstName, lastName, address, gender, login, password, role " +
                               "FROM Employee " +
                               "WHERE employeeId = @employeeId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@employeeId", employeeId);
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

        public List<Employee> findAll()
        {
            List<Employee> listEmployee = new List<Employee>();
            try
            {
                connection.Open();
                string query = "SELECT employeeId, firstName, lastName, address, gender, login, password, role " +
                               "FROM Employee " +
                               "ORDER BY employeeId ASC";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlDataReader cursor = command.ExecuteReader();
                while (cursor.Read())
                {
                    Employee employee = new Employee()
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
                    listEmployee.Add(employee);
                }
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                connection.Close();
            }
            return listEmployee;
        }

        public void update(Employee employee)
        {
            try
            {
                connection.Open();
                string query = "UPDATE Employee SET firstName = @firstName , lastName = @lastName, address = @address," +
                               " gender = @gender, login = @login, password = SHA1(@password), role = @role " +
                               "WHERE employeeId = @employeeId";

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@employeeId", employee.EmployeeId);
                command.Parameters.AddWithValue("@firstName", employee.FirstName);
                command.Parameters.AddWithValue("@lastName", employee.LastName);
                command.Parameters.AddWithValue("@address", employee.Address);
                command.Parameters.AddWithValue("@gender", employee.Gender);
                command.Parameters.AddWithValue("@login", employee.Login);
                command.Parameters.AddWithValue("@password", employee.Password);
                command.Parameters.AddWithValue("@role", employee.Role);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                connection.Close();
            }
        }

        public void delete(int employeeId)
        {
            try
            {
                connection.Open();
                string query = "DELETE " +
                               "FROM Employee " +
                               "WHERE employeeId = @employeeId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@employeeId", employeeId);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                connection.Close();
            }
        }
    }
}
