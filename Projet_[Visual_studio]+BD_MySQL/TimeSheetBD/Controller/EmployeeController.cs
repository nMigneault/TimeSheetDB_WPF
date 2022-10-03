using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheetBD.Model;

namespace TimeSheetBD.Controller
{
    public class EmployeeController
    {
        EmployeeDao employeeDao = new EmployeeDao();
        public EmployeeController()
        {
        }

        public void createEmployee(Employee employee)
        {
            employeeDao.create(employee);
        }

        public void updateEmployee(Employee employee)
        {
            employeeDao.update(employee);
        }

        public void deleteEmployee(int employeeId)
        {
            employeeDao.delete(employeeId);
        }

        public List<Employee> getEmployeeList()
        {
            return employeeDao.findAll();
        }
    }
}
