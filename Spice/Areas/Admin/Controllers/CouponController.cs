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
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;

        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        //GET create
        public  IActionResult Create()
        {
            return View();
        }

        //POST create method
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                // if there is pictures uploaded
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new System.IO.MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    coupons.Picture = p1;
                }

               
                //save the usual inputs 
                _db.Coupon.Add(coupons);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            return View(coupons);
        }

        //GET edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var coupong = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if(coupong == null)
            {
                return NotFound();
            }

            return View(coupong);
        }

        // POST edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupons)
        {
            if(coupons.Id == 0)
            {
                return NotFound(); 
            }

            if (ModelState.IsValid)
            {
                var coupongFromDB = await _db.Coupon.Where(m => m.Id == coupons.Id).FirstOrDefaultAsync();

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new System.IO.MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    coupongFromDB.Picture = p1;
                }

                coupongFromDB.Name = coupons.Name;
                coupongFromDB.MinimumAmount = coupons.MinimumAmount;
                coupongFromDB.Discount = coupons.Discount;
                coupongFromDB.IsActive = coupons.IsActive;
                coupongFromDB.CouponType = coupons.CouponType;

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(coupons);
        }

        // GET details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupong = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if (coupong == null)
            {
                return NotFound();
            }

            return View(coupong);
        }

        //GET delete
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var coupong = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if (coupong == null)
            {
                return NotFound();
            }

            return View(coupong);
        }

        //POST delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int id)
        {
            var coupongFromDB = await _db.Coupon.FirstOrDefaultAsync(m => m.Id == id);
            _db.Coupon.Remove(coupongFromDB);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}