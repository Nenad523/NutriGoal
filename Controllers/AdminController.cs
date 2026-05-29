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
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Korisnik");

            var recepti = db.Recepti
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
    }
}
