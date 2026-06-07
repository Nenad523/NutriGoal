using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NutriGoal.Models
{
    public class ReceptFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [StringLength(200)]
        public string Naziv { get; set; }

        public string Opis { get; set; }
        public string Postupak { get; set; }
        public string Fotografija { get; set; }

        [Required(ErrorMessage = "Kategorija je obavezna.")]
        public int KategorijaId { get; set; }

        [Required(ErrorMessage = "Vrijeme pripreme je obavezno.")]
        [Range(1, 600, ErrorMessage = "Vrijeme mora biti između 1 i 600 minuta.")]
        public int VrijemePripreme { get; set; }

        public decimal? Kalorije { get; set; }
        public decimal? Proteini { get; set; }
        public decimal? UgljeniHidrati { get; set; }
        public decimal? Masti { get; set; }

        public List<int> OdabraniCiljeviIds { get; set; } = new List<int>();
        public List<SastojakInput> Sastojci { get; set; } = new List<SastojakInput>();
    }

    public class SastojakInput
    {
        public int SastojakId { get; set; }
        public int KolicinaG { get; set; }
    }
}
