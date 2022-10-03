using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TimeSheetBD.Model;
using TimeSheetBD.Utils;
using TimeSheetBD.View;

namespace TimeSheetBD.Controller
{
    public class ProjectTimeEntryService
    {
        ProjectTimeEntryDao projectTimeEntryDao = new ProjectTimeEntryDao();

        public List<ProjectTimeEntry> getEmployeeProjectTimeEntries(int employeeId)
        {
            List<ProjectTimeEntry> employeeProjectTimeEntries = new List<ProjectTimeEntry>();
            try
            {

                List<ProjectTimeEntry> projectTimeEntries = projectTimeEntryDao.findAll();

                /*
                 * On parcours la liste de toutes les entrées de projet, et on rajoute à la liste 
                 * employeeProjectTimeEntries toutes celles qui ont un employeeId correspondant à celui
                 * fourni en paramètre
                 */
                foreach (ProjectTimeEntry projectTimeEntry in projectTimeEntries)
                {
                    if(projectTimeEntry.Employee.EmployeeId == employeeId)
                    {
                        employeeProjectTimeEntries.Add(projectTimeEntry);
                    }
                }
            }catch(Exception e)
            {
                MessageBox.Show("Erreur  : " + e.Message);
            }


            /*On cast vers List<ProjectTimeEntry> car Where renvoie une référence IEnumerable*/
            return employeeProjectTimeEntries;
        }

        /*
        * Cette méthode permet d'extraire les date de début et de fin d'une semaine
        * correspondant à une TimeCard. Le but c'est de l'utiliser plus tard pour extraire 
        * les entrées de projets liés à une carte de temps
        */
        public List<TimeCard> getEmployeeTimeCards(int employeeId)
        {

            List<TimeCard> timeCards = new List<TimeCard>();
            MySqlConnection connection = ConnectionFactory.getConnection();
            try{
                connection.Open();
                string query = "SELECT employeeId, startDate, endDate FROM ProjectTimeEntry " +
                               "WHERE employeeId = @employeeId";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@employeeId", employeeId);
                MySqlDataReader cursor = command.ExecuteReader();
                while (cursor.Read())
                {
                    TimeCard timeCard = new TimeCard()
                    {
                        StartDate = cursor.GetDateTime(1),
                        EndDate = cursor.GetDateTime(2),
                    };
                    /*
                     * La carte de temps n'est ajoutée à la liste des cartes de temps
                     * de l'employé que si elle n'existe pas déjà
                     */
                    if(!containsTimeCard(timeCards, timeCard)){
                        timeCards.Add(timeCard);
                    }
                }
                connection.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return timeCards;
        }
        
        public List<ProjectTimeEntry> getTimeCardProjectTimeEntries(int employeeId, TimeCard timeCard)
        {
            /*
             * On récupère d'abord toutes entrées de temps de projets associées à l'employé courant
             */

            List<ProjectTimeEntry> employeeProjectTimeEntries = getEmployeeProjectTimeEntries(employeeId);

            /*
             * On extrait les entrées de projets qui nous intéressent par filtrage : si la date 
             * d'une entrée est comprise entre la date de début (lundi) et la date de fin (dimanche)
             * de la carte de temps, elle sera ajoutée à la liste employeeTimeCardEntries.
             */
            List<ProjectTimeEntry> employeeTimeCardEntries = new List<ProjectTimeEntry>();
            foreach(ProjectTimeEntry projectTimeEntry in employeeProjectTimeEntries)
            {
                if(DateTime.Compare(projectTimeEntry.Date, timeCard.StartDate) >=0 && 
                    DateTime.Compare(projectTimeEntry.Date, timeCard.EndDate) <= 0)
                {
                    employeeTimeCardEntries.Add(projectTimeEntry);
                }
            }
            return employeeTimeCardEntries;
        }

        /*
         * Cette méthode extrait les entrées de projets d'une carte de temps d'un employé
         * pour un jour de la semaine
         */
        public List<ProjectTimeEntry> getDayProjectTimeEntries(int employeeId, TimeCard timeCard, 
            DayOfWeek dayOfWeek)
        {
            /*
             * On commence par l'extraction de toutes les entrées de projets de la carte de temps
             * de l'employée, puis on procède à un filtrage : si le jour de la semaine associé à la date
             * de l'entrée de projet correspond au jour reçu en paramètre, on rajoute cette entrée
             * à la liste dayProjectTimeEntries
             */
            List<ProjectTimeEntry> employeeTimeCardEntries = getTimeCardProjectTimeEntries(employeeId,
                timeCard);
            List<ProjectTimeEntry> dayProjectTimeEntries = new List<ProjectTimeEntry>();
            foreach(ProjectTimeEntry projectTimeEntry in employeeTimeCardEntries)
            {
                if(projectTimeEntry.Date.DayOfWeek == dayOfWeek)
                {
                    dayProjectTimeEntries.Add(projectTimeEntry);
                }
            }
            return dayProjectTimeEntries;
        }
        private static bool containsTimeCard(List<TimeCard> timeCards,TimeCard aTimeCard)
        {
            foreach (TimeCard timeCard in timeCards){
                if (timeCard.StartDate.CompareTo(aTimeCard.StartDate) == 0 &&
                    timeCard.EndDate.CompareTo(aTimeCard.EndDate) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public string validateTimeCard(Employee employee, List<ProjectTimeEntry> projectTimeEntries, 
            Message messages)
        {

            RulesValidation.verifyMinWorkHoursPerWeek(employee, projectTimeEntries, messages);
            RulesValidation.verifyMaxWorkHoursPerWeek(employee, projectTimeEntries, messages);
            RulesValidation.verifyMaxRemoteHoursPerWeekAdmin(employee, projectTimeEntries, messages);
            RulesValidation.verifyMinimumWorkedHourPerBusinessDay(employee, projectTimeEntries, messages);
            RulesValidation.verifyMaxTimePerDay(employee, projectTimeEntries, messages);
            RulesValidation.verifySickLeave(employee, projectTimeEntries, messages);
            RulesValidation.verifyHoliday(employee, projectTimeEntries, messages);
            RulesValidation.verifyTransportRules(employee, projectTimeEntries, messages);
            RulesValidation.verifyParentalLeaveRules(employee, projectTimeEntries, messages);
            return messages.ToString();
        }
    }
}
