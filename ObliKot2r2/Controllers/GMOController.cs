using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObliKot2r2.Models;
using System.Linq;

namespace ObliKot2r2.Controllers
{
    [Route("[controller]")]
    public class GMOController : Controller
    {
        private readonly AppDbContext _context;

        public GMOController(AppDbContext context)
        {
            _context = context;
        }

        // Вспомогательные методы
        private async Task<List<Area>> GetAreasWithFullInfo()
        {
            return await _context.Areas
                .Include(a => a.Subdivision)
                .ThenInclude(s => s.Philia)
                .OrderBy(a => a.Subdivision.Philia.NamePhilia)
                .ThenBy(a => a.Subdivision.NameSubdivision)
                .ThenBy(a => a.NameArea)
                .ToListAsync();
        }

        private async Task<object> GetAreasForSelectAsync()
        {
            return await _context.Areas
                .Include(a => a.Subdivision)
                .ThenInclude(s => s.Philia)
                .Select(a => new {
                    a.IdArea,
                    FullName = $"{a.Subdivision.Philia.NamePhilia} → {a.Subdivision.NameSubdivision} → {a.NameArea}"
                })
                .ToListAsync();
        }

        // Основные методы контроллера

        [HttpGet("Index")]
        public async Task<IActionResult> Index(int? areaId, string sortOrder = "", string searchString = "")
        {
            // Сортировка
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.AreaSortParam = sortOrder == "area_asc" ? "area_desc" : "area_asc";
            ViewBag.SapIdSortParam = sortOrder == "sapid_asc" ? "sapid_desc" : "sapid_asc";

            ViewBag.Areas = await GetAreasWithFullInfo();

            var query = _context.GasMeasuringObjects
                .Include(g => g.Area)
                .ThenInclude(a => a.Subdivision)
                .ThenInclude(s => s.Philia)
                .AsQueryable();

            // Фильтрация
            if (areaId.HasValue)
            {
                query = query.Where(g => g.IdArea == areaId.Value);
                ViewBag.FilteredArea = await _context.Areas
                    .Include(a => a.Subdivision)
                    .FirstOrDefaultAsync(a => a.IdArea == areaId.Value);
            }

            // Поиск
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(g => g.NameGMO.Contains(searchString) ||
                                      g.IdGmoSAP.Contains(searchString));
            }

            // Сортировка
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(g => g.NameGMO),
                "area_asc" => query.OrderBy(g => g.Area.NameArea),
                "area_desc" => query.OrderByDescending(g => g.Area.NameArea),
                "sapid_asc" => query.OrderBy(g => g.IdGmoSAP),
                "sapid_desc" => query.OrderByDescending(g => g.IdGmoSAP),
                _ => query.OrderBy(g => g.NameGMO)
            };

            return View(await query.AsNoTracking().ToListAsync());
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Areas = await GetAreasForSelectAsync();
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GasMeasuringObject model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Areas = await GetAreasForSelectAsync();
            return View(model);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.GasMeasuringObjects.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.Areas = await _context.Areas
                .Include(a => a.Subdivision)
                .ThenInclude(s => s.Philia)
                .Select(a => new {
                    a.IdArea,
                    FullName = $"{a.Subdivision.Philia.NamePhilia} → {a.Subdivision.NameSubdivision} → {a.NameArea}"
                })
                .ToListAsync();
            return View(model);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, GasMeasuringObject model)
        {
            if (id != model.IdGMO)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GasMeasuringObjectExists(model.IdGMO))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.Areas = await GetAreasForSelectAsync();
            return View(model);
        }

        private async Task<bool> GasMeasuringObjectExists(long id)
        {
            return await _context.GasMeasuringObjects.AnyAsync(e => e.IdGMO == id);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var model = await _context.GasMeasuringObjects.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            try
            {
                _context.GasMeasuringObjects.Remove(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Не удалось удалить объект. Возможно, есть связанные данные.");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("GetMainData")]
        public async Task<IActionResult> GetMainData(int id)
        {
            var gmo = await _context.GasMeasuringObjects
                .Include(g => g.Area)
                .FirstOrDefaultAsync(g => g.IdGMO == id);

            if (gmo == null)
            {
                return NotFound();
            }

            ViewBag.Areas = await _context.Areas
                .Include(a => a.Subdivision)
                .ThenInclude(s => s.Philia)
                .Select(a => new {
                    a.IdArea,
                    FullName = $"{a.Subdivision.Philia.NamePhilia} → {a.Subdivision.NameSubdivision} → {a.NameArea}"
                })
                .ToListAsync();

            return PartialView("_MainDataPartial", gmo);
        }

        [HttpPost("UpdateDetails")]
        public async Task<IActionResult> UpdateDetails([FromBody] GasMeasuringObject model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GasMeasuringObjectExists(model.IdGMO))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpGet("GetPointIOs")]
        public async Task<IActionResult> GetPointIOs(long id)
        {
            // Проверяем существование ГМО
            var gmoExists = await _context.GasMeasuringObjects.AnyAsync(g => g.IdGMO == id);
            if (!gmoExists)
            {
                return NotFound("Газовимірювальний об'єкт не знайдено");
            }

            var pointIOs = await _context.PointIOs
                .Include(p => p.PointIOType)
                .Where(p => p.IdGMO == id)
                .ToListAsync();

            // Если нет записей, возвращаем пустой список с сообщением
            if (!pointIOs.Any())
            {
                return PartialView("_PointIOListPartial", new List<PointIO>());
            }

            return PartialView("_PointIOListPartial", pointIOs);
        }

        [HttpGet("GetPointIO")]
        public async Task<IActionResult> GetPointIO(long id)
        {
            var pointIO = await _context.PointIOs.FindAsync(id);
            if (pointIO == null)
            {
                return NotFound();
            }

            ViewBag.PointIOTypes = await _context.PointIOTypes.ToListAsync();
            return PartialView("_PointIOFormPartial", pointIO);
        }

        [HttpPost("SavePointIO")]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePointIO([FromBody] PointIO pointIO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                
                return Json(new { success = false, error = "Невірні дані", modelErrors = errors });
            }


            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(pointIO.IdPointIOSAP))
                    {
                        _context.Add(pointIO);
                    }
                    else
                    {
                        _context.Update(pointIO);
                    }
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateException ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
            }
            return Json(new { success = false, error = "Невірні дані" });
        }

        [HttpPost("DeletePointIO")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePointIO(string id)
        {
            var pointIO = await _context.PointIOs.FindAsync(id);
            if (pointIO == null)
            {
                return NotFound();
            }

            _context.PointIOs.Remove(pointIO);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet("GetPointIOTypes")]
        public async Task<JsonResult> GetPointIOTypes()
        {
            var types = await _context.PointIOTypes
                .Select(t => new { id = t.IdTypePointIO, name = t.NameTypePointIO })
                .ToListAsync();
            return Json(types);
        }

        //MeasuringPipesController
        
        
       

    }
}