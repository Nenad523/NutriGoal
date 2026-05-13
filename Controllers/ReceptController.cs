using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NutriGoal.Models;

namespace NutriGoal.Controllers
{
    public class ReceptController : Controller
    {
        private NutriGoalEntities db = new NutriGoalEntities();

        // @desc - Prikazivanje liste recepata
        // @route - GET: /Recept/Index
        // @access - Public AND Private
        public ActionResult Index(bool sviRecepti = false)
        {
            if (Session["KorisnikId"] != null)
            {
                var korisnikId = (int)Session["KorisnikId"];
                var korisnik = db.Korisnici.Find(korisnikId);
               
                // Dohvati alergene korisnika
                var korisnikAlergeni = korisnik.Alergije
                    .Select(a => a.Id)
                    .ToList();

                ViewBag.KorisnikAlergeni = korisnikAlergeni;

                // Dohvati profil korisnika
                var profil = db.KorisnickiProfil
                    .FirstOrDefault(kp => kp.KorisnikId == korisnikId);

                // Ako ima cilj i nije kliknuo "Svi recepti"
                if (profil != null && profil.CiljId != null && !sviRecepti)
                {
                    var preporuceniRecepti = db.sp_PreporuciRecepte(korisnikId)
                        .Select(r => new ReceptViewModel
                        {
                            Id = r.Id,
                            Naziv = r.Naziv,
                            Opis = r.Opis,
                            Fotografija = r.Fotografija,
                            KategorijaId = r.KategorijaId,
                            VrijemePripreme = r.VrijemePripreme,
                            Kalorije = r.Kalorije,
                            Proteini = r.Proteini,
                            UgljeniHidrati = r.UgljeniHidrati,
                            Masti = r.Masti,
                            SadrziAlergen = false // uvijek false jer procedura već filtrira
                        }).ToList();

                    ViewBag.Personalizovano = true;
                    ViewBag.SviRecepti = false;
                    return View(preporuceniRecepti);
                }

                // Prijavljen ali kliknuo "Svi recepti" ili nema cilj
                var sviReceptiLista = db.Recepti
                    .Select(r => new ReceptViewModel
                    {
                        Id = r.Id,
                        Naziv = r.Naziv,
                        Opis = r.Opis,
                        Fotografija = r.Fotografija,
                        KategorijaId = r.KategorijaId,
                        VrijemePripreme = r.VrijemePripreme,
                        Kalorije = r.Kalorije,
                        Proteini = r.Proteini,
                        UgljeniHidrati = r.UgljeniHidrati,
                        Masti = r.Masti,
                        SadrziAlergen = r.ReceptSastojci
                            .Any(rs => rs.Sastojci.Alergije
                                .Any(a => korisnikAlergeni.Contains(a.Id)))
                    }).ToList();

                ViewBag.Personalizovano = false;
                ViewBag.SviRecepti = true;
                return View(sviReceptiLista);
            }

            // Neregistrovani korisnik — svi recepti bez oznaka
            var recepti = db.Recepti
                .Select(r => new ReceptViewModel
                {
                    Id = r.Id,
                    Naziv = r.Naziv,
                    Opis = r.Opis,
                    Fotografija = r.Fotografija,
                    KategorijaId = r.KategorijaId,
                    VrijemePripreme = r.VrijemePripreme,
                    Kalorije = r.Kalorije,
                    Proteini = r.Proteini,
                    UgljeniHidrati = r.UgljeniHidrati,
                    Masti = r.Masti,
                    SadrziAlergen = false
                }).ToList();

            ViewBag.Personalizovano = false;
            ViewBag.SviRecepti = false;
            return View(recepti);
        }

        // @desc - Dodavanje recepta u favorite
        // @route - POST: /Recept/DodajUFavorite
        // @access - Private
        [HttpPost]
        public ActionResult DodajUFavorite(int receptId)
        {
            if (Session["KorisnikId"] == null)
            {
                return RedirectToAction("Login", "Korisnik");
            }

            var korisnikId = (int)Session["KorisnikId"];

            // Provjeri da li je već u favoritima
            var postojiFavorit = db.Favoriti
                .Any(f => f.KorisnikId == korisnikId && f.ReceptId == receptId);

            if (!postojiFavorit)
            {
                var favorit = new Favoriti
                {
                    KorisnikId = korisnikId,
                    ReceptId = receptId,
                    DatumDodavanja = DateTime.Now
                };

                db.Favoriti.Add(favorit);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // @desc - Uklanjanje recepta iz favorita
        // @route - POST: /Recept/UkloniIzFavorita
        // @access - Private
        [HttpPost]
        public ActionResult UkloniIzFavorita(int receptId)
        {
            if (Session["KorisnikId"] == null)
            {
                return RedirectToAction("Login", "Korisnik");
            }

            var korisnikId = (int)Session["KorisnikId"];

            var favorit = db.Favoriti
                .FirstOrDefault(f => f.KorisnikId == korisnikId && f.ReceptId == receptId);

            if (favorit != null)
            {
                db.Favoriti.Remove(favorit);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        

    }
}