using System;
using System.Collections.Generic;

namespace NutriGoal.Models
{
    public class ReceptDetaljiViewModel
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string Fotografija { get; set; }
        public string KategorijaIme { get; set; }
        public int VrijemePripreme { get; set; }
        public bool SadrziAlergen { get; set; }
        public bool UFavoritima { get; set; }

        public List<string> Ciljevi { get; set; }
        public List<SastojakStavka> Sastojci { get; set; }
        public List<string> KoraciPripreme { get; set; }
        public List<KomentarStavka> Komentari { get; set; }

        public decimal Kalorije { get; set; }
        public decimal Proteini { get; set; }
        public decimal UgljeniHidrati { get; set; }
        public decimal Masti { get; set; }

        public double? ProsjekOcjena { get; set; }
        public int? KorisnikovaOcjena { get; set; }
    }

    public class SastojakStavka
    {
        public string Naziv { get; set; }
        public decimal KolicinaG { get; set; }
    }

    public class KomentarStavka
    {
        public int Id { get; set; }
        public string KorisnikIme { get; set; }
        public string Tekst { get; set; }
        public DateTime Datum { get; set; }
    }
}
