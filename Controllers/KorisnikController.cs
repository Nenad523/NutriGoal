using NutriGoal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NutriGoal.Controllers
{
    public class KorisnikController : Controller
    {
        private NutriGoalEntities db = new NutriGoalEntities();

        // @desc - Povlacenje stranice za registraciju korisnika
        // @route - GET: /Korisnik/Register
        // @access - Public 
        public ActionResult Register()
        {
            return View();
        }

        // @desc - Registracija - kreiranje naloga za korisnika
        // @route - POST /Korisnik/Register
        // @access - Public
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postojeci = db.Korisnici.FirstOrDefault(k => k.Email == model.Email);

                if (postojeci != null)
                {
                    ModelState.AddModelError("Email", "Korisnik sa ovim emailom već postoji.");
                    return View(model);
                }

                var noviKorisnik = new Korisnici
                {
                   Email = model.Email,
                   PasswordHash = model.Password, // U stvarnoj aplikaciji, lozinku treba hashirati!
                   DatumRegistracije = DateTime.Now,
                   Uloga = "Korisnik"
                };

                db.Korisnici.Add(noviKorisnik);
                db.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}