using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NutriGoal.Models;

namespace NutriGoal.Controllers
{
    public class HomeController : Controller
    {
        private NutriGoalEntities db = new NutriGoalEntities();
        public ActionResult Index()
        {
            var sat = DateTime.Now.Hour;

            string kategorijaObroka;
            string naslovObroka;
            string opisObroka;

            if (sat >= 6 && sat < 12)
            {
                kategorijaObroka = "Doručak";
                naslovObroka = "Dobro jutro!";
                opisObroka = "Počnite dan pravim obrokom.";
            }
            else if (sat >= 12 && sat < 17)
            {
                kategorijaObroka = "Ručak";
                naslovObroka = "Vrijme je za ručak.";
                opisObroka = "Napunite energiju za ostatak dana.";
            }
            else if (sat >= 17 && sat < 21)
            {
                kategorijaObroka = "Večera";
                naslovObroka = "Večernji obrok.";
                opisObroka = "Lagano i hranjivo za kraj dana.";
            }
            else
            {
                kategorijaObroka = "Snack";
                naslovObroka = "Vrijme za užinu.";
                opisObroka = "Lagana užina između obroka.";
            }

            var preporuceniRecepti = db.Recepti
                .Where(r => r.Kategorije.Naziv == kategorijaObroka)
                .Take(3)
                .Select(r => new ReceptViewModel
                {
                    Id = r.Id,
                    Naziv = r.Naziv,
                    Fotografija = r.Fotografija,
                    KategorijaId = r.KategorijaId,
                    VrijemePripreme = r.VrijemePripreme,
                    Kalorije = r.Kalorije
                })
                .ToList();

            ViewBag.PreporuceniRecepti = preporuceniRecepti;
            ViewBag.KategorijaObroka = kategorijaObroka;
            ViewBag.NaslovObroka = naslovObroka;
            ViewBag.OpisObroka = opisObroka;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}