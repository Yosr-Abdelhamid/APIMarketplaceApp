namespace APIMarketplaceApp.Models
{
    public class Utilisateur
    {
        public Utilisateur(string id,string nom , string prenom , string email, string adresse , string num , int zipcode) {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Adresse = adresse;
            Num_Telephone =num;
            ZipCode = zipcode ;
           
        }
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Num_Telephone { get; set; }
        public int ZipCode { get; set; }
        
    }
}