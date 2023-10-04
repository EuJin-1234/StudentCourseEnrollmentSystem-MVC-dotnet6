using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Student_Course_Enrollment_System.Data;
using Student_Course_Enrollment_System.Models;
using Student_Course_Enrollment_System.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;

namespace Student_Course_Enrollment_System.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Student,SuperAdmin")]
        //GET: Enrollments
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Student"))
            {
                var studentEnrollments = _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Student.UserId == userId)
                    .ToList();

                return View(studentEnrollments);
            }
            else if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                var allEnrollments = _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .ToList();

                return View(allEnrollments);
            }

            // Handle other roles or unauthorized access
            return View("Error");
        }


        [Authorize(Roles = "Admin,Student,SuperAdmin")]
        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "courseId", "courseId");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID");
            return View();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        // POST: Enrollments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,StudentID,CourseID,IsEnrolled")] EnrollmentRequestDTO enrollmentDTO)
        {
            if (ModelState.IsValid)
            {
                var enrollment = new Enrollment
                {
                    StudentID = enrollmentDTO.StudentID,
                    CourseID = enrollmentDTO.CourseID,
                    IsEnrolled = enrollmentDTO.IsEnrolled
                };

                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "courseId", "courseId", enrollmentDTO.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollmentDTO.StudentID);
            return View(enrollmentDTO);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "courseId", "courseId", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        // POST: Enrollments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentID,StudentID,CourseID,IsEnrolled")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();

                    // Update the isEnrolled property of the associated Student record
                    var student = await _context.Students.FindAsync(enrollment.StudentID);
                    student.IsEnrolled = enrollment.IsEnrolled;
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
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
            ViewData["CourseID"] = new SelectList(_context.Courses, "courseId", "courseId", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enrollments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Enrollments'  is null.");
            }
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return (_context.Enrollments?.Any(e => e.EnrollmentID == id)).GetValueOrDefault();
        }



        // POST: Enrollments/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (enrollment.IsEnrolled || enrollment.IsRejected)
            {
                return RedirectToAction("Index"); // No status change if already enrolled or rejected
            }

            enrollment.IsEnrolled = true;
            enrollment.IsRejected = false;
            _context.Enrollments.Update(enrollment);

            // Update the corresponding Student record
            var student = await _context.Students.FindAsync(enrollment.StudentID);
            if (student != null)
            {
                student.IsEnrolled = true;
                _context.Students.Update(student);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: Enrollments/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (enrollment.IsEnrolled || enrollment.IsRejected)
            {
                return RedirectToAction("Index"); // No status change if already enrolled or rejected
            }

            enrollment.IsEnrolled = false;
            enrollment.IsRejected = true;
            _context.Enrollments.Update(enrollment);

            // Update the corresponding Student record
            var student = await _context.Students.FindAsync(enrollment.StudentID);
            if (student != null)
            {
                student.IsEnrolled = false;
                _context.Students.Update(student);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Enrollments/Withdraw/5
        public async Task<IActionResult> Withdraw(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Withdraw/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Student"))
            {
                return Forbid();
            }

            if (!enrollment.IsEnrolled)
            {
                return RedirectToAction("Index"); // No status change if not enrolled
            }

            enrollment.IsEnrolled = false;
            enrollment.IsWithdrawn = true;
            _context.Enrollments.Update(enrollment);

            // Update the corresponding Student record
            var student = await _context.Students.FindAsync(enrollment.StudentID);
            if (student != null)
            {
                student.IsEnrolled = false;
                _context.Students.Update(student);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}