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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APIMarketplaceApp.Services
{
    public class UserService
    {
        private readonly IMongoCollection<Vendeur> vendeurs;
        private readonly IMongoCollection<ProductVend> produits;
         private readonly IMongoCollection<Client> clients;
        private readonly IMongoCollection<Admin> admins;

        private readonly IMongoCollection<Contact> contacts;
        private readonly IMongoCollection<Commande> commandes;
        private readonly IMongoCollection<Notif> notifications ;

        private readonly IMongoCollection<PortfeuilleVendeur> portfeuilles ;

        private readonly string key;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        byte[] hash;

        public UserService(IConfiguration configuration ,IOptions<AppSettings> appSettings,
            IEmailService emailService , IMapper mapper)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("MarketplaceSiteDB");
            vendeurs = database.GetCollection<Vendeur>("Vendeur");
            produits = database.GetCollection<ProductVend>("ProductVend");
            admins= database.GetCollection<Admin>("Admin");
            clients= database.GetCollection<Client>("Client");
            contacts=database.GetCollection<Contact>("Contact") ;
            commandes=database.GetCollection<Commande>("Commande") ;
            notifications =database.GetCollection<Notif>("Notifications");
            portfeuilles =database.GetCollection<PortfeuilleVendeur>("PortfeuilleVendeur");

            this.key = configuration.GetSection("JwtKey").ToString();
             _appSettings = appSettings.Value;
            _emailService = emailService;
            _mapper = mapper;
        }

        public List<Vendeur> GetVendeurs() => vendeurs.Find(vendeur => true).ToList();

        public Vendeur GetVendeur(string id) => vendeurs.Find<Vendeur>(vendeur => (vendeur.Id).ToString() == id).FirstOrDefault();

        public string  Signup(RegisterRequest vendeur, string origin)
        { 
            var test = vendeurs.Find<Vendeur>(x => x.Email == vendeur.Email).FirstOrDefault() ;
            if ( test == null) {
                var account = _mapper.Map<Vendeur>(vendeur);
                account.VerificationToken = randomTokenString();
                account.MotDePasse = BCrypt.Net.BCrypt.HashPassword(vendeur.MotDePasse);
                account.isVerified = false ;
                account.isActived = false ;
                account.CartNumber = CreateMD5Hash(vendeur.CartNumber);
                account.CartName = vendeur.CartName ;
                account.expireDate = vendeur.expireDate;
                this.vendeurs.InsertOne(account) ;
                sendVerificationEmail(account,origin);
                return  "Success Registration"; 
                //sendAlreadyRegisteredEmail(vendeur.Email,origin);
            }
            return null;
        }

          public string ActivateVendeur(string id)
        {
      
            var filter = Builders<Vendeur>.Filter.Eq("Id", id);
            var update = Builders<Vendeur>.Update.Set("isActived", "true") ;

            //vendeurs.ReplaceOne(x =>  x.isVerified == true, account);
            //var result = this.vendeurs.UpdateOne(account, update);
            this.vendeurs.UpdateOne(filter, update);
            return ("Vendeur Actived");
        }


         public string  RegisterClient(ClientRequest client, string origin)
        { 
            var test = clients.Find<Client>(x => x.Email == client.Email).FirstOrDefault() ;
            if ( test == null) {
                var account = _mapper.Map<Client>(client);
                account.VerificationToken = randomTokenString();
                account.Role="isClient" ;
                account.MotDePasse = BCrypt.Net.BCrypt.HashPassword(client.MotDePasse);
                account.isVerified = false ;
                account.isActived = true ;
                this.clients.InsertOne(account) ;
                sendVerificationEmails(account,origin);
                return  "Success Registration"; 
                //sendAlreadyRegisteredEmail(vendeur.Email,origin);
            }
            return null;
        }

        public string BlockClient(string id)
        {
      
            var filter = Builders<Client>.Filter.Eq("Id", id);
            var update = Builders<Client>.Update.Set("isActived", "false") ;

            //vendeurs.ReplaceOne(x =>  x.isVerified == true, account);
            //var result = this.vendeurs.UpdateOne(account, update);
            this.clients.UpdateOne(filter, update);
            return ("Vendeur Actived");
        }



        public string  RegisterAdmin(UserAdminRequest admin, string origin)
        { 
            var test = admins.Find<Admin>(x => x.Email == admin.Email).FirstOrDefault() ;
            if ( test == null) {
                var account = _mapper.Map<Admin>(admin);
                account.VerificationToken = randomTokenString();
                account.Role = "isAdmin" ;
                account.MotDePasse = BCrypt.Net.BCrypt.HashPassword(admin.MotDePasse); 
                this.admins.InsertOne(account) ;
                //sendVerificationEmails(account,origin);
                return  "Success Registration"; 
                //sendAlreadyRegisteredEmail(vendeur.Email,origin);
            }
            return null;
        }

        public void AddProduct(RequestProduct produit , IFormFile image_prod,string id)

        {   var id_user = vendeurs.Find<Vendeur>(x => x.Id== id).FirstOrDefault();
            var Produit = _mapper.Map<ProductVend>(produit);
            MemoryStream memoryStream = new MemoryStream();
            image_prod.OpenReadStream().CopyTo(memoryStream) ;
            Produit.image_prod = Convert.ToBase64String(memoryStream.ToArray());
            Produit.Id = (id_user.Id).ToString() ;
            this.produits.InsertOne(Produit) ;

        } 

          public void AddOrder(RequestOrder order )

        {   
            var Order = _mapper.Map<Commande>(order);
            this.commandes.InsertOne(Order) ;

        } 

         public void AddPortfeuille(RequestPortfeuille solde )

        {   
            var Sold = _mapper.Map<PortfeuilleVendeur>(solde);
            this.portfeuilles.InsertOne(Sold) ;

        } 
        
        public async Task RemoveAsync(string id) =>
            await produits.DeleteOneAsync(x => x.Id_prod == id);
        
        public async Task RemoveAsyn(string id) =>
            await contacts.DeleteOneAsync(x => x.Id == id);

        public async Task RemoveAsy(string id) =>
            await notifications.DeleteOneAsync(x => x.Id == id);

        public List<ProductVend> GetProductById(string id) => produits.Find<ProductVend>(produit => produit.Id == id).ToList();

        public List<ProductVend> GetProductByCategory(string sous_famille_prod) => produits.Find<ProductVend>(produit => produit.sous_famille_prod == sous_famille_prod).ToList();

        

          private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        public  AuthenticateResponse Login(AuthenticateRequest model) {
            var vendeura = this.vendeurs.Find(x => x.Email == model.Email && x.isVerified && x.isActived).FirstOrDefault();
            if (vendeura != null){
                bool isValidPassword = BCrypt.Net.BCrypt.Verify(model.MotDePasse , vendeura.MotDePasse);
                if (isValidPassword) {
                    var token = generateJwtToken(vendeura);
                    return new AuthenticateResponse(vendeura, token);
            }
            }
            return null ;
        }

        public  AdminUserResponse LoginUser(AuthenticateRequest model) {
            var client = this.clients.Find(x => x.Email == model.Email && x.isVerified && x.isActived).FirstOrDefault();
            if (client != null) {
                bool isValidPassword = BCrypt.Net.BCrypt.Verify(model.MotDePasse , client.MotDePasse);
                if (isValidPassword) {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenKey = Encoding.ASCII.GetBytes(key);
                    var tokenDescriptor = new SecurityTokenDescriptor
                        {
                        Subject = new ClaimsIdentity(new[] { new Claim("id", client.Id.ToString()) }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                        };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return new AdminUserResponse(client, tokenHandler.WriteToken(token));}
           
            }else {
                var admin = this.admins.Find(x => x.Email == model.Email).FirstOrDefault();
                bool isValidPassword = BCrypt.Net.BCrypt.Verify(model.MotDePasse , admin.MotDePasse);
                if (isValidPassword) { 
                var token = generateJwtTokens(admin);
                return new AdminUserResponse(admin, token);}
                
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

        private string generateJwtTokens(Admin admin)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", admin.Id.ToString()) }),
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

         public void ForgotPasswordUser(ForgotPasswordRequest model, string origin)
        {
           var client = this.clients.Find(x => x.Email == model.Email).SingleOrDefault();

            // always return ok response to prevent email enumeration
            if (client == null) 
                return;
            client.ResetToken = randomTokenString();
            clients.ReplaceOneAsync(x =>  x.Email== model.Email, client);
            sendPasswordResetUserEmail(client, origin);
        }

        public void ResetPassword(ResetPasswordRequest model)
        {

            var account = Builders<Vendeur>.Filter.Eq("ResetToken" ,model.Token);

            // update password and remove reset token
            //account.MotDePasse = BCrypt.Net.BCrypt.HashPassword(model.Password);
            //account.PasswordReset = DateTime.UtcNow;
            var update = Builders<Vendeur>.Update.Set("MotDePasse", BCrypt.Net.BCrypt.HashPassword(model.Password))
                                                .Set("PasswordReset" ,DateTime.UtcNow) ;
            this.vendeurs.UpdateOne(account, update);
            //vendeurs.ReplaceOneAsync(x =>  x.MotDePasse== model.Password, account);
        }
        public void ResetPasswordUser(ResetPasswordRequest model)
        {

            var account = Builders<Client>.Filter.Eq("ResetToken" ,model.Token);

            // update password and remove reset token
            //account.MotDePasse = BCrypt.Net.BCrypt.HashPassword(model.Password);
            //account.PasswordReset = DateTime.UtcNow;
            var update = Builders<Client>.Update.Set("MotDePasse", BCrypt.Net.BCrypt.HashPassword(model.Password))
                                                .Set("PasswordReset" ,DateTime.UtcNow) ;
            this.clients.UpdateOne(account, update);
            //vendeurs.ReplaceOneAsync(x =>  x.MotDePasse== model.Password, account);
        }
        
           public string VerifyEmail(string token)
        {
            //var account = vendeurs.Find(x => x.VerificationToken == token).SingleOrDefault();
            var filter = Builders<Vendeur>.Filter.Eq("VerificationToken", token);

            if (filter == null) return ("Verification failed");

            //account.isVerified = true;
            var update = Builders<Vendeur>.Update.Set("isVerified", "true") ;

            //vendeurs.ReplaceOne(x =>  x.isVerified == true, account);
            //var result = this.vendeurs.UpdateOne(account, update);
            this.vendeurs.UpdateOne(filter, update);
            return ("Email Verified");
        }

           public string VerifyEmailClient(string token)
        {
            //var account = vendeurs.Find(x => x.VerificationToken == token).SingleOrDefault();
            var filter = Builders<Client>.Filter.Eq("VerificationToken", token);

            if (filter == null) return ("Verification failed");

            //account.isVerified = true;
            var update = Builders<Client>.Update.Set("isVerified", "true") ;

            //vendeurs.ReplaceOne(x =>  x.isVerified == true, account);
            //var result = this.vendeurs.UpdateOne(account, update);
            this.clients.UpdateOne(filter, update);
            return ("Email Verified");
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
                subject: "Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message1}"
            );
        }

         private void sendPasswordResetUserEmail(Client client, string origin)
        {
            string message1,message;
           
                var resetUrl = $"{origin}/reset-password-user?token={client.ResetToken}";
                message1 = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{client.ResetToken}</code></p>";
            

            _emailService.Send(
                to: client.Email,
                subject: "Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message1}"
            );
        }

        private void sendVerificationEmail(Vendeur vendeur, string origin)
        {
            string message;
            
            
            var verifyUrl = $"{origin}/verify-email?token={vendeur.VerificationToken}";
            message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            

            _emailService.Send(
                to: vendeur.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }
        private void sendVerificationEmails(Client client, string origin)
        {
            string message;
            
            
            var verifyUrl = $"{origin}/verify-email-client?token={client.VerificationToken}";
            message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            

            _emailService.Send(
                to: client.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }

        private void sendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))

                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/forgot-password</code> api route.</p>";

            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private void sendEmailReply(string email , string sujet , string message) {
            _emailService.Send(
                to : email ,
                subject : sujet ,
                html : message) ;
        }


        public void sendNotifSeller(string email) {
            _emailService.Send(
                to : email ,
                 subject: "Warning! Selling fees not paid",
                html: $@"You have an unpaid selling fee. Please top up your wallet to continue the payment process.") ;
        }

         public string DelivredOrder(string id ) 
        {
      
            var filter = Builders<Commande>.Filter.Eq("Id", id);
            var update = Builders<Commande>.Update.Set("delivred", "true") ;

            //vendeurs.ReplaceOne(x =>  x.isVerified == true, account);
            //var result = this.vendeurs.UpdateOne(account, update);
            this.commandes.UpdateOne(filter, update);
            return ("Order delivred");
        }

        public string CreateMD5Hash(string input)
            {
                // Step 1, calculate MD5 hash from input
                MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                
                // Step 2, convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }

        
    }
}

