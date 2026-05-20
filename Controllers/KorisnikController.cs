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

                var noviProfil = new KorisnickiProfil
                {
                    KorisnikId = noviKorisnik.Id,
                    DatumAzuriranja = DateTime.Now
                };
                db.KorisnickiProfil.Add(noviProfil);
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

                var profil = postoji.KorisnickiProfil.FirstOrDefault();
                Session["KorisnikId"] = postoji.Id;
                Session["KorisnikEmail"] = postoji.Email;
                Session["KorisnikUloga"] = postoji.Uloga;
                Session["KorisnikIme"] = profil?.Ime ?? postoji.Email;

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

        // @desc - Ažuriranje korisničkog profila
        // @route - POST: /Korisnik/Profil
        // @access - Private
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profil(ProfilViewModel model)
        {
            if (Session["KorisnikId"] == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Ciljevi = db.Ciljevi.ToList();
                return View(model);
            }

            var korisnikId = (int)Session["KorisnikId"];
            var postojeciProfil = db.KorisnickiProfil
                .FirstOrDefault(kp => kp.KorisnikId == korisnikId);

            if (postojeciProfil == null)
            {
                // INSERT — novi profil
                var noviProfil = new KorisnickiProfil
                {
                    KorisnikId = korisnikId,
                    Ime = model.Ime,
                    Prezime = model.Prezime,
                    DatumRodjenja = model.DatumRodjenja,
                    Pol = model.Pol,
                    Visina = model.Visina,
                    Tezina = model.Tezina,
                    NivoAktivnosti = model.NivoAktivnosti,
                    CiljId = model.CiljId,
                    DatumAzuriranja = DateTime.Now
                };
                db.KorisnickiProfil.Add(noviProfil);
            }
            else
            {
                // UPDATE — postojeći profil
                postojeciProfil.Ime = model.Ime;
                postojeciProfil.Prezime = model.Prezime;
                postojeciProfil.DatumRodjenja = model.DatumRodjenja;
                postojeciProfil.Pol = model.Pol;
                postojeciProfil.Visina = model.Visina;
                postojeciProfil.Tezina = model.Tezina;
                postojeciProfil.NivoAktivnosti = model.NivoAktivnosti;
                postojeciProfil.CiljId = model.CiljId;
                postojeciProfil.DatumAzuriranja = DateTime.Now;
            }

            db.SaveChanges();
            // Triger se automatski okine ovdje ↑
            // i izračuna BMR, TDEE, PreporucenoKalorija

            return RedirectToAction("Profil");
        }
    
    }
}