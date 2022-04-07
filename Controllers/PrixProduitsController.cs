#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIMarketplaceApp.Data;
using APIMarketplaceApp.Models;

namespace APIMarketplaceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrixProduitsController : ControllerBase
    {
        private readonly DB_MarketplaceContext _context;

        public PrixProduitsController(DB_MarketplaceContext context)
        {
            _context = context;
        }

        // GET: api/PrixProduits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrixProduit>>> GetPrixProduits()
        {
            return await _context.PrixProduits.ToListAsync();
        }

        // GET: api/PrixProduits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrixProduit>> GetPrixProduit(int id)
        {
            var prixProduit = await _context.PrixProduits.FindAsync(id);

            if (prixProduit == null)
            {
                return NotFound();
            }

            return prixProduit;
        }

        // PUT: api/PrixProduits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrixProduit(int id, PrixProduit prixProduit)
        {
            if (id != prixProduit.IdPrix)
            {
                return BadRequest();
            }

            _context.Entry(prixProduit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrixProduitExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PrixProduits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PrixProduit>> PostPrixProduit(PrixProduit prixProduit)
        {
            _context.PrixProduits.Add(prixProduit);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PrixProduitExists(prixProduit.IdPrix))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPrixProduit", new { id = prixProduit.IdPrix }, prixProduit);
        }

        // DELETE: api/PrixProduits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrixProduit(int id)
        {
            var prixProduit = await _context.PrixProduits.FindAsync(id);
            if (prixProduit == null)
            {
                return NotFound();
            }

            _context.PrixProduits.Remove(prixProduit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrixProduitExists(int id)
        {
            return _context.PrixProduits.Any(e => e.IdPrix == id);
        }
    }
}
