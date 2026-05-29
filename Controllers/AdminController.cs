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

        private void PopuniViewBag()
        {
            ViewBag.Kategorije = db.Kategorije.OrderBy(k => k.Naziv).ToList();
            ViewBag.SviCiljevi = db.Ciljevi.OrderBy(c => c.Naziv).ToList();
            ViewBag.SviSastojci = db.Sastojci.OrderBy(s => s.Naziv).ToList();
        }
    }
}
