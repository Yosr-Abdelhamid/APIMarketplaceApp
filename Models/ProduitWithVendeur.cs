using System.Collections.Generic;

namespace APIMarketplaceApp.Models
{
    public class ProduitWithVendeur
    {
        public ProduitWithVendeur(string id, string reference, int quantity, string prix_prod, string image_prod, List<string> products)
        {
            Id = id;
            Reference = reference;
            Quantity = quantity;
            Prix_prod = prix_prod;
            Image_prod = image_prod;
            produits = products;


        }
        public string Id { get; set; }
        public string Reference { get; set; }
        public int Quantity { get; set; }
        public string Prix_prod { get; set; }
        public string Adresse { get; set; }
        public string Image_prod { get; set; }

        public List<string> produits { get; set; }


    }

}