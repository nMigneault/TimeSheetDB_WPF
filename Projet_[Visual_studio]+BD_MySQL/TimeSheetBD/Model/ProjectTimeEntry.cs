using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheetBD.Model
{
    /*
     * Cette classe modélise une entrée de temps pour un jour de la semaine.
     * Elle est caractérisé par un attribut Project rerésentant le projet sur lequel
     * a travaillé l'employé et un attribut duration pour la durée de travail de cet
     * employé sur ce projet. L'attribut date, de type DateTime, représente le jour 
     * de la semaine durant lequel l'employé a travaillé sur le projet
     */
    public class ProjectTimeEntry
    {
        private int entryId;
        public int EntryId
        {
            get
            {
                return entryId;
            }
            set
            {
                entryId = value;
            }
        }

        private Employee employee;

        public Employee Employee
        {
            set
            {
                employee = value;
            }
            get
            {
                return employee;
            }
        }

        private Project project;
        public Project Project
        {
            set
            {
                project = value;
            }
            get
            {
                return project;
            }
        }
        private DateTime startDate;
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
            }
        }
        private DateTime endDate;
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                endDate = value;
            }
        }

        private DateTime date;
        public DateTime Date
        {
            set
            {
                date = value;
            }
            get
            {
                return date;
            }
        }
        private int duration;
        public int Duration
        {
            set
            {
                if (value > 0)
                {
                    duration = value;
                }
            }
            get
            {
                return duration;
            }
        }
        public ProjectTimeEntry()
        { }

        public ProjectTimeEntry(Employee employee, Project project, DateTime startDate, DateTime endDate, DateTime date, int duration)
        {

            this.employee = employee;
            this.project = project;
            this.startDate = startDate;
            this.endDate = endDate;
            this.date = date;
            this.duration = duration;

        }
        public ProjectTimeEntry(int entryId, Employee employee, Project project, DateTime startDate, DateTime endDate, DateTime date, int duration)
        {
            this.entryId = entryId;
            this.employee = employee;
            this.project = project;
            this.startDate = startDate;
            this.endDate = endDate;
            this.date = date;
            this.duration = duration;

        }

    }
}
