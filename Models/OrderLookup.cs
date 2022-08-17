namespace APIMarketplaceApp.Models
{
    public class OrderLookup
    {
    
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Organization { get; set; } 
        public virtual List<ProduitOrder> produits { get; set; }
    }
}