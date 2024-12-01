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
            try {
                ViewBag.Msg = Msg;
                var Categories = db.Categories.ToList();

                if (Categories.Count == 0) {
                    ViewBag.Msg = "NO RECORDS FOUND";
                    return View(Categories);
                }
                return View(Categories);
            }
            catch (Exception e)
            {
                throw new Exception("An Error occured while fetching Categories Data", e);
            }

        }


        //create
        [HttpGet]
        public ActionResult Create()
        {
            try {
                Category category = new Category();
                ViewBag.Create = "Create";
                return View(category);
            }
            catch (Exception e) {
                throw new Exception("An Error occured while clicking Create Button",e);
            }
           
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
                        ViewBag.Create = "Create";
                        ModelState.AddModelError("CategoryName", "A category with this name already exists.");
                        return View(category);
                    }
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index", new {Msg = "Data Saved Successfully"});
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the category.");
            }
            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(int? id)
        {
            try
            {
                var category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

                if (category != null)
                {
                    return View("Create", category);
                }
                return RedirectToAction("Index", new { Msg = "Category trying to edit does not exist" });
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while editing the category.", ex);
            }
        }

        [HttpPost]
        public ActionResult SaveEdit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var c = db.Categories.Where(c => c.CategoryId  == category.CategoryId);
                    if (c == null) {
                        ViewBag.Msg = "Category to be edited  does not exist";
                        return View("Create",category);
                    }

                    bool isDuplicate = db.Categories.Any(c => c.CategoryName == category.CategoryName && c.CategoryId != category.CategoryId);

                    if (isDuplicate)
                    {
                        ModelState.AddModelError("CategoryName", "A category with this name already exists.");
                        return View("Create", category);
                    }
                    db.Categories.Update(category);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Edited Successfully" });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the category.");
            }
            return View("EditForm", category);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            try
            {
                var category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

                if (category != null)
                {
                    //var products = db.Products.Where(p => p.CategoryId == id).ToList();
                    var products = db.Products.Any(p => p.CategoryId == id);


                    if (products)
                    {
                        return RedirectToAction("Index",new { Msg = "First delete the product for particular category from product list"});
                        //db.Products.RemoveRange(products);
                    }

                    db.Categories.Remove(category);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Deleted Successfully" });
                }

                return RedirectToAction("Index", new { Msg = "Category to be deleted does not exists"});
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the category.", ex);
            }
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
