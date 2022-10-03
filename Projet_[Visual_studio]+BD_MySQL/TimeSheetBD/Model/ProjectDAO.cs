using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TimeSheetBD.Model
{
    public class ProjectDAO
    {
        private MySqlConnection connection = ConnectionFactory.getConnection();

        public ProjectDAO()
        {}

        // Création d'un nouveau projet dans Project.
        public void create(Project project)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO project(projectId, projectName, projectDescription) " +
                               "VALUES(@projectId, @projectName, @projectDescription)";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlParameter projectIdParam = new MySqlParameter("@projectId", project.ProjectId);
                MySqlParameter projectNameParam = new MySqlParameter("@projectName", project.ProjectName);
                MySqlParameter projectDescrParam = new MySqlParameter("@projectDescription", project.ProjectDescription);
                command.Parameters.Add(projectIdParam);
                command.Parameters.Add(projectNameParam);
                command.Parameters.Add(projectDescrParam);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Récupère et retourne un projet de la bdd, s'il existe, à partir du ID d'un projet.
        public Project findById(int projectId)
        {
            Project project = null;
            try
            {
                connection.Open();
                string query = "SELECT projectId, projectName, projectDescription " +
                               "FROM project " +
                               "WHERE projectId = @projectId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlParameter projectIdParam = new MySqlParameter("@projectId", projectId);
                command.Parameters.Add(projectIdParam);
                MySqlDataReader cursor = command.ExecuteReader();
                if (cursor.Read())
                {
                    project = new Project()
                    {
                        ProjectId = cursor.GetInt32(0),
                    };
                }
                connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return project;
        }

        // Récupère et retourne la liste de tous les projets contenus dans la Table Projet de la bdd.
        public List<Project> findAll()
        {
            List<Project> listProjects = new List<Project>();
            try
            {
                connection.Open();
                string query = "SELECT projectId, projectName, projectDescription " +
                               "FROM project ORDER BY projectId ASC";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlDataReader cursor = command.ExecuteReader();
                while (cursor.Read())
                {
                    Project project = new Project()
                    {
                        ProjectId = cursor.GetInt32(0),
                        ProjectName = cursor.GetString(1),
                        ProjectDescription = cursor.GetString(2),
                    };
                    listProjects.Add(project);
                }
                connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return listProjects;
        }

        // Mise à jour d'un projet dans la bdd à partir du ID d'un projet.
        public void update(Project project)
        {
            try
            {
                connection.Open();
                string query = "UPDATE project " +
                               "SET projectName = @projectName , projectDescription = @projectDescription " +
                               "WHERE projectId = @projectId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlParameter projectIdParam = new MySqlParameter("@projectId", project.ProjectId);
                MySqlParameter projectNameParam = new MySqlParameter("@projectName", project.ProjectName);
                MySqlParameter projectDescrParam = new MySqlParameter("@projectDescription", project.ProjectDescription);
                command.Parameters.Add(projectIdParam);
                command.Parameters.Add(projectNameParam);
                command.Parameters.Add(projectDescrParam);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Suppression d'un projet de la bdd à partir du ID d'un projet.
        public void delete(int projectId)
        {
            try
            {
                connection.Open();
                string query = "DELETE FROM project " +
                               "WHERE projectId = @projectId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                MySqlParameter projectIdParam = new MySqlParameter("@projectId", projectId);
                command.Parameters.Add(projectIdParam);
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

