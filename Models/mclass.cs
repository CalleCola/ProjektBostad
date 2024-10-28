using System.Data;
using System.Data.SqlClient;

namespace ProjektBostad.Models
{
    public class mclass
    {
        public int Sidnr { get; set; }
        public string keywords { get; set; }
        public string description { get; set; } 
        public string Viewport { get; set; }
        public string ogtitle {  get; set; }
        public string ogtype { get; set; }
        public string oglocale { get; set; }

       
    }
}
