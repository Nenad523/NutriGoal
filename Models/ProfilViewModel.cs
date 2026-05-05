using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NutriGoal.Models
{
    public class ProfilViewModel
    {
        [StringLength(100, ErrorMessage = "Ime ne smije biti duže od 100 karaktera.")]    
        public string Ime { get; set; }

        [StringLength(100, ErrorMessage = "Prezime ne smije biti duže od 100 karaktera.")]
        public string Prezime { get; set; }

        public DateTime? DatumRodjenja { get; set; }

        [RegularExpression("^(M|Z)$", ErrorMessage = "Pol mora biti 'M' (muški) ili 'Z' (ženski).")]
        public string Pol { get; set; }

        [Range(50, 300, ErrorMessage = "Visina mora biti između 50 i 300 cm.")]
        public decimal? Visina { get; set; }

        [Range(20, 500, ErrorMessage = "Težina mora biti između 20 i 500 kg.")]
        public decimal? Tezina { get; set; }

        [Range(1, 4, ErrorMessage = "Nivo aktivnosti mora biti između 1 i 4.")]
        public int? NivoAktivnosti { get; set; }

        public int? CiljId { get; set; }

    }
}