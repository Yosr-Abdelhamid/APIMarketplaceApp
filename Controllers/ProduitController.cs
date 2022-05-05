using APIMarketplaceApp.Data;
using APIMarketplaceApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIMarketplaceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduitController : ControllerBase
    {
        private readonly DB_MarketplaceContext _context;

        public ProduitController(DB_MarketplaceContext context)
        {
            _context = context;
        }


        // GET: api/PrixProduits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produit>>> GetProduits()
        {
            return await _context.Produits.ToListAsync();
        }

        [HttpGet("SearchProduitAsync")]
        public async Task<IEnumerable<SearchProduitResult>> SearchProduitAsync(string? reference)
        {
            return await _context.GetProcedures().SearchProduitAsync(reference);
        }
        [HttpGet("ProduitSimilairesAsync")]
        public async Task<IEnumerable<ProduitSimilairesResult>> ProduitSimilairesAsync(string? reference, string? categorie)
        {
            return await _context.GetProcedures().ProduitSimilairesAsync(reference, categorie);
        }
        


    }


}

