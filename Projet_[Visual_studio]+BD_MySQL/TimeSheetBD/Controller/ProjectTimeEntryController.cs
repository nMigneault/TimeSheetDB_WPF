using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheetBD.Model;

namespace TimeSheetBD.Controller
{
    public class ProjectTimeEntryController
    {
        ProjectTimeEntryDao projectTimeEntryDao = new ProjectTimeEntryDao();
        public void createProjectTimeEntry(ProjectTimeEntry projectTimeEntry)
        {
            projectTimeEntryDao.create(projectTimeEntry);
        }
        public void updateProjectTimeEntry(ProjectTimeEntry projectTimeEntry)
        {
            projectTimeEntryDao.update(projectTimeEntry);
        }

        public void deleteProjectTimeEntry(int entryId)
        {
            projectTimeEntryDao.delete(entryId);
        }

        public List<ProjectTimeEntry> getProjectTimeEntryList()
        {
            return projectTimeEntryDao.findAll();
        }

        public List<ProjectTimeEntry> getProjectListByEmployeeIdAndDate(int employeeId, DateTime date)
        {
            List<ProjectTimeEntry> projectList = projectTimeEntryDao.findAll();
            List<ProjectTimeEntry> projectListByIdEmployeeAndDate = new List<ProjectTimeEntry>();
            foreach (var projectTimeEntry in projectList)
            {
                if (projectTimeEntry.Employee.EmployeeId == employeeId && projectTimeEntry.Date == date)
                {
                    projectListByIdEmployeeAndDate.Add(projectTimeEntry);
                }
            }


            return projectListByIdEmployeeAndDate;

        }

        public void deleteAllbyEmployeeDate(int employeeId, DateTime date)
        {
            List<ProjectTimeEntry> projectEntryDayList = getProjectListByEmployeeIdAndDate(employeeId, date);

            if(projectEntryDayList.Count> 0)
            {
                for (int i = 0; i < projectEntryDayList.Count; i++)
                {
                    projectTimeEntryDao.delete(projectEntryDayList[i].EntryId);
                }
            }
            

        }

        public bool ContainsProjectTimeEntry(int projectId, int employeeId, DateTime date)
        {
            List<ProjectTimeEntry> projectTimeEntries = new List<ProjectTimeEntry>();
            projectTimeEntries = getProjectListByEmployeeIdAndDate(employeeId, date);
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

        public ProjectTimeEntry getProjectTimeEntry(int entryId)
        {
            return projectTimeEntryDao.findById(entryId);
        }

        /*
         * TODO : vérifier si cette fonction est toujours utilisée avant de supprimer
         * car je crois qu'elle est remplacé par : getProjectListByEmployeeIdAndDate
         */
        public List<ProjectTimeEntry> getProjectListByEmployeeId(int employeeId)
        {
            List<ProjectTimeEntry> projectList = projectTimeEntryDao.findAll();
            List<ProjectTimeEntry> projectListByIdEmployee = new List<ProjectTimeEntry>();
            foreach (var projectTimeEntry in projectList)
            {
                if (projectTimeEntry.Employee.EmployeeId == employeeId)
                {
                    projectListByIdEmployee.Add(projectTimeEntry);
                }
            }
            return projectListByIdEmployee;
        }


    }


}