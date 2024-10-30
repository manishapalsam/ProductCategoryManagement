using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategoryManagement.Models;

namespace ProductCategoryManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext db;

        public ProductController(AppDbContext context)
        {
            db = context;
        }

        // View Product List
        public async Task<IActionResult> ViewList(int page = 1, int pageSize = 10)
        {
            try
            {
                int skip = (page - 1) * pageSize;
                var products = await db.Products
                    .Include(p => p.Category)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                int totalProducts = await db.Products.CountAsync();
                int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

                var viewModel = new ProductViewModel
                {
                    Products = products,
                    CurrentPage = page,
                    TotalPages = totalPages,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the product list.", ex);
            }
        }

        // Index
        public IActionResult Index(string Msg)
        {
            try
            {
                ViewBag.Msg = Msg;
                var productList = db.Products.Include(p => p.Category).ToList();
                return View(productList);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products for the Index page.", ex);
            }
        }

        // Create Product (GET)
        public IActionResult Create()
        {
            try
            {
                ViewBag.Categories = db.Categories.ToList();
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading categories for the Create page.", ex);
            }
        }

        // Create Product (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isDuplicate = db.Products.Any(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId && p.ProductName == product.ProductName);

                    if (isDuplicate)
                    {
                        ModelState.AddModelError("ProductName", "A product with the same name already exists in this category.");
                        ViewBag.Categories = db.Categories.ToList();
                        return View(product);
                    }

                    db.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Created" });
                }

                ViewBag.Categories = db.Categories.ToList();
                return View(product);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating a new product.", ex);
            }
        }

        // Edit (GET)
        [HttpPost]
        public IActionResult Edit(int id)
        {
            try
            {
                ViewBag.Categories = db.Categories.ToList();
                var product = db.Products.FirstOrDefault(p => p.ProductId == id);
                return View("EditForm", product);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading the Edit form.", ex);
            }
        }

        // Save Form (POST)
        [HttpPost]
        public IActionResult SaveForm(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isDuplicate = db.Products.Any(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId && p.ProductName == product.ProductName);

                    if (isDuplicate)
                    {
                        ModelState.AddModelError("ProductName", "A product with the same name already exists in this category.");
                        ViewBag.Categories = db.Categories.ToList();
                        return View(product);
                    }

                    db.Products.Update(product);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Edited" });
                }

                ViewBag.Categories = db.Categories.ToList();
                return View(product);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the product details.", ex);
            }
        }

        // Delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var product = db.Products.FirstOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Deleted" });
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the product.", ex);
            }
        }
    }
}
