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
        public ActionResult Index(bool sviRecepti = false, string naziv = null, int? kategorijaId = null, int[] ciljeviIds = null, int? minKalorije = null, int? maxKalorije = null, int? maxVrijeme = null, string sortiranje = "Naziv")
        {
            ViewBag.Kategorije = db.Kategorije.ToList();
            ViewBag.Ciljevi = db.Ciljevi.ToList();
            ViewBag.SviRecepti = sviRecepti;

            bool filtersActive = !string.IsNullOrWhiteSpace(naziv)
                || kategorijaId.HasValue
                || (ciljeviIds != null && ciljeviIds.Length > 0)
                || (minKalorije.HasValue && minKalorije.Value > 0)
                || (maxKalorije.HasValue && maxKalorije.Value < 1500)
                || maxVrijeme.HasValue;

            List<int> korisnikAlergeni = new List<int>();
            List<int> favoritiIds = new List<int>();

            if (Session["KorisnikId"] != null)
            {
                var korisnikId = (int)Session["KorisnikId"];
                var korisnik = db.Korisnici.Find(korisnikId);

                korisnikAlergeni = korisnik.Alergije.Select(a => a.Id).ToList();

                favoritiIds = db.Favoriti
                    .Where(f => f.KorisnikId == korisnikId)
                    .Select(f => f.ReceptId)
                    .ToList();

                var profil = db.KorisnickiProfil.FirstOrDefault(kp => kp.KorisnikId == korisnikId);
                var imaCilj = profil != null && profil.CiljId != null;
                ViewBag.ImaKorisnikCilj = imaCilj;

                if (imaCilj && !sviRecepti && !filtersActive)
                {
                    var kategorijeMapa = db.Kategorije.ToDictionary(k => k.Id, k => k.Naziv);

                    var preporuceni = db.sp_PreporuciRecepte(korisnikId)
                        .Select(r => new ReceptViewModel
                        {
                            Id = r.Id,
                            Naziv = r.Naziv,
                            Opis = r.Opis,
                            Fotografija = r.Fotografija,
                            KategorijaId = r.KategorijaId,
                            KategorijaIme = kategorijeMapa.ContainsKey(r.KategorijaId) ? kategorijeMapa[r.KategorijaId] : "",
                            VrijemePripreme = r.VrijemePripreme,
                            Kalorije = r.Kalorije,
                            Proteini = r.Proteini,
                            UgljeniHidrati = r.UgljeniHidrati,
                            Masti = r.Masti,
                            SadrziAlergen = false,
                            UFavoritima = favoritiIds.Contains(r.Id)
                        }).ToList();

                    ViewBag.Personalizovano = true;
                    return View(preporuceni);
                }
            }
            else
            {
                ViewBag.ImaKorisnikCilj = false;
            }

            // LINQ filtering
            var query = db.Recepti.AsQueryable();

            if (!string.IsNullOrWhiteSpace(naziv))
                query = query.Where(r => r.Naziv.Contains(naziv));

            if (kategorijaId.HasValue)
                query = query.Where(r => r.KategorijaId == kategorijaId.Value);

            if (ciljeviIds != null && ciljeviIds.Length > 0)
                query = query.Where(r => r.Ciljevi.Any(c => ciljeviIds.Contains(c.Id)));

            if (minKalorije.HasValue && minKalorije.Value > 0)
                query = query.Where(r => r.Kalorije >= minKalorije.Value);

            if (maxKalorije.HasValue && maxKalorije.Value < 1500)
                query = query.Where(r => r.Kalorije <= maxKalorije.Value);

            if (maxVrijeme.HasValue)
                query = query.Where(r => r.VrijemePripreme <= maxVrijeme.Value);

            switch (sortiranje)
            {
                case "Kalorije":
                    query = query.OrderBy(r => r.Kalorije);
                    break;
                case "Vrijeme":
                    query = query.OrderBy(r => r.VrijemePripreme);
                    break;
                default:
                    query = query.OrderBy(r => r.Naziv);
                    break;
            }

            var recepti = query.Select(r => new ReceptViewModel
            {
                Id = r.Id,
                Naziv = r.Naziv,
                Opis = r.Opis,
                Fotografija = r.Fotografija,
                KategorijaId = r.KategorijaId,
                KategorijaIme = r.Kategorije.Naziv,
                VrijemePripreme = r.VrijemePripreme,
                Kalorije = r.Kalorije,
                Proteini = r.Proteini,
                UgljeniHidrati = r.UgljeniHidrati,
                Masti = r.Masti,
                SadrziAlergen = r.ReceptSastojci
                    .Any(rs => rs.Sastojci.Alergije
                        .Any(a => korisnikAlergeni.Contains(a.Id))),
                UFavoritima = favoritiIds.Contains(r.Id)
            }).ToList();

            ViewBag.Personalizovano = false;
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