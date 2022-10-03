using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheetBD.Model
{
    public class Project
    {
        private int projectId;
        private string projectName;
        private string projectDescription;
        public int ProjectId
        {
            set
            {
                projectId = value;   
            }
            get
            {
                return projectId;
            }
        }

        public string ProjectName
        {
            set
            {
                projectName = value;
            }
            get
            {
                return projectName;
            }
        }

        public string ProjectDescription
        {
            set
            {
                projectDescription = value;
            }
            get
            {
                return projectDescription;
            }
        }

        public Project()
        { }

        public Project(int projectId)
        {            
            this.projectId = projectId;            
        }

        public Project(int projectId, string projectName, string projectDescription) : this(projectId)
        {
            this.projectName = projectName;
            this.projectDescription = projectDescription;
        }


        public override string ToString()
        {
            return "ProjectID : " + projectId + "\n" + 
                   "ProjectName : " + projectName + "\n" + 
                   "Project Description " + projectDescription;
        }
    }
}
