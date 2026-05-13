namespace NutriGoal.Models
{
    public class ReceptViewModel
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string Fotografija { get; set; }
        public int KategorijaId { get; set; }
        public int VrijemePripreme { get; set; }
        public decimal? Kalorije { get; set; }
        public decimal? Proteini { get; set; }
        public decimal? UgljeniHidrati { get; set; }
        public decimal? Masti { get; set; }
        public bool SadrziAlergen { get; set; }
    }
}