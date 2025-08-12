using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObliKot2r2.Models;
using System.Linq;

namespace ObliKot2r2.Controllers
{
    [Route("[controller]")]
    public class PointIOController : Controller
    {
        private readonly AppDbContext _context;

        public PointIOController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("api/GetByGMO")]
        public async Task<IActionResult> GetByGMO(long gmoId)
        {
            try
            {
                var points = await _context.PointIOs
                    .Where(p => p.IdGMO == gmoId)
                    .Include(p => p.PointIOType)
                    .Select(p => new
                    {
                        idPointIOSAP = p.IdPointIOSAP,
                        namePointIO = p.NamePointIO,
                        pointIOType = p.PointIOType != null ? new
                        {
                            p.PointIOType.IdTypePointIO,
                            p.PointIOType.NameTypePointIO
                        } : null,
                        eic = p.EIC,
                        dateBegin = p.DateBegin.ToString("yyyy-MM-dd"),
                        dateEnd = p.DateEnd.HasValue ? p.DateEnd.Value.ToString("yyyy-MM-dd") : null,
                        idAnother = p.IdAnother
                    })
                    .ToListAsync();

                return Json(points);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                //_logger.LogError(ex, "Error getting points for GMO");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("api/GetTypes")]
        public async Task<IActionResult> GetTypes()
        {
            var types = await _context.PointIOTypes
                .OrderBy(t => t.NameTypePointIO)
                .Select(t => new
                {
                    idTypePointIO = t.IdTypePointIO,
                    nameTypePointIO = t.NameTypePointIO
                })
                .ToListAsync();

            return Json(types);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var point = await _context.PointIOs
                .Include(p => p.PointIOType)
                .FirstOrDefaultAsync(p => p.IdPointIOSAP == id);

            if (point == null) return NotFound();

            return Json(new
            {
                idPointIOSAP = point.IdPointIOSAP,
                idGMO = point.IdGMO,
                namePointIO = point.NamePointIO,
               
                typeName = point.PointIOType?.NameTypePointIO,
                eic = point.EIC,
                dateBegin = point.DateBegin.ToString("yyyy-MM-dd"),
                dateEnd = point.DateEnd?.ToString("yyyy-MM-dd"),
                idAnother = point.IdAnother
            });
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] PointIO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (string.IsNullOrEmpty(model.IdPointIOSAP))
                {
                    return BadRequest("IdPointIOSAP is required");
                }

                if (await _context.PointIOs.AnyAsync(p => p.IdPointIOSAP == model.IdPointIOSAP))
                {
                    _context.Update(model);
                }
                else
                {
                    model.DateBegin = model.DateBegin == DateTime.MinValue ? DateTime.Today : model.DateBegin;
                    await _context.AddAsync(model);
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (DbUpdateException ex)
            {
                // _logger.LogError(ex, "Error saving point IO");
                return StatusCode(500, new { error = "Database update error" });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Unexpected error saving point IO");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var point = await _context.PointIOs.FindAsync(id);
                if (point == null) return NotFound();

                _context.PointIOs.Remove(point);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error deleting point IO {PointId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        public async Task<IActionResult> Index(long? gmoId, string sortOrder = "")
        {
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.TypeSortParam = sortOrder == "type_asc" ? "type_desc" : "type_asc";
            ViewBag.DateSortParam = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            var query = _context.PointIOs
                .Include(p => p.GMO)
                .Include(p => p.PointIOType)
                .AsQueryable();

            if (gmoId.HasValue)
            {
                query = query.Where(p => p.IdGMO == gmoId);
                ViewBag.GMO = await _context.GasMeasuringObjects.FindAsync(gmoId);
            }

            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(p => p.NamePointIO),
                "type_asc" => query.OrderBy(p => p.PointIOType.NameTypePointIO),
                "type_desc" => query.OrderByDescending(p => p.PointIOType.NameTypePointIO),
                "date_asc" => query.OrderBy(p => p.DateBegin),
                "date_desc" => query.OrderByDescending(p => p.DateBegin),
                _ => query.OrderBy(p => p.NamePointIO)
            };

            return View(await query.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Create(long gmoId)
        {
            ViewBag.Types = await _context.PointIOTypes.ToListAsync();
            ViewBag.GMO = await _context.GasMeasuringObjects.FindAsync(gmoId);
            return View(new PointIO { IdGMO = gmoId, DateBegin = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PointIO model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { gmoId = model.IdGMO });
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "Error creating point IO");
                    ModelState.AddModelError("", "Не удалось создать точку");
                }
            }

            ViewBag.Types = await _context.PointIOTypes.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var model = await _context.PointIOs
                .Include(p => p.GMO)
                .FirstOrDefaultAsync(p => p.IdPointIOSAP == id);

            if (model == null) return NotFound();

            ViewBag.Types = await _context.PointIOTypes.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PointIO model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { gmoId = model.IdGMO });
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "Error updating point IO {PointId}", model.IdPointIOSAP);
                    ModelState.AddModelError("", "Не удалось обновить точку");
                }
            }

            ViewBag.Types = await _context.PointIOTypes.ToListAsync();
            return View(model);
        }

        
    }
}