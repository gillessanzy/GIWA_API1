using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GIWA_API1.Data;
using GIWA_API1.Modèles;
using GIWA_API1.DTO;

namespace GIWA_API1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaysController : ControllerBase
    {
        private readonly GIWA_API1Context _context;

        public PaysController(GIWA_API1Context context)
        {
            _context = context;
        }

        //Méthode pour convertir une instance de Pays en une instance de PaysDTO
        private static readonly Expression<Func<Pays, DTO.PaysDTO>> AsPaysDTO =
            pays => new PaysDTO
            {
                Id = pays.Id,
                nom_pays = pays.nom_pays,
                Id_continent = pays.Id_continent,
                nom_continent = pays.continent.nom
            };


        // GET: api/Pays
        [HttpGet]
        public Task<List<PaysDTO>> GetPays()
        {
            return _context.Pays.Include(p => p.continent).
                         Select(AsPaysDTO).ToListAsync();
        }

        // GET: api/Pays/5
        //Récupère un pays donné par son identifiant
        [HttpGet("{id:int}")]
        public Task<PaysDTO> GetPays(int id)
        {
            var paysDTO = _context.Pays.Include(p => p.continent).
                   Where(p => p.Id == id).
                   Select(AsPaysDTO).FirstOrDefaultAsync();

            if (paysDTO == null)
            {
                return null;
            }

            return paysDTO;
        }

        //GET: api/Pays/nom_du_pays
        //Récupère un pays donné par son nom
        [HttpGet("{pays}")]
        public IQueryable<PaysDTO> GetPaysByName(string pays)
        {
            //var pays = await _context.Pays.FindAsync(id);
            var paysDTO = _context.Pays.AsQueryable().Include(p => p.continent).
                   Where(p => p.nom_pays == pays).
                   Select(AsPaysDTO);

            if (paysDTO == null)
            {
                return null;
            }

            return paysDTO;
        }

        // PUT: api/Pays/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPays(int id, Pays pays)
        {
            if (id != pays.Id)
            {
                return BadRequest();
            }

            _context.Entry(pays).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaysExists(id))
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

        // POST: api/Pays
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PaysDTO>> PostPays(PostPaysDTO postPaysDTO)
        {
            var pays = new Pays
            {
                nom_pays = postPaysDTO.nom_pays,
                Id_continent = postPaysDTO.Id_continent
            };
            _context.Pays.Add(pays);
            await _context.SaveChangesAsync();

            var paysByName= GetPaysByName(pays.nom_pays);

            var paysdto = new PaysDTO
            {
                Id = paysByName.Select(p => p.Id).FirstOrDefault(),
                nom_pays = paysByName.Select(p => p.nom_pays).FirstOrDefault(),
                Id_continent = paysByName.Select(p => p.Id_continent).FirstOrDefault(),
                nom_continent = paysByName.Select(p => p.nom_continent).FirstOrDefault(),
            };

            return CreatedAtAction("GetPays", new { id = pays.Id }, paysdto);
        }

        // DELETE: api/Pays/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PostPaysDTO>> DeletePays(int id)
        {
            var pays = await _context.Pays.FindAsync(id);
            if (pays == null)
            {
                return NotFound();
            }

            _context.Pays.Remove(pays);
            await _context.SaveChangesAsync();

            var postPaysDto = new PostPaysDTO
            {
                Id = pays.Id,
                nom_pays = pays.nom_pays,
                Id_continent = pays.Id_continent
            };

            return postPaysDto;
        }

        private bool PaysExists(int id)
        {
            return _context.Pays.Any(e => e.Id == id);
        }
    }
}
