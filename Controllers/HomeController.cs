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
                naslovObroka = "Vrijeme je za ručak.";
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
                naslovObroka = "Vrijeme za užinu.";
                opisObroka = "Lagana užina između obroka.";
            }

            var kategorija = db.Kategorije.FirstOrDefault(k => k.Naziv == kategorijaObroka);
            var kategorijaId = kategorija != null ? kategorija.Id : 0;

            List<ReceptViewModel> preporuceni;

            if (Session["KorisnikId"] != null)
            {
                var korisnikId = (int)Session["KorisnikId"];
                var profil = db.KorisnickiProfil.FirstOrDefault(kp => kp.KorisnikId == korisnikId);
                ViewBag.PreporucenoKalorija = profil != null ? profil.PreporucenoKalorija : null;

                if (profil != null && profil.CiljId != null)
                {
                    // Prijavljen, ima profil i cilj — sp_PreporuciRecepte filtrirano po kategoriji
                    preporuceni = db.sp_PreporuciRecepte(korisnikId)
                        .Where(r => r.KategorijaId == kategorijaId)
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

                    // Fallback: sp nije vratio recepte za ovu kategoriju
                    if (preporuceni.Count == 0)
                    {
                        var alergeni = db.Korisnici.Find(korisnikId).Alergije.Select(a => a.Id).ToList();
                        preporuceni = db.Recepti
                            .Where(r => r.KategorijaId == kategorijaId &&
                                        !r.ReceptSastojci.Any(rs => rs.Sastojci.Alergije.Any(a => alergeni.Contains(a.Id))))
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
                    }
                }
                else
                {
                    // Prijavljen, nema cilj — kategorija + provjera alergija
                    var alergeni = db.Korisnici.Find(korisnikId).Alergije.Select(a => a.Id).ToList();
                    preporuceni = db.Recepti
                        .Where(r => r.KategorijaId == kategorijaId &&
                                    !r.ReceptSastojci.Any(rs => rs.Sastojci.Alergije.Any(a => alergeni.Contains(a.Id))))
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
                }
            }
            else
            {
                // Nije prijavljen — samo kategorija po dobu dana
                preporuceni = db.Recepti
                    .Where(r => r.KategorijaId == kategorijaId)
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
            }

            ViewBag.PreporuceniRecepti = preporuceni;
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