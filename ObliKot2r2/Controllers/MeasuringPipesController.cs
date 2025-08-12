using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObliKot2r2.Models;

namespace ObliKot2r2.Controllers
{
    [Route("[controller]")]
    public class MeasuringPipesController : Controller
    {
        private readonly AppDbContext _context;

        public MeasuringPipesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetByGmo")]
        public async Task<IActionResult> GetByGmo(int gmoId)
        {
            var pipes = await _context.MeasuringPipes
                .Where(p => p.IdGMO == gmoId)
                .ToListAsync();
            return Ok(pipes);
        }

        [HttpPost("CreateOrUpdate")]
        public async Task<IActionResult> CreateOrUpdate([FromBody] MeasuringPipe pipe)
        {
            if (pipe.Id == 0)
            {
                _context.MeasuringPipes.Add(pipe);
            }
            else
            {
                _context.MeasuringPipes.Update(pipe);
            }

            await _context.SaveChangesAsync();
            return Ok(pipe);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var pipe = await _context.MeasuringPipes.FindAsync(id);
            if (pipe == null) return NotFound();

            _context.MeasuringPipes.Remove(pipe);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("Index")]
        public IActionResult Index(long gmoId)
        {
            var pipes = _context.MeasuringPipes
                .Where(p => p.IdGMO == gmoId)
                .ToList();

            return PartialView("_MeasuringPipesList", pipes);
        }
    }
}
