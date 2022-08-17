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
using System.Net;
using System.Net.Http;
using AutoMapper;
using System.Web.Http.Cors;
namespace APIMarketplaceApp.Controllers

{  
  
    [Route("api/[controller]")]
    [ApiController]

    public class VendeurController : Controller
    {
        private readonly UserService service;
        private readonly IMongoCollection<Vendeur> vendeurs;
        private readonly IMongoCollection<ProductVend> produits;

        private readonly IMongoCollection<Notif> notifications ;
        private readonly IMongoCollection<PortfeuilleVendeur> portfeuilles ;
        private readonly IMapper _mapper;
      

        public VendeurController (UserService _service,IConfiguration configuration ,IMapper mapper)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("MarketplaceSiteDB");
            vendeurs = database.GetCollection<Vendeur>("Vendeur");
            produits = database.GetCollection<ProductVend>("ProductVend");
            notifications = database.GetCollection<Notif>("Notifications");
            portfeuilles =database.GetCollection<PortfeuilleVendeur>("PortfeuilleVendeur");
            service = _service;
            _mapper = mapper;
            
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        
        public ActionResult <List<Vendeur>>GetVendeurs()
        {
            return service.GetVendeurs();
        }
      
        [HttpPost]
        [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        public  async Task<object>  Signup(RegisterRequest vendeur)

        {   var response = service.Signup(vendeur ,Request.Headers["origin"]);
            if (response == null) {
                return BadRequest(new { message = "Email in use." });
            } 
            return Ok(new { message = "Registration successful" });   
        }    

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("addProduct")]
        //[Consumes("multipart/form-data")]
        public IActionResult AddProduct([FromForm] RequestProduct produit ,[FromForm] IFormFile image_prod ,[FromForm] string id)
        {  
            service.AddProduct(produit,image_prod,id);
            return Ok(new { message = "Product added" });
        }   

         // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
         [HttpGet("GetAllProducts")]
        public ActionResult <List<ProductVend>> GetProductById(string id)
        {
            var Products = service.GetProductById(id);
            return Json(Products);
        }
        // POST: VendeurController/Create
         [HttpGet("GetAllNotifications")]
        public ActionResult <List<Notif>> GetAllNotifications(string id_vendeur)
        {
            var notifs = notifications.Find<Notif>(x => x.Id_vendeur == id_vendeur).ToList();

            return Json(notifs);
        }


        [HttpPut]
        public JsonResult Put([FromForm] ProductModel produit)
        {    
            var filter = Builders<ProductVend>.Filter.Eq("Id_prod", produit.Id_prod);
            var Produit = _mapper.Map<ProductVend>(produit);
            var update = Builders<ProductVend>.Update.Set("Reference", Produit.Reference)
                                                    .Set("sous_famille_prod", Produit.sous_famille_prod)
                                                    .Set("Brand", Produit.Brand)
                                                    .Set("quantity", Produit.quantity)
                                                    .Set("description_prod", Produit.description_prod)
                                                    .Set("prix_prod", Produit.prix_prod);

            this.produits.UpdateOne(filter, update);

            return new JsonResult("Updated Successfully");
        }
        [HttpGet("GetProductByCategory")]
        public ActionResult <List<ProductVend>> GetProductByCategory(string sous_famille_prod){

             var Products = service.GetProductByCategory(sous_famille_prod);
            return Json(Products);
        }

        [HttpGet("GetProductByReference")]
        public async Task<object> GetProductByReference (string  Reference)
        {
            var prod = produits.Find<ProductVend>(x => x.Reference == Reference).FirstOrDefault();
            return prod ;
        
        }
        [HttpGet("GetProductByBrand")]
        public ActionResult <List<ProductVend>> GetProductByBrand (string Brand , string sous_famille_prod)
        {
            var prod = produits.Find<ProductVend>(x => x.sous_famille_prod == sous_famille_prod && x.Brand == Brand).ToList();
            return Json(prod) ;
        
        }

        [HttpPut("UpdateImage")]
        public JsonResult Put([FromForm] ImageUpload image , [FromForm] IFormFile image_prod, [FromForm] string Id_prod)
        {
            var filter = Builders<ProductVend>.Filter.Eq("Id_prod", Id_prod);
            var Produit = _mapper.Map<ProductVend>(image);
            MemoryStream memoryStream = new MemoryStream();
            image_prod.OpenReadStream().CopyTo(memoryStream);
            Produit.image_prod= Convert.ToBase64String(memoryStream.ToArray()) ;
            var update = Builders<ProductVend>.Update.Set("image_prod", Produit.image_prod);
            this.produits.UpdateOne(filter, update);
            return new JsonResult("Updated Successfully");
        }

        [HttpPut("UpdateProfile")]
        public JsonResult PutProfile ([FromForm] UpdateVendeur vendeur)
        {    
            var filter = Builders<Vendeur>.Filter.Eq("Id", vendeur.id);
            var Vendeur = _mapper.Map<Vendeur>(vendeur);
            var update = Builders<Vendeur>.Update.Set("Nom" , vendeur.Nom)
                                                .Set("Prenom", vendeur.Prenom)
                                                .Set("Email" , vendeur.Email)
                                                .Set("Adresse" , vendeur.Adresse)
                                                .Set("Num_Telephone" , vendeur.Num_Telephone)
                                                .Set("ZipCode" , vendeur.ZipCode)
                                                .Set("Organization" , vendeur.Organization) ;                         

            this.vendeurs.UpdateOne(filter, update);

            return new JsonResult("Updated Successfully");
        }

         [HttpPut("AddImageOrg")]
        public JsonResult AddImageOrg([FromForm] ImageOrganisation image , [FromForm] IFormFile image_org, [FromForm] string id)
        {
            var filter = Builders<Vendeur>.Filter.Eq("Id", id);
            var vendeur = _mapper.Map<Vendeur>(image);
            MemoryStream memoryStream = new MemoryStream();
            image_org.OpenReadStream().CopyTo(memoryStream);
            vendeur.image_org= Convert.ToBase64String(memoryStream.ToArray()) ;
            var update = Builders<Vendeur>.Update.Set("image_org", vendeur.image_org);
            this.vendeurs.UpdateOne(filter, update);
            return new JsonResult("Logo updated Successfully");
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
         var prod = this.produits.Find(x => x.Id_prod == id).FirstOrDefaultAsync();

        if (prod is null)
        {
            return NotFound();
        }

        await service.RemoveAsync(id);

        return NoContent();
        }

        /*  [HttpPost("deleteNotif")]
        public async Task<IActionResult> DeleteNotif(string id)
        {
         var notif = this.notifications.Find(x => x.Id == id).FirstOrDefaultAsync();

        if (notif is null)
        {
            return NotFound();
        }
         await service.RemoveAsy(id);

        return NoContent();
        } */
        

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<object>  Login(AuthenticateRequest model)
        {
           var response =  service.Login(model);
            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect." });
                
            return await Task.FromResult(new ResponseModel(1, "", response));
        }

        [HttpPost("verify-email")]
        [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        public async Task<object> VerifyEmail(VerifyEmailRequest model)

        {
            var resp =service.VerifyEmail(model.Token);
            if (resp == null)
                return BadRequest(new { message = "Account not verified." });
            
            return Ok(new { message = "Verification successful, you can now login" });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequest model)
        {
            service.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            service.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }
        

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAllUser")]
         public async Task<object> GetAllVendeurs()
        {
            
                List<User> allUserDTO = new List<User>();
                var users =  vendeurs.Find(vendeur => true).ToList();
                foreach (var vendeur in users)
                {
                    
                    allUserDTO.Add(new User(vendeur.Id,vendeur.Nom, vendeur.Prenom, vendeur.Email, vendeur.Adresse ,
                    vendeur.Num_Telephone,vendeur.ZipCode, vendeur.Organization , vendeur.isActived));
                }
                return await Task.FromResult(new ResponseModel(1, "", allUserDTO));
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id:length(24)}")]
         public async Task<object> GetVendeurById(string id)
        {
            
                var user =  vendeurs.Find<Vendeur>(vendeur => vendeur.Id == id).FirstOrDefault();
                var UserDTO = new User((user.Id).ToString(),user.Nom, user.Prenom, user.Email, user.Adresse ,user.Num_Telephone , user.ZipCode , user.Organization, user.isActived);
                //UserDTO.Add(new User(user.Id,user.Nom, user.Prenom, user.Email, user.Adresse ,user.Num_Telephone));
                return await Task.FromResult(new ResponseModel(1, "", UserDTO));
        }


        [HttpGet("GetUserProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> GetUserProfile() {
            string userId = User.Claims.First(c => c.Type == "id").Value;
            var user = vendeurs.Find<Vendeur>(vendeur => vendeur.Id == userId).FirstOrDefault();
            return new
            {       
                user.Id,
                 user.Nom,
                 user.Prenom,
                 user.Email,
                 user.Adresse,
                 user.Num_Telephone,
                 user.ZipCode,
                 user.Organization,
                 user.image_org
            };
        }
           
        [HttpPost("AddPortfeuille")]
        public IActionResult  AddPortfeuille(RequestPortfeuille portfeuille)

        {    
            service.AddPortfeuille(portfeuille) ;
            return Ok(new { message = "Portfeuille added" });
        } 


        [HttpGet("GetPortfeuille")]
        public async Task<object> GetPortfeuille (string  Id)
        {
            var poftf = portfeuilles.Find<PortfeuilleVendeur>(x => x.Id == Id).FirstOrDefault();
            return poftf ;
        
        }

         [HttpPut("UpdateSold")]
        public JsonResult Put([FromForm] ChangeSoldPortf solde)
        {    
            var filter = Builders<PortfeuilleVendeur>.Filter.Eq("Id_portf", solde.Id_portf);
            var update = Builders<PortfeuilleVendeur>.Update.Set("Sold", solde.Sold);
            this.portfeuilles.UpdateOne(filter, update);

            return new JsonResult("Updated Successfully");
        }

        [HttpGet("GetPocketSeller")]
        public async Task<object> GetPocketSeller()

       {    
            var query = from o in vendeurs.AsQueryable()
             join i in portfeuilles.AsQueryable()
             on o.Id equals i.Id
             select new PotfeuilleVendeur
            {
                Id = o.Id,
                Nom = o.Nom,
                Prenom= o.Prenom,
                Email = o.Email ,
                Num_Telephone = o.Num_Telephone ,
                Organization = o.Organization ,
                Sold = i.Sold,
                Id_portf = i.Id_portf
            
            };
            var pocketseller =  query.ToList();
            return pocketseller ; 
            
        }
        
    }
}
