using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIMarketplaceApp.Models;
using APIMarketplaceApp.Controllers;
using APIMarketplaceApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using APIMarketplaceApp.Helpers;
using Microsoft.Extensions.Options;
using AutoMapper;
using BC = BCrypt.Net.BCrypt;
using System.Security.Cryptography;

namespace APIMarketplaceApp.Services
{
    public class UserService
    {
        private readonly IMongoCollection<Vendeur> vendeurs;
        private readonly IMongoCollection<ProductVend> produits;
        private readonly string key;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
         private readonly IMapper _mapper;

        public UserService(IConfiguration configuration ,IOptions<AppSettings> appSettings,
            IEmailService emailService , IMapper mapper)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("MarketplaceSiteDB");
            vendeurs = database.GetCollection<Vendeur>("Vendeur");
            produits = database.GetCollection<ProductVend>("ProductVend");
            this.key = configuration.GetSection("JwtKey").ToString();
             _appSettings = appSettings.Value;
            _emailService = emailService;
            _mapper = mapper;
        }

        public List<Vendeur> GetVendeurs() => vendeurs.Find(vendeur => true).ToList();

        public Vendeur GetVendeur(string id) => vendeurs.Find<Vendeur>(vendeur => vendeur.Id == id).FirstOrDefault();

        public void Signup(RegisterRequest vendeur)
        { 
            var account = _mapper.Map<Vendeur>(vendeur);
            account.VerificationToken = randomTokenString();
            account.MotDePasse = BCrypt.Net.BCrypt.HashPassword(vendeur.MotDePasse);
            this.vendeurs.InsertOne(account) ;
        }

        public void AddProduct(RequestProduct produit , IFormFile image_prod,string id)

        {   var id_user = vendeurs.Find<Vendeur>(x => x.Id == id).FirstOrDefault();
            var Produit = _mapper.Map<ProductVend>(produit);
            MemoryStream memoryStream = new MemoryStream();
            image_prod.OpenReadStream().CopyTo(memoryStream) ;
            Produit.image_prod = Convert.ToBase64String(memoryStream.ToArray());
            Produit.Id = id_user.Id ;
            this.produits.InsertOne(Produit) ;

        } 

        public List<ProductVend> GetProductById(string id) => produits.Find<ProductVend>(produit => produit.Id == id).ToList();

          private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        public  AuthenticateResponse Login(AuthenticateRequest model) {
            var vendeura = this.vendeurs.Find(x => x.Email == model.Email).FirstOrDefault();
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(model.MotDePasse , vendeura.MotDePasse);
            if (isValidPassword) {
                var token = generateJwtToken(vendeura);
                return new AuthenticateResponse(vendeura, token);
            }
            return null ;
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

        private string generateJwtToken(Vendeur vendeur)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", vendeur.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
           var vendeur = this.vendeurs.Find(x => x.Email == model.Email).SingleOrDefault();

            // always return ok response to prevent email enumeration
            if (vendeur == null) 
                return;
            vendeur.ResetToken = randomTokenString();
            vendeurs.ReplaceOneAsync(x =>  x.Email== model.Email, vendeur);
            sendPasswordResetEmail(vendeur, origin);
        }

        public void ResetPassword(ResetPasswordRequest model)
        {

            var account = this.vendeurs.Find(x => x.ResetToken == model.Token ).SingleOrDefault();

            // update password and remove reset token
            account.MotDePasse = BCrypt.Net.BCrypt.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            vendeurs.ReplaceOneAsync(x =>  x.MotDePasse== model.Password, account);
        }


           private void sendPasswordResetEmail(Vendeur vendeur, string origin)
        {
            string message1,message;
           
                var resetUrl = $"{origin}/reset-password?token={vendeur.ResetToken}";
                message1 = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{vendeur.ResetToken}</code></p>";
            

            _emailService.Send(
                to: vendeur.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message1}"
            );
        }

    }
}
