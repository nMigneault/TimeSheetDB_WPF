using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSheetBD.Model;

namespace TimeSheetBD.Controller
{
    public class LoginController
    {
        public LoginDao loginDao = new LoginDao();
        public Employee connect(string login, string password) 
        { 
            return loginDao.connect(login, password);   
        }
    }
}
