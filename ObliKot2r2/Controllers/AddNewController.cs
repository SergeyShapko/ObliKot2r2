using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObliKot2r2.Models;
using System;

namespace ObliKot2r2.Controllers
{
    public class AddNewController : Controller
    {
        private readonly AppDbContext _context;

        public AddNewController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Наприклад, перенаправляємо на сторінку філій за замовчуванням
            return RedirectToAction("CreatePhilia");

            // Або повертаємо спеціальне представлення:
            // return View();
        }

        [HttpGet]
        public IActionResult CreatePhilia()
        {
            ViewBag.ExistingPhilas = _context.Philia.ToList();
            ViewBag.PhilasCount = _context.Philia.Count();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePhilia(Philia model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Філія успішно додана!";
                return RedirectToAction(nameof(CreatePhilia));
            }
            ViewBag.ExistingPhilas = _context.Philia.ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult EditPhilia(int id)
        {
            var model = _context.Philia.Find(id);
            if (model == null) return NotFound();
            return PartialView("_EditPhiliaPartial", model); //  имя представления
        }

        [HttpPost]
        public IActionResult EditPhilia(Philia model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    newCount = _context.Philia.Count() // возвращаем новое количество
                });
            }

            return PartialView("_EditPhiliaPartial", model);
        }


        [HttpGet]
        public IActionResult DeletePhilia(int id)
        {
            var model = _context.Philia.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return PartialView("_DeletePhiliaPartial", model);
        }

        [HttpPost, ActionName("DeletePhilia")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePhiliaConfirmed(int id)
        {
            var philia = _context.Philia.Find(id);
            if (philia == null)
            {
                return Json(new { success = false, message = "Філіал не знайдено" });
            }

            try
            {
                _context.Philia.Remove(philia);
                _context.SaveChanges();
                return Json(new
                {
                    success = true,
                    message = "Філіал успішно видалено",
                    newCount = _context.Philia.Count()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Помилка при видаленні: {ex.Message}"
                });
            }
        }


        // Підрозділи
        //public IActionResult Subdivision()
        //{
        //    var model = _context.Subdivisions.Include(s => s.Philia).ToList();
        //    return View(model);
        //}
        public IActionResult Subdivision(int? philiaId, string sortOrder = "", string searchString = "")
        {
            // Устанавливаем параметры сортировки
            ViewBag.NameSortParam = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.PhiliaSortParam = sortOrder == "philia_asc" ? "philia_desc" : "philia_asc";
            ViewBag.SapIdSortParam = sortOrder == "sapid_asc" ? "sapid_desc" : "sapid_asc";

            var subdivisions = _context.Subdivisions
                .Include(s => s.Philia)
                .AsQueryable();

            // Фильтрация по филиалу
            if (philiaId.HasValue)
            {
                subdivisions = subdivisions.Where(s => s.IdPhilia == philiaId.Value);
                ViewBag.FilteredPhilia = _context.Philia.Find(philiaId.Value)?.NamePhilia;
            }

            // Поиск
            if (!string.IsNullOrEmpty(searchString))
            {
                subdivisions = subdivisions.Where(s =>
                    s.NameSubdivision.Contains(searchString) ||
                    s.IdSubdivisionSAP.Contains(searchString));
            }

            // Сортировка
            subdivisions = sortOrder switch
            {
                "name_asc" => subdivisions.OrderBy(s => s.NameSubdivision),
                "name_desc" => subdivisions.OrderByDescending(s => s.NameSubdivision),
                "philia_asc" => subdivisions.OrderBy(s => s.Philia.NamePhilia),
                "philia_desc" => subdivisions.OrderByDescending(s => s.Philia.NamePhilia),
                "sapid_asc" => subdivisions.OrderBy(s => s.IdSubdivisionSAP),
                "sapid_desc" => subdivisions.OrderByDescending(s => s.IdSubdivisionSAP),
                _ => subdivisions.OrderBy(s => s.NameSubdivision) // Сортировка по умолчанию
            };

            return View(subdivisions.ToList());
        }

        [HttpGet]
        public IActionResult CreateSubdivision()
        {
            ViewBag.Philias = _context.Philia.ToList();
            return View();
        }


        [HttpPost]
        public IActionResult CreateSubdivision(Subdivision model)
        {
            if (ModelState.IsValid)
            {
                _context.Subdivisions.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Subdivision");
            }
            ViewBag.Philias = _context.Philia.ToList();
            return View(model);
        }

        // Редактирование подразделения
        [HttpGet]
        public IActionResult EditSubdivision(int id)
        {
            var model = _context.Subdivisions.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.Philias = _context.Philia.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSubdivision(Subdivision model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Підрозділ успішно оновлено!";
                return RedirectToAction(nameof(Subdivision));
            }
            ViewBag.Philias = _context.Philia.ToList();
            return View(model);
        }

        // Удаление подразделения
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSubdivision(int id)
        {
            var subdivision = _context.Subdivisions.Find(id);
            if (subdivision == null)
            {
                return Json(new { success = false, message = "Підрозділ не знайдено" });
            }

            try
            {
                _context.Subdivisions.Remove(subdivision);
                _context.SaveChanges();
                return Json(new
                {
                    success = true,
                    message = "Підрозділ успішно видалено"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Помилка при видаленні: {ex.Message}"
                });
            }
        }


        // Промышленные площадки
        public IActionResult Area(int? subdivisionId, string sortOrder = "", string searchString = "")
        {
            // Устанавливаем параметры сортировки
            ViewBag.NameSortParam = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.SubdivisionSortParam = sortOrder == "subdivision_asc" ? "subdivision_desc" : "subdivision_asc";
            ViewBag.PhiliaSortParam = sortOrder == "philia_asc" ? "philia_desc" : "philia_asc";
            ViewBag.SapIdSortParam = sortOrder == "sapid_asc" ? "sapid_desc" : "sapid_asc";

            var areas = _context.Areas
                .Include(a => a.Subdivision)
                .ThenInclude(s => s.Philia)
                .AsQueryable();

            // Фильтрация
            if (subdivisionId.HasValue)
            {
                areas = areas.Where(a => a.IdSubdivision == subdivisionId.Value);
                ViewBag.FilteredSubdivision = _context.Subdivisions
                    .Include(s => s.Philia)
                    .FirstOrDefault(s => s.IdSubdivision == subdivisionId.Value);
            }

            // Поиск
            if (!string.IsNullOrEmpty(searchString))
            {
                areas = areas.Where(a => a.NameArea.Contains(searchString) ||
                                       a.IdAreaSAP.Contains(searchString));
            }

            // Сортировка
            areas = sortOrder switch
            {
                "name_asc" => areas.OrderBy(a => a.NameArea),
                "name_desc" => areas.OrderByDescending(a => a.NameArea),
                "subdivision_asc" => areas.OrderBy(a => a.Subdivision.NameSubdivision),
                "subdivision_desc" => areas.OrderByDescending(a => a.Subdivision.NameSubdivision),
                "philia_asc" => areas.OrderBy(a => a.Subdivision.Philia.NamePhilia),
                "philia_desc" => areas.OrderByDescending(a => a.Subdivision.Philia.NamePhilia),
                "sapid_asc" => areas.OrderBy(a => a.IdAreaSAP),
                "sapid_desc" => areas.OrderByDescending(a => a.IdAreaSAP),
                _ => areas.OrderBy(a => a.NameArea) // Сортировка по умолчанию
            };

            return View(areas.ToList());
        }

        [HttpGet]
        public IActionResult CreateArea()
        {
            //ViewBag.Subdivisions = _context.Subdivisions
            //    .Include(s => s.Philia)
            //    .OrderBy(s => s.Philia.NamePhilia)
            //    .ThenBy(s => s.NameSubdivision)
            //    .ToList();
            //return View();
            var model = new Area(); // Создаём новую модель
            ViewBag.Subdivisions = _context.Subdivisions.ToList(); // Загружаем данные для выпадающего списка
            return View(model); // Передаём модель в представление
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateArea(Area model)
        {
            if (ModelState.IsValid)
            {
                _context.Areas.Add(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Промисловий майданчик успішно додано!";
                return RedirectToAction(nameof(Area));
            }
            ViewBag.Subdivisions = _context.Subdivisions
                .Include(s => s.Philia)
                .ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult EditArea(int id)
        {
            var model = _context.Areas.Find(id);
            if (model == null) return NotFound();

            ViewBag.Subdivisions = _context.Subdivisions
                .Include(s => s.Philia)
                .ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditArea(Area model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Зміни успішно збережено!";
                return RedirectToAction(nameof(Area));
            }
            ViewBag.Subdivisions = _context.Subdivisions
                .Include(s => s.Philia)
                .ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteArea(int id)
        {
            var area = _context.Areas.Find(id);
            if (area == null)
            {
                return Json(new { success = false, message = "Майданчик не знайдено" });
            }

            try
            {
                _context.Areas.Remove(area);
                _context.SaveChanges();
                return Json(new { success = true, message = "Майданчик успішно видалено" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Помилка: {ex.Message}" });
            }
        }
    }
}
