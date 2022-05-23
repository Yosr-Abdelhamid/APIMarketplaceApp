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
using System.Web.Http.Cors;
namespace APIMarketplaceApp.Controllers

{  
  
    [Route("api/[controller]")]
    [ApiController]

    public class VendeurController : Controller
    {
        private readonly UserService service;
        private readonly IMongoCollection<Vendeur> vendeurs;
      

        public VendeurController (UserService _service,IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("MarketplaceSiteDB");
            vendeurs = database.GetCollection<Vendeur>("Vendeur");
            service = _service;
            vendeurs = database.GetCollection<Vendeur>("Vendeur");
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        
        public ActionResult <List<Vendeur>>GetVendeurs()
        {
            return service.GetVendeurs();
        }

        // GET: VendeurController/Details/5
    
       /*  [HttpGet("{id:length(24)}")]
        public ActionResult<Vendeur> GetVendeur(string id)
        {
            var vendeur = service.GetVendeur(id);
            return Json(vendeur);
        } */

         [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
         [HttpGet("GetAllProducts")]
        public ActionResult <List<ProductVend>> GetProductById(string id)
        {
            var Products = service.GetProductById(id);
            return Json(Products);
        }
        // POST: VendeurController/Create
      
        [HttpPost]
        [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        public IActionResult  Signup( RegisterRequest vendeur)
        {  
            service.Signup(vendeur);
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


        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<object>  Login(AuthenticateRequest model)
        {
           var response =  service.Login(model);
            if (response == null)
                return await Task.FromResult(new ResponseModel(2, "invalid Email or password", null));
                
            return await Task.FromResult(new ResponseModel(1, "", response));
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
                    
                    allUserDTO.Add(new User(vendeur.Id,vendeur.Nom, vendeur.Prenom, vendeur.Email, vendeur.Adresse ,vendeur.Num_Telephone));
                }
                return await Task.FromResult(new ResponseModel(1, "", allUserDTO));
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id:length(24)}")]
         public async Task<object> GetVendeurById(string id)
        {
            
                var user =  vendeurs.Find<Vendeur>(vendeur => vendeur.Id == id).FirstOrDefault();
                var UserDTO = new User(user.Id,user.Nom, user.Prenom, user.Email, user.Adresse ,user.Num_Telephone);
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
                 user.Num_Telephone
            };
        }
           
           
        
    }
}
