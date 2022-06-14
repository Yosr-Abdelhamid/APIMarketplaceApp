namespace APIMarketplaceApp.Models
{
    public class UpdateVendeur
    {
        public string id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Num_Telephone { get; set; }
        public int  ZipCode { get; set; }
        public string Organization { get; set; }
    }
}