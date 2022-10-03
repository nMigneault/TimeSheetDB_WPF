using System;
using System.Collections.Generic;
using TimeSheetBD.View;
using MySql.Data.MySqlClient;
using System.Windows;

namespace TimeSheetBD.Model
{
    public class ProjectTimeEntryDao
    {
        private MySqlConnection connection = ConnectionFactory.getConnection();

        public ProjectTimeEntryDao()
        { }

        /**
         * Création d'une nouvelle entrée sur ProjectTimeEntry. 
         * */
        public void create(ProjectTimeEntry projectTimeEntry)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO ProjectTimeEntry(employeeId, projectId, startDate, endDate, projectEntryDate, duration) " +
                               "VALUES(@employeeId, @projectId, @startDate, @endDate, @projectEntryDate, @duration)";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlParameter employeeIdParam = new MySqlParameter("@employeeId", projectTimeEntry.Employee.EmployeeId);
                MySqlParameter projectIdParam = new MySqlParameter("@projectId", projectTimeEntry.Project.ProjectId);
                MySqlParameter startDateParam = new MySqlParameter("@startDate", projectTimeEntry.StartDate);
                MySqlParameter endDateParam = new MySqlParameter("@endDate", projectTimeEntry.EndDate);
                MySqlParameter projectEntryDateParam = new MySqlParameter("@projectEntryDate", projectTimeEntry.Date);
                MySqlParameter durationParam = new MySqlParameter("@duration", projectTimeEntry.Duration);
                command.Parameters.Add(employeeIdParam);
                command.Parameters.Add(projectIdParam);
                command.Parameters.Add(startDateParam);
                command.Parameters.Add(endDateParam);
                command.Parameters.Add(projectEntryDateParam);
                command.Parameters.Add(durationParam);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
         * DAO pour la recherche d'une entrée sur ProjectTimeEntry. Comme il n'y a q'une clé primaire, mais trois, la recherche se fera à partir de la combinaison des trois clés. 
         * */

        public ProjectTimeEntry findById(int entryId)
        {
            ProjectTimeEntry projectTimeEntry = null;
            try
            {
                connection.Open();
                string query = "SELECT entryId, employeeId, projectId, startDate, endDate, projectEntryDate, duration " +
                               "FROM ProjectTimeEntry " +
                               "WHERE entryId = @entryId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlParameter entryIdParam = new MySqlParameter("@entryId", entryId);
                command.Parameters.Add(entryIdParam);
                MySqlDataReader cursor = command.ExecuteReader();
                if (cursor.Read())
                {
                    Employee employee = new Employee();
                    Project project = new Project();
                    employee.EmployeeId = cursor.GetInt32(1);
                    project.ProjectId = cursor.GetInt32(2);
                    projectTimeEntry = new ProjectTimeEntry()
                    {
                        EntryId = cursor.GetInt32(0),
                        Employee = employee,
                        Project = project,
                        StartDate = cursor.GetDateTime(3),
                        EndDate = cursor.GetDateTime(4),
                        Date = cursor.GetDateTime(5),
                        Duration = cursor.GetInt32(6)
                    };
                }
                connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return projectTimeEntry;
        }

        public List<ProjectTimeEntry> findAll()
        {
            List<ProjectTimeEntry> listProjectTimeEntry = new List<ProjectTimeEntry>();


            try
            {
                connection.Open();
                string query = "SELECT entryId, employeeId, projectId, startDate, endDate, projectEntryDate, duration " +
                               "FROM ProjectTimeEntry " +
                               "ORDER BY employeeId ASC";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlDataReader cursor = command.ExecuteReader();
                while (cursor.Read())
                {
                    Employee employee = new Employee();
                    Project project = new Project();
                    employee.EmployeeId = cursor.GetInt32(1);
                    project.ProjectId = cursor.GetInt32(2);
                    ProjectTimeEntry projectTimeEntry = new ProjectTimeEntry()
                    {
                        EntryId = cursor.GetInt32(0),
                        Employee = employee,
                        Project = project,
                        StartDate = cursor.GetDateTime(3),
                        EndDate = cursor.GetDateTime(4),
                        Date = cursor.GetDateTime(5),
                        Duration = cursor.GetInt32(6)
                    };
                    listProjectTimeEntry.Add(projectTimeEntry);
                }
                connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listProjectTimeEntry;
        }


        public void update(ProjectTimeEntry projectTimeEntry) //VOIR SI ÇA MARCHE SANS TOUCHER
        {
            try
            {
                connection.Open();
                string query = "UPDATE ProjectTimeEntry SET employeeId = @employeeId, projectId = @projectId, startDate = @startDate, " +
                                "endDate = @endDate, projectEntryDate = @projectEntryDate, duration = @duration " +
                               "WHERE entryId = @entryId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlParameter entryIdParam = new MySqlParameter("@entryId", projectTimeEntry.EntryId);
                MySqlParameter employeeIdParam = new MySqlParameter("@employeeId", projectTimeEntry.Employee.EmployeeId);
                MySqlParameter projectIdParam = new MySqlParameter("@projectId", projectTimeEntry.Project.ProjectId);
                MySqlParameter startDateParam = new MySqlParameter("@startDate", projectTimeEntry.StartDate);
                MySqlParameter endDateParam = new MySqlParameter("@endDate", projectTimeEntry.EndDate);
                MySqlParameter projectDateParam = new MySqlParameter("@projectEntryDate", projectTimeEntry.Date);
                MySqlParameter durationParam = new MySqlParameter("@duration", projectTimeEntry.Duration);

                command.Parameters.Add(entryIdParam);
                command.Parameters.Add(employeeIdParam);
                command.Parameters.Add(projectIdParam);
                command.Parameters.Add(startDateParam);
                command.Parameters.Add(endDateParam);
                command.Parameters.Add(projectDateParam);
                command.Parameters.Add(durationParam);

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                //MessageBox.Show("Erreur update : " + e.Message);
                throw new Exception(e.Message);
            }
        }

        public void delete(int entryId)
        {
            try
            {
                connection.Open();
                string query = "DELETE FROM projecttimeentry " +
                               "WHERE entryId = @entryId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlParameter entryIdParam = new MySqlParameter("@entryId", entryId);
                command.Parameters.Add(entryIdParam);
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }

        }


    }
}
