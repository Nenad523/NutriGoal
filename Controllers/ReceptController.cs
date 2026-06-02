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
            ViewBag.FilterNaziv = naziv;
            ViewBag.FilterKategorijaId = kategorijaId;
            ViewBag.FilterCiljeviIds = ciljeviIds ?? new int[0];
            ViewBag.FilterMinKalorije = minKalorije ?? 0;
            ViewBag.FilterMaxKalorije = maxKalorije ?? 1500;
            ViewBag.FilterMaxVrijeme = maxVrijeme;
            ViewBag.FilterSortiranje = sortiranje ?? "Naziv";

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

        // @desc - Prikazivanje detalja recepta
        // @route - GET: /Recept/Details/5
        // @access - Public AND Private
        public ActionResult Details(int id)
        {
            var recept = db.Recepti.Find(id);
            if (recept == null)
                return HttpNotFound();

            // Sastojci
            var sastojci = db.ReceptSastojci
                .Where(rs => rs.ReceptId == id)
                .Select(rs => new SastojakStavka
                {
                    Naziv = rs.Sastojci.Naziv,
                    KolicinaG = rs.KolicinaG
                }).ToList();

            // Ciljevi
            var ciljevi = recept.Ciljevi
                .Select(c => c.Naziv)
                .ToList();

            // Koraci pripreme — split po novom redu
            var koraci = string.IsNullOrWhiteSpace(recept.Postupak)
                ? new List<string>()
                : recept.Postupak
                    .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(k => k.Trim())
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToList();

            // Nutritivne vrijednosti iz funkcije
            var nutritivne = db.fn_NutritivneVrijednostiRecepta(id).FirstOrDefault();

            // Komentari
            var komentari = db.Komentari
                .Where(k => k.ReceptId == id)
                .OrderByDescending(k => k.DatumKreiranja)
                .Select(k => new KomentarStavka
                {
                    Id = k.Id,
                    KorisnikIme = k.Korisnici.KorisnickiProfil
                        .Select(kp => kp.Ime != null && kp.Ime != "" ? kp.Ime + " " + kp.Prezime : null)
                        .FirstOrDefault() ?? k.Korisnici.Email,
                    Tekst = k.Tekst,
                    Datum = k.DatumKreiranja
                }).ToList();

            // Ocjene
            var ocjene = db.Ocjene.Where(o => o.ReceptId == id).ToList();
            double? prosjek = ocjene.Any() ? ocjene.Average(o => o.Vrijednost) : (double?)null;

            // Favoriti i alergeni za prijavljenog korisnika
            bool uFavoritima = false;
            bool sadrziAlergen = false;
            int? korisnikovaOcjena = null;

            if (Session["KorisnikId"] != null)
            {
                var korisnikId = (int)Session["KorisnikId"];
                uFavoritima = db.Favoriti.Any(f => f.KorisnikId == korisnikId && f.ReceptId == id);
                var korisnikAlergeni = db.Korisnici.Find(korisnikId).Alergije.Select(a => a.Id).ToList();
                sadrziAlergen = sastojci.Any() && db.ReceptSastojci
                    .Where(rs => rs.ReceptId == id)
                    .Any(rs => rs.Sastojci.Alergije.Any(a => korisnikAlergeni.Contains(a.Id)));
                korisnikovaOcjena = db.Ocjene
                    .Where(o => o.ReceptId == id && o.KorisnikId == korisnikId)
                    .Select(o => (int?)o.Vrijednost)
                    .FirstOrDefault();
            }

            var model = new ReceptDetaljiViewModel
            {
                Id = recept.Id,
                Naziv = recept.Naziv,
                Opis = recept.Opis,
                Fotografija = recept.Fotografija,
                KategorijaIme = recept.Kategorije.Naziv,
                VrijemePripreme = recept.VrijemePripreme,
                SadrziAlergen = sadrziAlergen,
                UFavoritima = uFavoritima,
                Ciljevi = ciljevi,
                Sastojci = sastojci,
                KoraciPripreme = koraci,
                Komentari = komentari,
                Kalorije = nutritivne != null ? nutritivne.Kalorije : recept.Kalorije ?? 0,
                Proteini = nutritivne != null ? nutritivne.Proteini : recept.Proteini ?? 0,
                UgljeniHidrati = nutritivne != null ? nutritivne.UgljeniHidrati : recept.UgljeniHidrati ?? 0,
                Masti = nutritivne != null ? nutritivne.Masti : recept.Masti ?? 0,
                ProsjekOcjena = prosjek,
                KorisnikovaOcjena = korisnikovaOcjena
            };

            return View(model);
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

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
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

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            return RedirectToAction("Index");
        }
        // @desc - Dodavanje komentara na recept
        // @route - POST: /Recept/DodajKomentar
        // @access - Private
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DodajKomentar(int receptId, string tekst)
        {
            if (Session["KorisnikId"] == null)
                return RedirectToAction("Login", "Korisnik");

            if (string.IsNullOrWhiteSpace(tekst))
                return RedirectToAction("Details", new { id = receptId });

            var komentar = new Komentari
            {
                ReceptId = receptId,
                KorisnikId = (int)Session["KorisnikId"],
                Tekst = tekst.Trim(),
                DatumKreiranja = DateTime.Now
            };

            db.Komentari.Add(komentar);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = receptId });
        }

        // @desc - Ocjenjivanje recepta
        // @route - POST: /Recept/OcijeniRecept
        // @access - Private
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OcijeniRecept(int receptId, int vrijednost)
        {
            if (Session["KorisnikId"] == null)
                return RedirectToAction("Login", "Korisnik");

            if (vrijednost < 1 || vrijednost > 5)
                return RedirectToAction("Details", new { id = receptId });

            var korisnikId = (int)Session["KorisnikId"];

            var postojecaOcjena = db.Ocjene
                .FirstOrDefault(o => o.ReceptId == receptId && o.KorisnikId == korisnikId);

            if (postojecaOcjena != null)
            {
                postojecaOcjena.Vrijednost = vrijednost;
            }
            else
            {
                db.Ocjene.Add(new Ocjene
                {
                    ReceptId = receptId,
                    KorisnikId = korisnikId,
                    Vrijednost = vrijednost,
                    DatumOcjene = DateTime.Now
                });
            }

            db.SaveChanges();

            return RedirectToAction("Details", new { id = receptId });
        }

    }
}