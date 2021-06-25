using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HHRROrganizer.Data;
using HHRROrganizer.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace HHRROrganizer.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public object Environment { get; private set; }
        private IWebHostEnvironment _environment;


        public EmployeesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.Department);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employees == null)
            {
                return NotFound();
            }

            return View(employees);
        }
        // GET: Employees/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Set<Department>(), "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Picture,StartDate,GrossSalary,NetSalary,DepartmentId")] Employees employees, List<IFormFile> postedFiles, string boolPicture, string randomPicture)
        {
            //Due to the random user generator, we must generate a bool that tells us if we enter the user randomly (by pressing the Fill Random User button), or manually (using the button to search for a local image).
           
            if (ModelState.IsValid)
            {
                var exist = await _context.Employees.Where(e => e.Name == employees.Name && e.Surname == employees.Surname).FirstOrDefaultAsync();
                if (exist == null)
                {
                    
                    if (boolPicture == "True")
                    {
                        employees.Picture = randomPicture;
                        _context.Add(employees);
                    }
                    else if (boolPicture == "False")
                    {
                        _context.Add(employees);
                        UploadProfilePic(postedFiles, employees);
                    }
                   
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["msg"] = "Error!";
                }

            }
            ViewData["DepartmentId"] = new SelectList(_context.Set<Department>(), "Id", "Name", employees.DepartmentId);
            return View(employees);

        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Set<Department>(), "Id", "Id", employees.DepartmentId);
            return View(employees);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Picture,StartDate,GrossSalary,NetSalary,DepartmentId")] Employees employees, List<IFormFile> postedFiles)
        {
            if (id != employees.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    UploadProfilePic(postedFiles, employees);
                    _context.Update(employees);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeesExists(employees.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Set<Department>(), "Id", "Name", employees.DepartmentId);
            return View(employees);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employees == null)
            {
                return NotFound();
            }

            return View(employees);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employees = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeesExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        //Method to upload a profile picture for Create view of Employers
        [HttpPost]
        public void UploadProfilePic(List<IFormFile> postedFiles, Employees employees)
        {

            string wwwPath = this._environment.WebRootPath;

            string path = Path.Combine(wwwPath, "pictures");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                employees.Picture = fileName;
                //FileMode formatea el espacio de memoria, FileStream abre el espacio de memoria
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                }
            }


        }

        //Employee finder, by name, salary or department
        [HttpPost]
        public async Task<IActionResult> Index(string nameSearch, string departmentSearch)
        {

            //If the user searches by department, leaving the name box empty:
            if (!String.IsNullOrEmpty(departmentSearch) && String.IsNullOrEmpty(nameSearch))
            {
                //var applicationDbContext = _context.Employees.Where(e => e.Department >= departmentSearch);
                var applicationDbContext = _context.Employees.Include(e => e.Department).Where(e => e.Department.Name.Contains(departmentSearch));
                //var applicationDbContext = _context.Employees.Where(e => e.Department >= departmentSearch);

                return View(await applicationDbContext.ToListAsync());
            }
            //If the user searches by name, leaving the department box empty
            else if (!String.IsNullOrEmpty(nameSearch) && String.IsNullOrEmpty(departmentSearch))
            {
                var applicationDbContext = _context.Employees.Where(e => e.Name.Contains(nameSearch));
                return View(await applicationDbContext.ToListAsync());
            }
            // If none of the fields are empty when the search engine is activated
            else if (!String.IsNullOrEmpty(nameSearch) && !String.IsNullOrEmpty(departmentSearch))
            {
                var applicationDbContext = _context.Employees.Include(e => e.Department).Where(e => e.Name.Contains(nameSearch) && e.Department.Name.Contains(departmentSearch));
                return View(await applicationDbContext.ToListAsync());
            }
            //If both of the fields are empty when the search engine is activated:
            else
            {
                var applicationDbContext = _context.Employees.Include(e => e.Department);
                return View(await applicationDbContext.ToListAsync());
            }

        }
        [HttpPost]
        public async Task<IActionResult> ModifySalary(List<int> searchList, int userPercentage)
        {
            foreach (var id in searchList)
            {
                Employees employee = await _context.Employees.Where(e => e.Id == id).FirstOrDefaultAsync();
                employee.NetSalary = employee.NetSalary + Convert.ToInt32(employee.NetSalary * (Convert.ToDouble(userPercentage) / Convert.ToDouble(100)));
                employee.GrossSalary = employee.GrossSalary + Convert.ToInt32(employee.GrossSalary * (Convert.ToDouble(userPercentage) / Convert.ToDouble(100)));
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
