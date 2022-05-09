using APIMarketplaceApp.Models;
using APIMarketplaceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public VendeurController (UserService _service)
        {
            service = _service;
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult <List<Vendeur>>GetVendeurs()
        {
            return service.GetVendeurs();
        }

        // GET: VendeurController/Details/5
    
        [HttpGet("{id:length(24)}")]
        public ActionResult<Vendeur> GetVendeur(string id)
        {
            var vendeur = service.GetVendeur(id);
            return Json(vendeur);
        }

        // POST: VendeurController/Create
        [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        [HttpPost]
      
        public object Signup(Vendeur vendeur)
        {  
            /* var filter = Builders<Vendeur>.Filter.Eq(x => x.Email , vendeur.Email) ;
            var result = vendeurs.Find(filter).ToList();
            
                if (result.Count > 0){
                    return new Response
                    { Status = "Error", Message = "Email in used" };
                } */
            service.Signup(vendeur);
            return new Response
            { Status = "Success", Message = "SuccessFully Saved." };

            
        }   

        [AllowAnonymous]
        [HttpPost("authenticate")]

        public ActionResult  Login(AuthenticateRequest model)
        {
           var response =  service.Login(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        


       
        

    }
}
