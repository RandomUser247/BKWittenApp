using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BKWitten_App_Frontend.Models
{
    public class Media
    {
        public int mediaID { get; set; }
        public int postID { get; set; }
        public string alt_text { get; set; }
        public bool isVideo { get; set; }
        public string file_path { get; set; }

        public Media() { }
    }
}
