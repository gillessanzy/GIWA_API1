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
    public class PopulationsController : ControllerBase
    {
        private readonly GIWA_API1Context _context;
        public PopulationsController(GIWA_API1Context context)
        {
            _context = context;
        }

        //Méthode pour convertir une instance de Population en une instance de PopulationDTO
        private static readonly Expression<Func<Population, DTO.PopulationsDTO>> AsPopulationDTO =
            population => new PopulationsDTO
            {
                Id = population.Id,
                annee = population.annee,
                population = population.population,
                Id_pays = population.Id_pays,
                Nom_pays = population.pays.nom_pays
            };

        // GET: api/Populations
        [HttpGet]
        public async Task<List<PopulationsDTO>> GetPopulation()
        {
            return await _context.Population.Include(pop => pop.pays).Select(AsPopulationDTO).ToListAsync();
        }

        // GET: api/Populations/5
        [HttpGet("{id:int}")]
        public Task<PopulationsDTO> GetPopulation(int id)
        {
            var population = _context.Population.Include(pop => pop.pays).
                             Where(pop => pop.Id == id).
                             Select(AsPopulationDTO).FirstOrDefaultAsync();

            if (population == null)
            {
                return null;
            }

            return population;
        }


        //Retourne l'historique de population d'un pays indiqué par son identifiant
        [HttpGet("Pays/{id:int}")]
        public IQueryable<PopulationsDTO> GetPopPaysById(int id)
        {
            var popPaysAnnee = _context.Population.Include(p => p.pays).
                               Where(p => p.pays.Id == id).
                               Select(AsPopulationDTO);
            if (popPaysAnnee == null)
                return null;

            return popPaysAnnee;
        }


        //Retourne l'historique de population d'un pays indiqué par son nom
        [HttpGet("Pays/{pays}")]
        public IQueryable<PopulationsDTO> GetPopPaysByName(string pays)
        {
            var popPaysAnnee = _context.Population.Include(p => p.pays).
                               Where(p => p.pays.nom_pays == pays).
                               Select(AsPopulationDTO);
            if (popPaysAnnee == null)
                return null;

            return popPaysAnnee;
        }

        // PUT: api/Populations/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPopulation(int id, Population population)
        {
            if (id != population.Id)
            {
                return BadRequest();
            }

            _context.Entry(population).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PopulationExists(id))
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

        // POST: api/Populations
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PopulationsDTO>> PostPopulation(PostPopulationsDTO postPopulationDto)
        {
            var population = new Population
            {
                population = postPopulationDto.population,
                annee = postPopulationDto.annee,
                Id_pays = postPopulationDto.Id_pays
            };

            _context.Population.Add(population);
            await _context.SaveChangesAsync();

            var popPaysById = GetPopPaysById(population.Id_pays);

            var popdto = new PopulationsDTO
            {
                Id = popPaysById.Where(p => p.Id == population.Id).Select(p => p.Id).FirstOrDefault(),
                annee = popPaysById.Where(p => p.Id == population.Id).Select(p => p.annee).FirstOrDefault(),
                population = popPaysById.Where(p => p.Id == population.Id).Select(p => p.population).FirstOrDefault(),
                Id_pays = popPaysById.Where(p => p.Id == population.Id).Select(p => p.Id_pays).FirstOrDefault(),
                Nom_pays = popPaysById.Where(p => p.Id == population.Id).Select(p => p.Nom_pays).FirstOrDefault()
            };

            return CreatedAtAction("GetPopulation", new { id = population.Id }, popdto);
        }

        // DELETE: api/Populations/5
        [HttpDelete("{id}")]
        public async Task<PopulationsDTO> DeletePopulation(int id)
        {
            var population = await _context.Population.FindAsync(id);

            var populationDTO = _context.Population.Include(p => p.pays).
                                Where(p => p.Id == id).
                                Select(AsPopulationDTO).FirstOrDefault();
            if (populationDTO == null)
            {
                return null;
            }

            _context.Population.Remove(population);
            await _context.SaveChangesAsync();

            return populationDTO;
        }


        //Retourne la population d'un pays donnée identifié par son Identifiant et d'une année donnée
        [HttpGet("Pays/{id:int}/annee/{annee}")]
        public IQueryable<PopulationsDTO> GetPopPaysAnneeByIdPays(int id, int annee)
        {
            var popPaysAnnee = _context.Population.Include(p => p.pays).
                               Where(p => p.pays.Id == id && p.annee == annee).
                               Select(AsPopulationDTO);
            if (popPaysAnnee == null)
                return null;

            return popPaysAnnee;
        }


        //Retourne la population d'un pays donnée identifié par son nom et d'une année donnée
        [HttpGet("Pays/{pays}/annee/{annee}")]
        public IQueryable<PopulationsDTO> GetPopPaysAnneeByNomPays(string pays, int annee)
        {
            var popPaysAnnee = _context.Population.Include(p => p.pays).
                               Where(p => p.pays.nom_pays == pays && p.annee == annee).
                               Select(AsPopulationDTO);
            if (popPaysAnnee == null)
                return null;

            return popPaysAnnee;
        }


        private bool PopulationExists(int id)
        {
            return _context.Population.Any(e => e.Id == id);
        }
    }
}
