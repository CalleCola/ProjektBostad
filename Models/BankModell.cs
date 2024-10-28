using Microsoft.Build.Framework;

namespace ProjektBostad.Models
{
    public class BankModell
    {

        
        public string Fornamn { get; set; }
      
        public string Efternamn {  get; set; }
       
        public string Email { get; set; }
        
        public int Kontonummer {  get; set; }
        
        public string Fakturering {  get; set; }   
    }
}
