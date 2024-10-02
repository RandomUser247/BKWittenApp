using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BKWitten_App_Frontend.Models
{
    public class Users
    {
        public int userID { get; set; }
        public string pass_hash { get; set; }
        public string tel_nr { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public bool isAdmin { get; set; }
        public bool isTeacher { get; set; }
        public string job_description { get; set; }

        public Users() { }
    }
}
