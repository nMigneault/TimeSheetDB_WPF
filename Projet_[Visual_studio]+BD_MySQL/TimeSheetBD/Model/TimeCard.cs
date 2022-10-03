using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheetBD.Model
{
    public class TimeCard
    {
        private DateTime startDate;
        public DateTime StartDate
        {
            set
            {
                startDate = value;
            }
            get 
            { 
                return startDate; 
            }
        }
        private DateTime endDate;
        public DateTime EndDate
        {
            set
            {
                endDate = value;
            }
            get
            {
                return endDate;
            }
        }
    }
}
