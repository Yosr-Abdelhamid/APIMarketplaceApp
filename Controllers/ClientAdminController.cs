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
 using System.Text.Json;
using System.Net;
using System.Net.Http;
using AutoMapper;
using System.Web.Http.Cors;
using MongoDB.Bson;
using Newtonsoft.Json;

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
        private readonly IMongoCollection<Commande> commandes;
         private readonly IMongoCollection<Commission> commissions;
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
            commandes=database.GetCollection<Commande>("Commande") ;
            commissions=database.GetCollection<Commission>("Commission") ;

            
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
                    client.Num_Telephone,client.ZipCode, client.isActived ));
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
         [HttpGet("GetListProductsById")]
        public async Task<object> GetListProductsById(string id_produit)

       {    
            var query = from o in produits.AsQueryable()
             join i in vendeurs.AsQueryable()
             on o.Id equals i.Id
             into ListVendeurs
             select new VendeurLookup
            {
                Id = id_produit, 
                Reference = o.Reference,
                sous_famille_prod= o.sous_famille_prod,
                Brand = o.Brand ,
                quantity = o.quantity ,
                description_prod = o.description_prod ,
                prix_prod = o.prix_prod,
                image_prod = o.image_prod,
                Vendeurs =  (List<Vendeur>)ListVendeurs
            };
            var productts =  query.FirstOrDefault();
            return productts ; 
            
        }
        [HttpGet("GetProductsByCategory")]
        public ActionResult <List<VendeurLookup>> GetProductsByCategory(string sous_famille)

       {    
            var query = from o in produits.AsQueryable().Where(x => x.sous_famille_prod == sous_famille)
             join i in vendeurs.AsQueryable()
             on o.Id equals i.Id 
             into ListVendeurs
             select new VendeurLookup
            {
                sous_famille_prod= sous_famille,
                Id = o.Id_prod,
                Reference = o.Reference,
                Brand = o.Brand ,
                quantity = o.quantity ,
                description_prod = o.description_prod ,
                prix_prod = o.prix_prod,
                image_prod = o.image_prod,
                Vendeurs =  (List<Vendeur>)ListVendeurs
            };
            var productts =  query.ToList();
            return Json(productts) ; 
            
        }
       
            
        [HttpPost("AddContact")]
        public  async Task<object> AddContact(Contact contact)

        {   
            contacts.InsertOne(contact);
            return Ok(new { message = "Contact Sended with success" });   
        }
        

        [HttpGet("GetContact")]
        public  ActionResult <List<Contact>> GetContact()

        {  var test = contacts.Find<Contact>(contact => true).ToList();  
         
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


        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
         var contact = this.contacts.Find(x => x.Id == id).FirstOrDefaultAsync();

        if (contact is null)
        {
            return NotFound();
        }

        await service.RemoveAsyn(id);

        return NoContent();
        }

         [HttpPost("forgot-password-user")]
        public IActionResult ForgotPasswordUser(ForgotPasswordRequest model)
        {
            service.ForgotPasswordUser(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [HttpPost("reset-password-user")]
        public IActionResult ResetPasswordUser(ResetPasswordRequest model)
        {
            service.ResetPasswordUser(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }


        [HttpPost("AddOrder")]
        public async Task <ActionResult<IEnumerable<Commande>>> AddOrder(CommandeRequest commande)

        {    var order = _mapper.Map<Commande>(commande);
             order.delivred = "Not mention";
             order.state = "Pending";
             commandes.InsertOne(order);
            return Ok(new { message = "Order passed with success" });   
        } 


        [HttpPut("UpdateOrder")]
        public async Task<object> UpdateOrder([FromForm] DelivredOrder model)

        {
           var filter = Builders<Commande>.Filter.Eq("Id", model.id);
            var update = Builders<Commande>.Update.Set("state", "Scheduled")
                                                    .Set("dateCommande" , model.dateCommande);

            this.commandes.UpdateOne(filter, update);
            return ("Order Updated");
        }

        [HttpPut("DelivredOrder")]
         public async Task<object> DelivredOrder ([FromForm] ChangeOrder model)

        {
          var filter = Builders<Commande>.Filter.Eq("Id", model.id);
            var update = Builders<Commande>.Update.Set("delivred", "Delivered")
                                                    .Set("dateCommande" , model.dateCommande);
            this.commandes.UpdateOne(filter, update);
            return ("Order delivred");

        }
        
        [HttpPut("RejectedOrder")]
         public async Task<object> RejectedOrder ([FromForm] ChangeOrder model)

        {
          var filter = Builders<Commande>.Filter.Eq("Id", model.id);
            var update = Builders<Commande>.Update.Set("delivred", "Rejected")
                                                    .Set("dateCommande" , model.dateCommande);
            this.commandes.UpdateOne(filter, update);
            return ("Order Rejected");

        }

        [HttpPost("ActivateVendeur")]
        public async Task<object> ActivateVendeur(ActiveVendeur model)

        {
            var resp =service.ActivateVendeur(model.id);
            if (resp == null)
                return BadRequest(new { message = "Account not activate." });
            
            return Ok(new { message = "Account Activate" });
        }

        [HttpPost("ActivateClient")]
        public async Task<object> ActivateClient(ActivateClient model)

        {
            var resp =service.BlockClient(model.id);
            if (resp == null)
                return BadRequest(new { message = "Account not activate." });
            
            return Ok(new { message = "Account Activate" });
        }



        [HttpGet("GetOrder")]
        public  async Task<object> GetOrder()

        {  var result = commandes.Find(commande => true).ToList();  
         
            return result;
            
        } 
         [HttpGet("GetOrderByEmail")]
        public  async Task<object> GetOrderByEmail(string email)

        {  var result = commandes.Find(x => x.email == email).ToList();  
         
            return result;
            
        } 

        [HttpGet("GetOrderByStore")]
        public  async Task<object> GetOrderByStore(string organization)

        {    
            /* var findFluent = commandes.Find(Builders<Commande>.Filter.ElemMatch(
                x => x.produits, 
                s => s.organization == organization)); */

            var query = from doc in commandes.AsQueryable()
            where doc.produits.Any(x => x.organization == organization)
            select new Commande()
            {
                Id = doc.Id,
                name = doc.name ,
                lastName = doc.lastName,
                country = doc.country ,
                street = doc.street ,
                city = doc.city,
                zip = doc.zip,
                phone = doc.phone,
                email= doc.email,
                produits = (List<ProduitOrder>)doc.produits.Where(x => x.organization == organization) ,
                total = doc.total ,
                dateCommande = doc.dateCommande,
                delivred = doc.delivred,
                state = doc.state,
                payment = doc.payment };

            var result = query.ToList();
            
            return result ;
        } 

        [HttpGet("GetCommission")]
        public  async Task<object> GetCommission()

        {  var result = commissions.Find(commission => true).ToList();  
         
            return result;
            
        } 
        
        [HttpPut("UpdateCommission")]
        public JsonResult Put([FromForm] CommissionUpdate Commision)
        {    
            var filter = Builders<Commission>.Filter.Eq("Id", Commision.Id);
            var update = Builders<Commission>.Update.Set("commission", Commision.commission);
            this.commissions.UpdateOne(filter, update);

            return new JsonResult("Updated Successfully");
        }

        [HttpGet("GetCommisionByCategorie")]

        public  async Task<Object> GetCommissionByCategorie(string categorie)

        {  var result = commissions.Find(commission => commission.categorie == categorie).FirstOrDefault();  
         
            return new {
                result.Id ,
                result.commission,
            };
            
        } 

        



        } 



    
}
            
                 