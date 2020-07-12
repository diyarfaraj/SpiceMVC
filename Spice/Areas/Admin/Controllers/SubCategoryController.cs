using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]

    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        //GET INDEX
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(c => c.Category).ToListAsync();
            return View(subCategories);
        }

        //GET - create method
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name)
                                  .Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);

        }

        // POST - create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _db.SubCategory.Include(s => s.Category)
                    .Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if(doesSubCategoryExist.Count() > 0)
                {
                    //Error message here
                    StatusMessage = "Error: Subcategory already exists under " + doesSubCategoryExist.First().Category.Name +" Category. Use another one.  ";
                } else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
            } else
            {
                return View(model);
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);

        }


        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        //GET edit 
        
        public async Task<IActionResult> Edit(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if(subCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name)
                                  .Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);

        }

        // POST - edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _db.SubCategory.Include(s => s.Category)
                    .Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExist.Count() > 0)
                {
                    //Error message here
                    StatusMessage = "Error: Subcategory already exists under " + doesSubCategoryExist.First().Category.Name + " Category. Use another one.  ";
                }
                else
                {
                    var subCategoryfromDb = await _db.SubCategory.FindAsync(id);
                    subCategoryfromDb.Name = model.SubCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            modelVM.SubCategory.Id = id;
            return View(modelVM);

        }

        // GET details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name)
                                  .Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //GET delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }


            return View(subCategory);
        }

        //POST delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubCat(int id)
        {
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if(subCategory == null)
            {
                return NotFound();
            }

            try
            {
                _db.SubCategory.Remove(subCategory);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

               
            }



           

            return RedirectToAction(nameof(Index));
        }


    }
}