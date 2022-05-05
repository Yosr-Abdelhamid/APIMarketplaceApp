using APIMarketplaceApp.Models;
using APIMarketplaceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIMarketplaceApp.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class VendeurController : Controller
    {
        private readonly UserService service;

        public VendeurController (UserService _service)
        {
            service = _service;
        }

        // GET: VendeurController
        [HttpGet]
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
        [HttpPost]
      
        public ActionResult <Vendeur> Create(Vendeur vendeur)
        {
            service.Create(vendeur);
            return Json(vendeur);
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]

        public ActionResult Login ([FromBody] Vendeur vendeur)
        {
            var token = service.Authenticate(vendeur.Email, vendeur.MotDePasse);
            if (token == null)
                return Unauthorized();
            return Ok(new { token, vendeur});
        }

    }
}
