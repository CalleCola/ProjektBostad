using Microsoft.AspNetCore.Mvc;
using ProjektBostad.Models;
using Microsoft.AspNetCore.Http;
using static ProjektBostad.Models.Databas;
using System.Net.Mail;
using System.Net;
using static ProjektBostad.Models.BostadsDetalj;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using Microsoft.CodeAnalysis.CodeStyle;
using System.Diagnostics.Tracing;

namespace ProjektBostad.Controllers
{
        
    public class HemnetController : Controller
    {

        private readonly IEmailSender _emailSender; 
        public HemnetController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
           
            return View("LogIn");
        }

        [HttpPost]
        public IActionResult LogIn(Anvandare anv)
        {
            Databas db = new Databas();
            int i = 0;
            string error = "";
            i = db.LoggaIn(anv, out error);

            ViewBag.antal = i;

            if (i == 1)
            {
                (string hashedLosenord, string salt) = db.GetHashedLosenordAndSaltFromDatabase(anv.Anv_Namn);

                if (hashedLosenord != null && salt != null)
                {
                    bool isValidPassword = BCrypt.Net.BCrypt.Verify(anv.Losenord + salt, hashedLosenord);

                    if (isValidPassword)
                    {
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        HttpContext.Session.SetString("UserName", anv.Anv_Namn);
                        ViewBag.UserName = anv.Anv_Namn;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.Felmeddelande = "Fel användarnamn eller lösenord";
            return View("LogIn");
        }
        public IActionResult KopSida(int Id)
        {
            Databas db = new Databas();
            BostadsDetalj bm = db.GetDetaljer(Id, out string error);
            ViewBag.error = error;
            return View(bm);
        }

        public IActionResult UppdateraResultat()
        {
            return View();
        }
        public IActionResult RaderaResultat()
        {
            return View();
        }
        public IActionResult KontoResultat()
        {
            return View();
        }
        public IActionResult BostadControll()
        {
            List<BostadsDetalj> Bostadslista = new List<BostadsDetalj>(); 
            Databas bm = new Databas();
            string error = "";
            Bostadslista = bm.VisaBostad(out error);
            ViewBag.error = error;
            ViewBag.Antal = HttpContext.Session.GetString("antal"); 
            return View(Bostadslista);
        }
        [HttpGet]
        public IActionResult SkapaAnvandare()
        {
            return View("SkapaAnvandare");
        }
        [HttpPost]
        public IActionResult SkapaAnvandare(Anvandare anv)
        {
            Databas db = new Databas();
            int i = 0;
            string error = "";
            i = db.SkapaAnv(anv, out error);        
            ViewBag.error = error;
            ViewBag.antal = i;
            if (i == 1)
            {
                return RedirectToAction("KontoResultat");
            }
            else
            {
                Console.WriteLine("Det gick inte att skapa ditt konto");
                return View("SkapaAnvandare");
            }
        }
        public IActionResult TaBortAnvandare(Anvandare anv)
        {
            Databas db = new Databas();
            int i = 0;
            string error = "";
            i = db.TaBortAnv(anv, out error);
            HttpContext.Session.SetString("antal", i.ToString());
            if (i == 1)
            {
                
                return RedirectToAction("RaderaResultat");
            }
            else
            {
                Console.WriteLine("Det gick inte att skapa ditt konto");
                return View("TaBortAnvandare");

            }
        }
        public IActionResult UppdateraAnvandare(Anvandare anv)
        {
            Databas db = new Databas();
            int i = 0;
            string error = "";
            i = db.UppdateraAnv(anv, out error);
            HttpContext.Session.SetString("antal", i.ToString());
            if (i == 1)
            {

                return RedirectToAction("UppdateraResultat");
            }
            else
            {
                Console.WriteLine("Det gick inte att skapa ditt konto");
                return View("TaBortAnvandare");

            }
        }
        public IActionResult KopFormular()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> KopFormular(BankModell model)
        {

            Databas db = new Databas();
            int i = 0;
            string error = "";
            i = db.BankUpg(model, out error);
            HttpContext.Session.SetString("antal", i.ToString());
            if (i == 1)
            {
                var subject = "Bekräftelse av köp";
                var message = "Hej, " + model.Fornamn + " Tack för ditt köp. Vi har mottagit din betalning";
                var resultat = "Tack för ditt köp, Vi har skickat ett konfirmationsmail till: " + model.Email + "";
                await _emailSender.SendEmailAsync(model.Email, subject, message);

                ViewBag.Email = resultat;

                return View("Confirmation", model);
             
            }
            else
            {
                Console.WriteLine("Det gick inte regristrera bankuppgifter");
                return View("KopFormular");

            }

           
        }
        public IActionResult Confirmation()
        {
            return View();
        }

      

    }
}
