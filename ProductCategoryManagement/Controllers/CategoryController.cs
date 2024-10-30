using Microsoft.AspNetCore.Mvc;
using ProductCategoryManagement.Models;


namespace ProductCategoryManagement.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext db;

        public CategoryController(AppDbContext context)
        {
            db = context;
        }
        public IActionResult Index(string Msg)
        {
            ViewBag.Msg = Msg;
            var Categories = db.Categories.ToList();
            return View(Categories);
        }


        //create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var duplicate = db.Categories.FirstOrDefault(c => c.CategoryName == category.CategoryName && c.CategoryId != category.CategoryId);

                    if (duplicate != null)
                    {
                        ModelState.AddModelError("CategoryName", "A category with this name already exists.");
                        return View(category);
                    }
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index", new {Msg = "Data Saved"});
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the category.");
            }
            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(int id)
        {
            try
            {
                var category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

                if (category != null)
                {
                    return View("EditForm", category);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while editing the category.", ex);
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult SaveEdit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isDuplicate = db.Categories.Any(c => c.CategoryName == category.CategoryName && c.CategoryId != category.CategoryId);

                    if (isDuplicate)
                    {
                        ModelState.AddModelError("CategoryName", "A category with this name already exists.");
                        return View("EditForm", category);
                    }
                    db.Categories.Update(category);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Edited" });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the category.");
            }
            return View("EditForm", category);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

                if (category != null)
                {
                    var products = db.Products.Where(p => p.CategoryId == id).ToList();

                    if (products.Any())
                    {
                        db.Products.RemoveRange(products);
                    }

                    db.Categories.Remove(category);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Deleted" });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the category.", ex);
            }
            return NotFound();
        }



        /* [HttpPost]
         public IActionResult DeleteConfirmed(Category cat)
         {
             var category = db.Categories.FirstOrDefault(c => c.CategoryId == cat.CategoryId);

             if (category != null)
             {
                 db.Categories.Remove(category);
                 db.SaveChanges();
             }
             return RedirectToAction("Index");
         }*/
    }
}
