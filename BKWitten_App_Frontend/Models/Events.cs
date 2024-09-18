using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BKWitten_App_Frontend.Models
{
    public class Events
    {
        public int userID { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public int end_date { get; set; }
        public int start_date { get; set; }
        public int eventID { get; set; }

        public Events() { }
    }
}
