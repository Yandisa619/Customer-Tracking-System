using CustomerTrackingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerTrackingSystem.Controllers
{

    public class CustomersController : Controller
    {
        // Database context for accessing customer data
        private readonly CustomerContext _context;

        // Constructor that receives the database context (dependency injection)
        public CustomersController(CustomerContext context)
        {
            _context = context;
        }

        // GET method to display a list of customers with filtering, sorting, and pagination
        public async Task<IActionResult> Index(string nameFilter, string vatFilter, string sortBy, string sortOrder, int page = 1)
        {
            // Store filter and sort values in ViewBag to preserve them in the view
            ViewBag.NameFilter = nameFilter;
            ViewBag.VATFilter = vatFilter;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentPage = page;

            // Start with all customers from the database as a queryable collection
            var customers = _context.Customers.AsQueryable();

            // Apply name filter if provided (search for names containing the filter text)
            if (!string.IsNullOrEmpty(nameFilter))
            {
                customers = customers.Where(c => c.Name.Contains(nameFilter));
            }

            // Apply VAT filter if provided (search for VAT numbers containing the filter text)
            if (!string.IsNullOrEmpty(vatFilter))
            {
                customers = customers.Where(c => c.VATNumber.Contains(vatFilter));
            }

            // Apply sorting based on the specified column and order
            switch (sortBy)
            {
                case "Name":
                    // Sort by name in ascending or descending order
                    customers = sortOrder == "desc" ? customers.OrderByDescending(c => c.Name) : customers.OrderBy(c => c.Name);
                    break;
                case "VATNumber":
                    // Sort by VAT number in ascending or descending order
                    customers = sortOrder == "desc" ? customers.OrderByDescending(c => c.VATNumber) : customers.OrderBy(c => c.VATNumber);
                    break;
                default:
                    // Default sorting by name in ascending order
                    customers = customers.OrderBy(c => c.Name);
                    break;
            }

            // Pagination - 10 records per page
            int pageSize = 10;
            int totalCount = await customers.CountAsync(); // Get total number of filtered records
            int totalPages = (totalCount + pageSize - 1) / pageSize; // Calculate total pages needed

            ViewBag.TotalPages = totalPages; // Store total pages for the view

            // Get only the records for the current page
            var customerList = await customers
                .Skip((page - 1) * pageSize) // Skip records from previous pages
                .Take(pageSize) // Take only records for this page
                .ToListAsync(); // Execute the query and get results as a list

            // Return the view with the filtered, sorted, and paginated customer list
            return View(customerList);
        }

        // GET: Display the form to create a new customer
        public IActionResult Create()
        {
            return View(); // Returns an empty form
        }

        // POST: Handle form submission to create a new customer
        [HttpPost] // This method only responds to HTTP POST requests
        [ValidateAntiForgeryToken] // Security measure to prevent cross-site request forgery
        public async Task<IActionResult> Create([Bind("Name,Address,TelephoneNumber,ContactPersonName,ContactPersonEmail,VATNumber")] Customer customer)
        {
            // Check if the submitted data passes validation rules
            if (ModelState.IsValid)
            {
                _context.Add(customer); // Add the new customer to the database context
                await _context.SaveChangesAsync(); // Save changes to the database
                return RedirectToAction(nameof(Index)); // Redirect to the customer list
            }
            // If validation fails, return to the form with error messages
            return View(customer);
        }

        // GET: Display the form to edit an existing customer
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Return 404 if no ID provided
            }

            // Find the customer by ID in the database
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(); // Return 404 if customer not found
            }
            return View(customer); // Return the edit form with customer data
        }

        // POST: Handle form submission to update an existing customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Name,Address,TelephoneNumber,ContactPersonName,ContactPersonEmail,VATNumber")] Customer customer)
        {
            // Verify that the ID in the URL matches the customer ID
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer); // Mark the customer as modified
                    await _context.SaveChangesAsync(); // Save changes to database
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency conflicts (if someone else modified the same record)
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound(); // Customer was deleted by someone else
                    }
                    else
                    {
                        throw; 
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to customer list
            }
            // If validation fails, return to edit form
            return View(customer);
        }

        // GET: Display confirmation page before deleting a customer
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the customer to delete
            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer); // Show delete confirmation page
        }

        // POST: Actually delete the customer 
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the customer by ID
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer); // Mark for deletion
            await _context.SaveChangesAsync(); // Execute delete in database
            return RedirectToAction(nameof(Index)); // Redirect to customer list
        }

        // Helper method to check if a customer exists
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }

}