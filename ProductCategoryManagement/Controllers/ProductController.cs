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
                if (productList.Count == 0) {
                    ViewBag.Msg = "NO RECORDS FOUND";
                    return View(productList);
                }
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
                Product product = new Product();
                ViewBag.Categories = db.Categories.ToList();
                ViewBag.Create = "Create";

                return View(product);
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
                        ViewBag.Create = "Create";
                        ModelState.AddModelError("ProductName", "A product with the same name already exists in this category.");
                        ViewBag.Categories = db.Categories.ToList();
                        return View(product);
                    }

                    db.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Created Successfully" });
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
        public IActionResult Edit(int? id)
        {
            try
            {
                ViewBag.Categories = db.Categories.ToList();
                var product = db.Products.FirstOrDefault(p => p.ProductId == id);
                if (product == null) {
                    return RedirectToAction("Index",new { Msg = "Product Trying to edit does not exists"});
                }
                return View("Create", product);
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
                    // Retrieve the existing product from the database
                    var existingProduct = db.Products.FirstOrDefault(p => p.ProductId == product.ProductId);

                    if (existingProduct == null)
                    {
                        ViewBag.Msg = "Product trying to edit does not exist";
                        ViewBag.Categories = db.Categories.ToList();
                        return View("Create", product);
                    }

                    // Check for duplicate product name within the same category
                    bool isDuplicate = db.Products.Any(p =>
                        p.CategoryId == product.CategoryId &&
                        p.ProductId != product.ProductId &&
                        p.ProductName == product.ProductName);

                    if (isDuplicate)
                    {
                        ModelState.AddModelError("ProductName", "A product with the same name already exists in this category.");
                        ViewBag.Categories = db.Categories.ToList();
                        return View("Create", product);
                    }

                    // Update fields of the existing entity
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.CategoryId = product.CategoryId;
                    // Add other fields as necessary...

                    // Save changes
                    db.SaveChanges();

                    return RedirectToAction("Index", new { Msg = "Data Edited Successfully" });
                }

                ViewBag.Categories = db.Categories.ToList();
                return View("Create", product);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the product details.", ex);
            }
        }


        // Delete
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            try
            {
                var product = db.Products.FirstOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Msg = "Data Deleted Successfully" });
                }

                return RedirectToAction("Index", new { Msg = "Product trying to delete does not exists"});
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the product.", ex);
            }
        }
    }
}
