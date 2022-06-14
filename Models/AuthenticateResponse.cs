using APIMarketplaceApp.Models;
using MongoDB.Bson;

namespace APIMarketplaceApp.Models
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Num_Telephone { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(Vendeur vendeur, string token)
        {
            Id = vendeur.Id;
            Nom = vendeur.Nom;
            Prenom = vendeur.Prenom;
            Email = vendeur.Email;
            Adresse= vendeur.Adresse;
            Num_Telephone=vendeur.Num_Telephone;
            Token = token;
        }
    }
}