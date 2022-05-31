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
        private readonly IMapper _mapper;
      

        public VendeurController (UserService _service,IConfiguration configuration ,IMapper mapper)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("MarketplaceSiteDB");
            vendeurs = database.GetCollection<Vendeur>("Vendeur");
            produits = database.GetCollection<ProductVend>("ProductVend");
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
        public  async Task<object>  Signup( RegisterRequest vendeur)

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
            return new JsonResult("Image added Successfully");
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
                    vendeur.Num_Telephone,vendeur.ZipCode, vendeur.Organization ));
                }
                return await Task.FromResult(new ResponseModel(1, "", allUserDTO));
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id:length(24)}")]
         public async Task<object> GetVendeurById(string id)
        {
            
                var user =  vendeurs.Find<Vendeur>(vendeur => vendeur.Id == id).FirstOrDefault();
                var UserDTO = new User(user.Id,user.Nom, user.Prenom, user.Email, user.Adresse ,user.Num_Telephone , user.ZipCode , user.Organization);
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
                 user.Organization
            };
        }
           
           
        
    }
}
