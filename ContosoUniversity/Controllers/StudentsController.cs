﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    //this controller takes SchoolContext as a constructor parameter
    //ASP.NET Core dependency injection takes care of passing an instance of SchoolContext into the controller
    //This was configured in the Startup.cs file
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        //get a list of students from the Students entity set by reading the Students property of the DB context instance
        //when the user clicks a column heading hyperlink, the appropriate sortOrder value is provided in the query string
        //if user hasn't clicked a paging or sorting link, all parameters will be null
        //--if a paging link is clicked, the page variable will contain the page number to display
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            //ViewData elements are used by the view to configure the column heading hyperlinks with the appropriate query string values
            
            //provides view with the current sort order
            //--this must be included in the paging links in order to keep the sort order the same while paging
            ViewData["CurrentSort"] = sortOrder;

            //specifies that if the sortOrder parameter is null or empty, NameSortParm should be set to name_desc
            //--otherwise it should be set to an empty string
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            //if the search string is changed during paging, the page has to be reset to 1
            //--because the new filter can result in different data to display
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            //provides view with the current filter string
            //this value must be included in the paging links in order to maintain the filter settings during paging
            //must also be restored to the text box when the page is redisplayed
            ViewData["CurrentFilter"] = searchString;

            var students = from s in _context.Students
                           select s;
            //execute only if there is a value to search for
            if (!String.IsNullOrEmpty(searchString))
            {
                //where clause selects only students whose first or last name contains the search string
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }
            //default is to display students in ascending order by last name
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            //convert the student query to a single page of students in a collection type that supports paging
            //that single page of students is then passed to the view
            //?? represents the null-coalescing operator, which defines a default value for a nullable type
                //return the value of pageNumber if it has a value, or return 1 if pageNumber is null
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                 return NotFound();
            }

            var student = await _context.Students
                //Include and ThenInclude methods cause the context to load the Student.Enrollments nav property
                    //context also loads Enrollment.Course nav property within each enrollment
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                //AsNoTracking method improves performance in scenarios where the entities returned won't be updated in the current context's lifetime
                .AsNoTracking()
                //SingleOrDefaultAsync method retrieves a single Student entity
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            //Bind attribute helps protect against overposting by limiting the fields that the model binder uses when it creates a Student instance
            //don't need to bind ID because SQL server will automatically set it when the row is inserted
            [Bind("EnrollmentDate,FirstMidName,LastName")] Student student)
        {
            try
            {
                //add Student entity created by the ASP.NET Core MVC model binder to the Students entity set
                //save changes to database
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        //named method EditPost because the method signature of the HttpPost Edit method is the same as the HttpGet Edit method
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
            //TryUpdateModel updates fields in the retrieved entity based on user input in the posted form data
            if (await TryUpdateModelAsync<Student>(
                //parameters here are the fields that have been whitelisted to be updateable
                studentToUpdate,
                //empty string is for a prefix to use within the form fields names
                "",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {
                try
                {
                    //when SaveChanges is called, the EF creates SQL statements to update the DB row
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Students/Delete/5
        // the method that's called in response to a GET request displays a view that gives the user a chance to approve or cancel the delete
        //if approved, POST method is created
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        //if error occurs, HttpPost Delete method calls the HttpGet Delete method
        //--passes a parameter that indicates that an error has occurred
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //perform delete operation and catch any DB update errors
        //retrieve selected entity, then call the Remove method to set entity's status to Delete
        //when SaveChanges is called, a SQL DELETE command is generated
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}
