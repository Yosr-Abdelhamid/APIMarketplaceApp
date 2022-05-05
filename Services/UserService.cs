using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIMarketplaceApp.Models;
using APIMarketplaceApp.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace APIMarketplaceApp.Services
{
    public class UserService
    {
        private readonly IMongoCollection<Vendeur> vendeurs;
        private readonly string key;

        public UserService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("MarketplaceSiteDB");
            vendeurs = database.GetCollection<Vendeur>("Vendeur");
            this.key = configuration.GetSection("JwtKey").ToString();
        }

        public List<Vendeur> GetVendeurs() => vendeurs.Find(vendeur => true).ToList();

        public Vendeur GetVendeur(string id) => vendeurs.Find<Vendeur>(vendeur => vendeur.Id == id).FirstOrDefault();

        public Vendeur Create(Vendeur vendeur)
        {
            vendeurs.InsertOne(vendeur);
            return vendeur;
        }

        public string Authenticate(string email, string password)
        {
            var vendeur = this.vendeurs.Find(x => x.Email == email && x.MotDePasse == password).FirstOrDefault();
            if (vendeur == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes. Email, email),
               }),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature
                        ),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
