using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using TimeSheetBD.Model;

namespace TimeSheetBD.Model
{
    public class Employee
    {
        private int employeeId;

        public int EmployeeId
        {
            set
            {
                if (value > 0)
                {
                    employeeId = value;
                }
            }
            get
            {
                return employeeId;
            }

        }
        private string firstName;
        public string FirstName
        {
            set
            {
                firstName = value;
            }
            get
            {
                return firstName;
            }
        }
        private string lastName;
        public string LastName
        {
            set
            {
                lastName = value;
            }
            get
            {
                return lastName;
            }
        }
        private string address;
        public string Address
        {
            set
            {
                address = value;
            }
            get
            {
                return address;
            }
        }

        private char gender;
        public char Gender
        {
            set
            {
                if (value == 'M' || value == 'F')
                {
                    gender = value;
                }
            }
            get
            {
                return gender;
            }
        }

        private string login;
        public string Login
        {
            set
            {
                login = value;
            }
            get
            {
                return login;
            }
        }

        private string password;
        public string Password
        {
            set
            {
                password = value;
            }
            get
            {
                return password;
            }
        }

        private string role;
        public string Role
        {
            set
            {
                if (value.Equals("Manager") || value.Equals("Employee"))
                {
                    role = value;
                }
            }
            get
            {
                return role;
            }
        }

        List<ProjectTimeEntry> projectTimeEntries = new List<ProjectTimeEntry>();
        public Employee()
        {
        }
        public Employee(int employeeID, string firstName, string lastName, string address, char gender,
                       string login, string password, string role)
        {
            if (employeeID > 0 && (gender == 'M' || gender == 'F') 
                               && (role.Equals("Manager") || role.Equals("Employee")))
            {
                this.employeeId = employeeID;
                this.firstName = firstName;
                this.lastName = lastName;
                this.address = address;
                this.gender = gender;
                this.login = login;
                this.password = password;
                this.role = role;
            }
        }

        /*
         * Cette méthode interne vérifie si le jour courant contient une entrée de temps
         * pour le projet dont le projectId est passé en pararmètre. Elle permet d'éviter 
         * la création d'entrée dupliqué pour un même projet durant une même journée de la
         * semaine
         */

        private bool containsProjectTimeEntry(int projectId, DateTime date)
        {
            foreach
                (ProjectTimeEntry projectTimeEntry in projectTimeEntries)
            {
                if (projectTimeEntry.Project.ProjectId == projectId && projectTimeEntry.Date == date)
                {
                    return true;
                }
            }
            return false;
        }

        /*
         *Ajout d'une nouvelle entrée de temps pour un projet 
         */
       /* public bool addProjectTimeEntry(Project project, int duration, DateTime date)
        {
            if (!containsProjectTimeEntry(project.ProjectId, date))
            {
                projectTimeEntries.Add(new ProjectTimeEntry(project, duration, date));
                return true;
            }
            return false;
        } */

        public override string ToString()
        {
            return  "ID : " + EmployeeId + 
                    "\nFirstName : " + FirstName +
                    "\nLastName : " + LastName + 
                    "\nAdresse : " + Address +
                    "\nGender : " + Gender + 
                    "\n LoginUI : " + Login + 
                    "\n Pass : " + Password +
                    "\nRole : " + Role;
        }
    }
}
