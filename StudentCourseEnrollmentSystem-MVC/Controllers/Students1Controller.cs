using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Student_Course_Enrollment_System.Data;
using Student_Course_Enrollment_System.DTOs;
using Student_Course_Enrollment_System.Models;

namespace Student_Course_Enrollment_System.Controllers
{
    public class Students1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Students1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Students1
        [Authorize(Roles = "Admin,Student,SuperAdmin")]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Student"))
            {
                var student = _context.Students
                    .Include(s => s.Enrollments)
                        .ThenInclude(e => e.Course)
                    .Where(s => s.UserId == userId)
                    .ToList();

                return View(student);
            }
            else if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                var students = _context.Students
                    .Include(s => s.Enrollments)
                        .ThenInclude(e => e.Course)
                    .ToList();

                return View(students);
            }

            // Handle other roles or unauthorized access
            return View("Error");
        }


        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students1/Create
        [Authorize(Roles = "Admin,Student, SuperAdmin")]
        public IActionResult Create()
        {
            var viewModel = new CreateStudentViewModel
            {
                Courses = _context.Courses.ToList()
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Admin,Student, SuperAdmin")]
        // POST: Students1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Get the current logged-in user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var student = new Student_Course_Enrollment_System.Models.Student
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    UserId = userId
                };

                _context.Add(student);
                await _context.SaveChangesAsync();

                // Create enrollments for the selected courses
                foreach (var courseId in viewModel.SelectedCourseIds)
                {
                    var enrollment = new Enrollment
                    {
                        StudentID = student.ID,
                        CourseID = courseId
                    };

                    _context.Enrollments.Add(enrollment);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            viewModel.Courses = _context.Courses.ToList();
            return View(viewModel);
        }


        // GET: Students1/Edit/5
        [Authorize(Roles = "Admin,Student, SuperAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
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

        [Authorize(Roles = "Admin,Student, SuperAdmin")]
        // POST: Students1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,IsEnrolled")] Student_Course_Enrollment_System.Models.Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
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
            return View(student);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        // GET: Students1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        // POST: Students1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return (_context.Students?.Any(e => e.ID == id)).GetValueOrDefault();
        }



        // GET: Students1/ChooseCourse
        [Authorize(Roles = "Student")]
        public IActionResult ChooseCourse()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }


    }
}
