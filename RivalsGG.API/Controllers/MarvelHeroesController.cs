using Microsoft.AspNetCore.Mvc;
using RivalsGG.BLL.Services;
using RivalsGG.Core.Models;

namespace RivalsGG.API.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class MarvelHeroesController : ControllerBase
        {
            private readonly MarvelApiClient _marvelApiClient;

            public MarvelHeroesController(MarvelApiClient marvelApiClient)
            {
                _marvelApiClient = marvelApiClient;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<MarvelHero>>> GetHeroes()
            {
                var heroes = await _marvelApiClient.GetHeroesAsync();
                return Ok(heroes);
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<MarvelHero>> GetHero(string id)
            {
                var hero = await _marvelApiClient.GetHeroByIdAsync(id);

                if (hero == null)
                {
                    return NotFound();
                }

                return Ok(hero);
            }
        }
}
