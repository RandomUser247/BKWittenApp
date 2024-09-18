using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BKWitten_App_Frontend.Models
{
    public class Posts
    {
        public int postID { get; set; }
        public int userID { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int publish_date { get; set; }
        public int creation_date { get; set; }

        public Posts() { }      
    }
}
