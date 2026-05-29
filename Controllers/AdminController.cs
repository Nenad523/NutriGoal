using System;
using System.Linq;
using System.Web.Mvc;
using NutriGoal.Models;

namespace NutriGoal.Controllers
{
    public class AdminController : Controller
    {
        private NutriGoalEntities db = new NutriGoalEntities();

        private bool IsAdmin()
        {
            return Session["KorisnikUloga"] != null
                && Session["KorisnikUloga"].ToString() == "Admin";
        }

        // @desc - Povlacenje pocetne stranice za admina
        // @route - GET: /Admin/Index
        // @access - Private
        public ActionResult Index(string naziv = null)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Korisnik");

            ViewBag.FilterNaziv = naziv;

            var query = db.Recepti.AsQueryable();

            if (!string.IsNullOrWhiteSpace(naziv))
                query = query.Where(r => r.Naziv.Contains(naziv));

            var recepti = query
                .OrderBy(r => r.Naziv)
                .Select(r => new ReceptViewModel
                {
                    Id = r.Id,
                    Naziv = r.Naziv,
                    KategorijaIme = r.Kategorije.Naziv,
                    VrijemePripreme = r.VrijemePripreme,
                    Kalorije = r.Kalorije
                }).ToList();

            return View(recepti);
        }

        // @desc - Forma za kreiranje novog recepta
        // @route - GET: /Admin/Kreiraj
        // @access - Private
        public ActionResult Kreiraj()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Korisnik");

            PopuniViewBag();
            return View("Forma", new ReceptFormViewModel());
        }

        // @desc - Cuvanje novog recepta u bazu
        // @route - POST: /Admin/Kreiraj
        // @access - Private
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Kreiraj(ReceptFormViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Korisnik");

            if (!ModelState.IsValid)
            {
                PopuniViewBag();
                return View("Forma", model);
            }

            var recept = new Recepti
            {
                Naziv = model.Naziv,
                Opis = model.Opis,
                Postupak = model.Postupak,
                Fotografija = model.Fotografija,
                KategorijaId = model.KategorijaId,
                VrijemePripreme = model.VrijemePripreme,
                Kalorije = model.Kalorije,
                Proteini = model.Proteini,
                UgljeniHidrati = model.UgljeniHidrati,
                Masti = model.Masti,
                DatumKreiranja = DateTime.Now
            };

            db.Recepti.Add(recept);
            db.SaveChanges();

            if (model.Sastojci != null)
            {
                foreach (var s in model.Sastojci.Where(s => s.SastojakId > 0 && s.KolicinaG > 0))
                {
                    db.ReceptSastojci.Add(new ReceptSastojci
                    {
                        ReceptId = recept.Id,
                        SastojakId = s.SastojakId,
                        KolicinaG = s.KolicinaG
                    });
                }
            }

            if (model.OdabraniCiljeviIds != null && model.OdabraniCiljeviIds.Any())
            {
                var ciljevi = db.Ciljevi.Where(c => model.OdabraniCiljeviIds.Contains(c.Id)).ToList();
                foreach (var c in ciljevi)
                    recept.Ciljevi.Add(c);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        private void PopuniViewBag()
        {
            ViewBag.Kategorije = db.Kategorije.OrderBy(k => k.Naziv).ToList();
            ViewBag.SviCiljevi = db.Ciljevi.OrderBy(c => c.Naziv).ToList();
            ViewBag.SviSastojci = db.Sastojci.OrderBy(s => s.Naziv).ToList();
        }
    }
}
