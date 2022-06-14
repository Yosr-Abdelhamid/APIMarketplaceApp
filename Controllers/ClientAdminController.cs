using APIMarketplaceApp.Models;
using APIMarketplaceApp.Services;
using APIMarketplaceApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
 using MongoDB.Driver.Linq;
using System.Net;
using System.Net.Http;
using AutoMapper;
using System.Web.Http.Cors;
using MongoDB.Bson;

namespace APIMarketplaceApp.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ClientAdminController : Controller
    {
        private readonly UserService service;
        private readonly IMongoCollection<Client> clients;
        private readonly IMongoCollection<Admin> admins;
        private readonly IMongoCollection<ProductVend> produits ;
        private readonly IMongoCollection<Vendeur> vendeurs ;
        private readonly IMongoCollection<Contact> contacts;
        private readonly IMapper _mapper;
         private readonly IEmailService _emailService;

    public ClientAdminController(UserService _service,IConfiguration configuration ,IMapper mapper ,IEmailService emailService){
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("MarketplaceSiteDB");
            clients = database.GetCollection<Client>("Client");
            admins = database.GetCollection<Admin>("Admin");
            vendeurs = database.GetCollection<Vendeur>("Vendeur");
            produits = database.GetCollection<ProductVend>("ProductVend");
            contacts=database.GetCollection<Contact>("Contact") ;
            
            _emailService = emailService;
            service = _service;
            _mapper = mapper;
    }

    [HttpPost("RegisterClient")]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        public  async Task<object>  RegisterClient(ClientRequest client)

        {   var response = service.RegisterClient(client ,Request.Headers["origin"]);
            if (response == null) {
                return BadRequest(new { message = "Email in use." });
            } 
            return Ok(new { message = "Please Check your email to finish registration instructions" });   
        }

     [HttpPost("RegisterAdmin")]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        public  async Task<object> RegisterAdmin(UserAdminRequest admin)

        {   var response = service.RegisterAdmin(admin ,Request.Headers["origin"]);
            if (response == null) {
                return BadRequest(new { message = "Email in use." });
            } 
            return Ok(new { message = "Registeration success" });   
        }


        [HttpPost("verify-email")]
        [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        public async Task<object> VerifyEmailClient(VerifyEmailRequest model)

        {
            var resp =service.VerifyEmailClient(model.Token);
            if (resp == null)
                return BadRequest(new { message = "Account not verified." });
            
            return Ok(new { message = "Verification successful, you can now login" });
        }

    [AllowAnonymous]
    [HttpPost("LoginUser")]
        public async Task<object>  LoginUser(AuthenticateRequest model)
        {
           var response =  service.LoginUser(model);
            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect." });
                
            return await Task.FromResult(new ResponseModel(1, "", response));
        }


        [HttpGet("GetAdminProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> GetAdminProfile() {
            string userId = User.Claims.First(c => c.Type == "id").Value;
            var user = admins.Find<Admin>(admin => admin.Id == userId).FirstOrDefault();
            return new
            {       
                user.Id,
                 user.Nom,
                 user.Prenom,
                 user.Email,
                 user.Adresse,
                 user.Num_Telephone,
                 user.ZipCode,
            };
        } 

        [HttpGet("GetClientProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> GetClientProfile() {
            string userId = User.Claims.First(c => c.Type == "id").Value;
            var user = clients.Find<Client>(client => client.Id == userId).FirstOrDefault();
            return new
            {       
                user.Id,
                 user.Nom,
                 user.Prenom,
                 user.Email,
                 user.Adresse,
                 user.Num_Telephone,
                 user.ZipCode,
            };
        }  


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAllClients")]
         public async Task<object> GetAllClients()
        {
            
                List<User> allUserDTO = new List<User>();
                var users =  clients.Find(client => true).ToList();
                foreach (var client in users)
                {
                    
                    allUserDTO.Add(new User(client.Id,client.Nom, client.Prenom, client.Email, client.Adresse ,
                    client.Num_Telephone,client.ZipCode ));
                }
                return await Task.FromResult(new ResponseModel(1, "", allUserDTO));
        }


        [HttpGet("GetListProducts")]
        public async Task<object> GetListProducts()

       {    
            var query = from o in produits.AsQueryable()
             join i in vendeurs.AsQueryable()
             on o.Id equals i.Id
             into ListVendeurs
             select new VendeurLookup
            {
                Reference = o.Reference,
                sous_famille_prod= o.sous_famille_prod,
                Brand = o.Brand ,
                quantity = o.quantity ,
                description_prod = o.description_prod ,
                prix_prod = o.prix_prod,
                image_prod = o.image_prod,
                Vendeurs =  (List<Vendeur>)ListVendeurs
            };
            var productts =  query.ToList();
            return productts ; 
            
        }
            
        [HttpPost("AddContact")]
        public  async Task<object> AddContact(Contact contact)

        {   
            contacts.InsertOne(contact);
            return Ok(new { message = "Contact Sended with success" });   
        }  
        

        [HttpGet("GetContact")]
        public  async Task<object> GetContact()

        {  var test = contacts.Find(contact => true).ToList();  
         
            return test;
            
        } 

        [HttpPost("ReplyToContact")]
        public  void sendEmailReply(ContactModelResponse reponse) 

        {   
            _emailService.Send(
                to : reponse.email ,
                subject : reponse.sujet ,
                html : reponse.message) ;
        } 
        } 

    
}
            
                