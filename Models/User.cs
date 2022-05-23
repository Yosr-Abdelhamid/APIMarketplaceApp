namespace APIMarketplaceApp.Models
{
    public class User
    {
         public User(string id,string nom , string prenom , string email, string adresse , string num) {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Adresse = adresse;
            Num_Telephone =num;
        }
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Num_Telephone { get; set; }


        
    }
}