using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NutriGoal.Models;

namespace NutriGoal.Controllers
{
    public class FavoritiController : Controller
    {
        private NutriGoalEntities db = new NutriGoalEntities();

        // @desc - Prikazivanje liste omiljenih recepata
        // @route - GET: /Favoriti/Index
        // @access - Private
        public ActionResult Index()
        {
            if (Session["KorisnikId"] == null)
                return RedirectToAction("Login", "Korisnik");

            var korisnikId = (int)Session["KorisnikId"];
            var korisnik = db.Korisnici.Find(korisnikId);
            var korisnikAlergeni = korisnik.Alergije.Select(a => a.Id).ToList();

            var favoriti = db.Favoriti
                .Where(f => f.KorisnikId == korisnikId)
                .Select(f => new ReceptViewModel
                {
                    Id = f.Recepti.Id,
                    Naziv = f.Recepti.Naziv,
                    Opis = f.Recepti.Opis,
                    Fotografija = f.Recepti.Fotografija,
                    KategorijaId = f.Recepti.KategorijaId,
                    KategorijaIme = f.Recepti.Kategorije.Naziv,
                    VrijemePripreme = f.Recepti.VrijemePripreme,
                    Kalorije = f.Recepti.Kalorije,
                    Proteini = f.Recepti.Proteini,
                    UgljeniHidrati = f.Recepti.UgljeniHidrati,
                    Masti = f.Recepti.Masti,
                    SadrziAlergen = f.Recepti.ReceptSastojci
                        .Any(rs => rs.Sastojci.Alergije
                            .Any(a => korisnikAlergeni.Contains(a.Id))),
                    UFavoritima = true
                })
                .OrderBy(r => r.Naziv)
                .ToList();

            return View(favoriti);
        }
    }
}
