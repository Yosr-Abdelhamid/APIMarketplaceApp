namespace APIMarketplaceApp.Models
{
    public class User
    {
         public User(string id,string nom , string prenom , string email, string adresse , string num , int zipcode , string organization , bool IsActived ) {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Adresse = adresse;
            Num_Telephone =num;
            ZipCode = zipcode ;
            Organization =organization ;
            isActived = IsActived;
        }
           public User(string id,string nom , string prenom , string email, string adresse , string num , int zipcode , bool IsActived ) {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Adresse = adresse;
            Num_Telephone =num;
            ZipCode = zipcode ;
            isActived = IsActived;
           
        }
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Num_Telephone { get; set; }
        public int ZipCode { get; set; }
        public string Organization { get; set; }
        public bool isActived {get ;set ;}


        
    }
}