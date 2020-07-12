using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public Exception eMessage = new Exception("hejhej");
        public CategoryController(ApplicationDbContext db)
        {
            _db = db; 
        }

        //GET Method
        public async Task<IActionResult> Index()
        {
            return View( await _db.Category.ToListAsync());
        }

        //GET - Create method 
        public IActionResult Create()
        {
            return View();
        }

        //POST - Create mehtod  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //if valid
                _db.Category.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //GET - Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var category = await _db.Category.FindAsync(id);
            
            if(category == null)
            {
                return NotFound();
            }

            return View(category);


        }

        //POST -edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
           
        }


        //GET - delete
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catergory = await _db.Category.FindAsync(id);

            if (catergory == null)
            {
                return NotFound();
            }

            return View(catergory);

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            
            var category = await _db.Category.FindAsync(id);

            if(category == null)
            {
               return NotFound();
            }

            try
            {
                _db.Category.Remove(category);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }



            return RedirectToAction(nameof(Index));

        }

        //GET details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); 
            }

            var category = await _db.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);


        }


    }
}