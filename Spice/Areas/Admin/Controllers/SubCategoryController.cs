using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
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
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);

        }

        //POST create method
        //public async Task<IActionResult> Create(SubCategory subCategory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.SubCategory.Add(subCategory);
        //        await _db.SaveChangesAsync();
        //    }

        //    return View(subCategory);
        //}

    }
}