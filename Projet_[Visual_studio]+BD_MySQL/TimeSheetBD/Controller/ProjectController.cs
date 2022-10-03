using System;
using System.Collections.Generic;
using System.Windows;
using TimeSheetBD.Model;

namespace TimeSheetBD.Controller
{
    public class ProjectController
    {
        ProjectDAO projectDao = new ProjectDAO();

        public void createProject(Project project)
        {
            projectDao.create(project);
        }

        public Project getProject(int projectId)
        {
            return projectDao.findById(projectId);
        }
        public void updateProject(Project project)
        {
            projectDao.update(project);
        }

        public void deleteProject(int projectId)
        {
            projectDao.delete(projectId);
        }

        public bool projectExist(int projectId)
        {
            return projectDao.findById(projectId) != null;
        }

        public List<Project> getProjectList()
        {
            return projectDao.findAll();
        }

        public List<String> getStringProjectList()
        {
            List<String> listString = new List<String>();
            List<Project> projectList = projectDao.findAll();
           

            for (int i = 0; i < projectList.Count; i++)
            {
                listString.Add(projectList[i].ProjectId.ToString() + " - " + projectList[i].ProjectName);
            }

            return listString;
        }

        public int getProjectIndex(List<Project> projectList, int projectId)
        {
            int index = -1;
            projectList = projectDao.findAll();
            bool foundIndex = false;
            int i = 0;
            while (!foundIndex)
            {
                if (projectList[i].ProjectId == projectId)
                {
                    foundIndex = true;
                    index = i;
                }
                i++;
            }

            return index;
        }
    }
}
