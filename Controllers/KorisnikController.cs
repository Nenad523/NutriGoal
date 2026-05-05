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

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var noviKorisnik = new Korisnici
                {
                   Email = model.Email,
                   PasswordHash = hashedPassword,
                   DatumRegistracije = DateTime.Now,
                   Uloga = "Korisnik"
                };

                db.Korisnici.Add(noviKorisnik);
                db.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // @desc - Prikaz forme za prijavu korisnika
        // @route - GET: /Korisnik/Login
        // @access - Public
        public ActionResult Login()
        {
            return View();
        }

        // @desc - Prijavljivanje korisnika na nalog
        // @route - POST: /Korisnik/Login
        // @access - Public
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postoji = db.Korisnici.FirstOrDefault(k => k.Email == model.Email);

                if (postoji == null)
                {
                    ModelState.AddModelError("Email", "Nalog sa ovom email adresom ne postoji.");
                    return View(model);
                }

                // Provjeri lozinku na postojećem korisniku
                if (!BCrypt.Net.BCrypt.Verify(model.Password, postoji.PasswordHash))
                {
                    ModelState.AddModelError("Password", "Lozinka je neispravna.");
                    return View(model);
                }
                 
                Session["KorisnikId"] = postoji.Id;
                Session["KorisnikEmail"] = postoji.Email;
                Session["KorisnikUloga"] = postoji.Uloga;
                Session["KorisnikIme"] = postoji.KorisnickiProfil.FirstOrDefault()?.Ime; // Pretpostavljamo da korisnik ima profil i da profil ima ime
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // @desc - Odjava korisnika sa naloga
        // @route - GET: /Korisnik/Logout
        // @access - Private
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // @desc - Prikaz stranice sa korisničkim profilom
        // @route - GET: /Korisnik/Profil
        // @access - Private
        public ActionResult Profil()
        {
            if (Session["KorisnikId"] == null)
            {
                return RedirectToAction("Login");
            }

            var korisnikId = (int)Session["KorisnikId"];
            var profil = db.KorisnickiProfil.FirstOrDefault(kp => kp.KorisnikId == korisnikId);

            var model = new ProfilViewModel();

            if (profil != null)
            {
                // Popuni model sa postojećim podacima
                model.Ime = profil.Ime;
                model.Prezime = profil.Prezime;
                model.DatumRodjenja = profil.DatumRodjenja;
                model.Pol = profil.Pol;
                model.Visina = profil.Visina;
                model.Tezina = profil.Tezina;
                model.NivoAktivnosti = profil.NivoAktivnosti;
                model.CiljId = profil.CiljId;
            }

            ViewBag.Ciljevi = db.Ciljevi.ToList();
            return View(model);
        }
    }
}