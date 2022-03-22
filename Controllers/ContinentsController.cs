using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ContinentsController : ControllerBase
    {
        private readonly GIWA_API1Context _context;

        public ContinentsController(GIWA_API1Context context)
        {
            _context = context;
        }

        // GET: api/Continents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Continent>>> GetContinent()
        {
            return await _context.Continent.ToListAsync();
        }

        // GET: api/Continents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Continent>> GetContinent(int id)
        {
            var continent = await _context.Continent.FindAsync(id);

            if (continent == null)
            {
                return NotFound();
            }

            return continent;
        }

        // PUT: api/Continents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContinent(int id, Continent continent)
        {
            if (id != continent.Id)
            {
                return BadRequest();
            }

            _context.Entry(continent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContinentExists(id))
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

        // POST: api/Continents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Continent>> PostContinent(Continent continent)
        {
            _context.Continent.Add(continent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContinent", new { id = continent.Id }, continent);
        }

        // DELETE: api/Continents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Continent>> DeleteContinent(int id)
        {
            var continent = await _context.Continent.FindAsync(id);
            if (continent == null)
            {
                return NotFound();
            }

            _context.Continent.Remove(continent);
            await _context.SaveChangesAsync();

            return continent;
        }


        //GET: api/Continents/1/Pays
        //Retourne la liste des pays d'un continent identifié par son Id
        [HttpGet("{id:int}/Pays")]
        public IQueryable<ContinentsPaysDTO> GetListePaysByContinentId(int id)
        {
            //jointure entre continents et pays
            var listeContinentPays = _context.Continent.
                                     Join(_context.Pays,
                                     continent => continent.Id,
                                     pays => pays.Id_continent,
                                     (continent, pays) => new { Continent = continent, Pays = pays }).
                                     Where(listeContinentPays => listeContinentPays.Continent.Id == id).
                                     Select(continentPays => new ContinentsPaysDTO { 
                                         Id_continent = continentPays.Continent.Id,
                                         nom_continent = continentPays.Continent.nom,
                                         Id_pays = continentPays.Pays.Id,
                                         nom_pays = continentPays.Pays.nom_pays
                                     });

            if(listeContinentPays == null)
            {
                return null;
            }

            return listeContinentPays;

        }


        //GET: api/Continents/Afrique/Pays
        //Retourne la liste des pays d'un continent identifié par son nom
        [HttpGet("{nom_continent}/Pays")]
        public IQueryable<ContinentsPaysDTO> GetListePaysByContinentName(string nom_continent)
        {
            //jointure entre continents et pays
            var listeContinentPays = _context.Continent.
                                     Join(_context.Pays,
                                     continent => continent.Id,
                                     pays => pays.Id_continent,
                                     (continent, pays) => new { Continent = continent, Pays = pays }).
                                     Where(listeContinentPays => listeContinentPays.Continent.nom == nom_continent).
                                     Select(continentPays => new ContinentsPaysDTO
                                     {
                                         Id_continent = continentPays.Continent.Id,
                                         nom_continent = continentPays.Continent.nom,
                                         Id_pays = continentPays.Pays.Id,
                                         nom_pays = continentPays.Pays.nom_pays
                                     });

            if (listeContinentPays == null)
            {
                return null;
            }

            return listeContinentPays;

        }

        [HttpGet("{id:int}/PopulationAnnee/{annee}")]
        public ContinentsPopulationAnneeDTO GetContinentPopulationAnneeById(int id, int annee)
        {
            var jointureContinentPays = _context.Continent.
                                        Join(_context.Pays,
                                        continent => continent.Id,
                                        pays => pays.Id_continent,
                                        (continent, pays) => new { Continent = continent, Pays = pays }).
                                        Join(_context.Population,
                                        continentPays => continentPays.Pays.Id,
                                        population => population.Id_pays,
                                        (continentPays, population) => new { ContinentPays = continentPays, Population = population }).
                                        Where(listeContPaysPop => listeContPaysPop.ContinentPays.Continent.Id == id
                                                                   && listeContPaysPop.Population.annee == annee);

            var listeContPopAnnee = jointureContinentPays.
                                    Select(contPaysPop => new ContinentsPopulationAnneeDTO
                                    {
                                        Id_continent = contPaysPop.ContinentPays.Continent.Id,
                                        nom_continent = contPaysPop.ContinentPays.Continent.nom,
                                        annee = contPaysPop.Population.annee,
                                        population = jointureContinentPays.GroupBy(x => x.ContinentPays.Continent.Id).
                                                                             Select(group => group.Sum(x => x.Population.population)).
                                                                             FirstOrDefault()
                                    });

            if (listeContPopAnnee == null)
                return null;

            return listeContPopAnnee.FirstOrDefault();
        }

        [HttpGet("{nom_continent}/PopulationAnnee/{annee}")]
        public ContinentsPopulationAnneeDTO GetContinentPopulationAnneeByName(string nom_continent, int annee)
        {
            var jointureContinentPays = _context.Continent.
                                        Join(_context.Pays,
                                        continent => continent.Id,
                                        pays => pays.Id_continent,
                                        (continent, pays) => new { Continent = continent, Pays = pays }).
                                        Join(_context.Population,
                                        continentPays => continentPays.Pays.Id,
                                        population => population.Id_pays,
                                        (continentPays, population) => new { ContinentPays = continentPays, Population = population }).
                                        Where(listeContPaysPop => listeContPaysPop.ContinentPays.Continent.nom == nom_continent
                                                                   && listeContPaysPop.Population.annee == annee);

            var listeContPopAnnee = jointureContinentPays.
                                    Select(contPaysPop => new ContinentsPopulationAnneeDTO
                                    {
                                        Id_continent = contPaysPop.ContinentPays.Continent.Id,
                                        nom_continent = contPaysPop.ContinentPays.Continent.nom,
                                        annee = contPaysPop.Population.annee,
                                        population = jointureContinentPays.GroupBy(x => x.ContinentPays.Continent.Id).
                                                                             Select(group => group.Sum(x => x.Population.population)).
                                                                             FirstOrDefault()
                                    });

            if (listeContPopAnnee == null)
                return null;

            return listeContPopAnnee.FirstOrDefault();
        }

        private bool ContinentExists(int id)
        {
            return _context.Continent.Any(e => e.Id == id);
        }
    }
}
