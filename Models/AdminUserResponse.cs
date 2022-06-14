namespace APIMarketplaceApp.Models
{
    public class AdminUserResponse{
     public string Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Num_Telephone { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }


        public AdminUserResponse(Admin admin, string token)
        {
            Id = admin.Id;
            Nom = admin.Nom;
            Prenom = admin.Prenom;
            Email = admin.Email;
            Adresse= admin.Adresse;
            Num_Telephone=admin.Num_Telephone;
            Role=admin.Role;
            Token = token;
        }

        public AdminUserResponse(Client client, string token)
        {
            Id = client.Id;
            Nom = client.Nom;
            Prenom = client.Prenom;
            Email = client.Email;
            Adresse= client.Adresse;
            Num_Telephone=client.Num_Telephone;
            Role= client.Role;
            Token = token;
        }
    }
}