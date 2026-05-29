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

        // @desc - Forma za uredivanje postojeceg recepta
        // @route - GET: /Admin/Uredi/5
        // @access - Private
        public ActionResult Uredi(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Korisnik");

            var recept = db.Recepti.Find(id);
            if (recept == null) return HttpNotFound();

            var model = new ReceptFormViewModel
            {
                Id = recept.Id,
                Naziv = recept.Naziv,
                Opis = recept.Opis,
                Postupak = recept.Postupak,
                Fotografija = recept.Fotografija,
                KategorijaId = recept.KategorijaId,
                VrijemePripreme = recept.VrijemePripreme,
                Kalorije = recept.Kalorije,
                Proteini = recept.Proteini,
                UgljeniHidrati = recept.UgljeniHidrati,
                Masti = recept.Masti,
                OdabraniCiljeviIds = recept.Ciljevi.Select(c => c.Id).ToList(),
                Sastojci = db.ReceptSastojci
                    .Where(rs => rs.ReceptId == id)
                    .Select(rs => new SastojakInput
                    {
                        SastojakId = rs.SastojakId,
                        KolicinaG = rs.KolicinaG
                    }).ToList()
            };

            PopuniViewBag();
            return View("Forma", model);
        }

        // @desc - Cuvanje izmjena postojeceg recepta
        // @route - POST: /Admin/Uredi
        // @access - Private
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Uredi(ReceptFormViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Korisnik");

            if (!ModelState.IsValid)
            {
                PopuniViewBag();
                return View("Forma", model);
            }

            var recept = db.Recepti.Find(model.Id);
            if (recept == null) return HttpNotFound();

            recept.Naziv = model.Naziv;
            recept.Opis = model.Opis;
            recept.Postupak = model.Postupak;
            recept.Fotografija = model.Fotografija;
            recept.KategorijaId = model.KategorijaId;
            recept.VrijemePripreme = model.VrijemePripreme;

            var stariSastojci = db.ReceptSastojci.Where(rs => rs.ReceptId == model.Id).ToList();
            db.ReceptSastojci.RemoveRange(stariSastojci);

            if (model.Sastojci != null)
            {
                foreach (var s in model.Sastojci.Where(s => s.SastojakId > 0 && s.KolicinaG > 0))
                {
                    db.ReceptSastojci.Add(new ReceptSastojci
                    {
                        ReceptId = model.Id,
                        SastojakId = s.SastojakId,
                        KolicinaG = s.KolicinaG
                    });
                }
            }

            recept.Ciljevi.Clear();
            if (model.OdabraniCiljeviIds != null && model.OdabraniCiljeviIds.Any())
            {
                var ciljevi = db.Ciljevi.Where(c => model.OdabraniCiljeviIds.Contains(c.Id)).ToList();
                foreach (var c in ciljevi)
                    recept.Ciljevi.Add(c);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // @desc - Brisanje recepta iz baze
        // @route - POST: /Admin/Obrisi/5
        // @access - Private
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Obrisi(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Korisnik");

            var recept = db.Recepti.Find(id);
            if (recept == null) return HttpNotFound();

            db.ReceptSastojci.RemoveRange(db.ReceptSastojci.Where(rs => rs.ReceptId == id));
            db.Komentari.RemoveRange(db.Komentari.Where(k => k.ReceptId == id));
            db.Ocjene.RemoveRange(db.Ocjene.Where(o => o.ReceptId == id));
            db.Favoriti.RemoveRange(db.Favoriti.Where(f => f.ReceptId == id));
            db.StavkePlana.RemoveRange(db.StavkePlana.Where(s => s.ReceptId == id));
            recept.Ciljevi.Clear();

            db.Recepti.Remove(recept);
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
