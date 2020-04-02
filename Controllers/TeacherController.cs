using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolRegister.BLL.Entities;
using SchoolRegister.Services.Interfaces;
using SchoolRegisterSystem.Services.Interfaces;
using SchoolRegisterSystem.ViewModel.DTos;

namespace SchoolRegisterSystem.Controllers
{

    public class TeacherController : Controller
    {

        private readonly IStudentService _studentService;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;
        private readonly ITeacherService _teacherService;
        private readonly UserManager<User> _userManager;
        public TeacherController(IStudentService studentService, ISubjectService subjectService, UserManager<User> userManager, IGradeService gradeService, ITeacherService teacherService)
        {
            _studentService = studentService;
            _subjectService = subjectService;
            _userManager = userManager;
            _gradeService = gradeService;
            _teacherService = teacherService;
        }



        public IActionResult Index()
        {


            return View();
        }
       // [Authorize(Roles = "Teacher")]
        [AllowAnonymous]
        public IActionResult AddGrade(int ?studentId = null)
        {
            var teacher = _userManager.GetUserAsync(User).Result;
            var students = _studentService.GetStudents(s => s.Group.SubjectGroups.Any(sq => sq.Subject.Teacher.Id == teacher.Id));
            var subjects = _subjectService.GetSubjects(t => t.TeacherId == teacher.Id);
            ViewBag.StudentList = new SelectList(students.Select(t => new
            {
                Text = $"{t.FirstName} {t.LastName}",
                Value = t.Id,
                Selected = t.Id 
            }), "Value", "Text");

            ViewBag.SubjectList = new SelectList(subjects.Select(s => new
            {
                Text = s.Name,
                Value = s.Id
            }), "Value", "Text");
            ViewBag.GradeScale = new SelectList(Enum.GetValues(typeof(GradeScale)).Cast<GradeScale>().Select(x => new
            {
                Text = x.ToString(),
                Value = ((int)x).ToString()
            }), "Value", "Text");

            return View();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Teacher")]
        public IActionResult AddGrade(AddGradeToStudentDto addGradeToStudentDto)
        {
            if (ModelState.IsValid)
            {
                var teacher = _userManager.GetUserAsync(User).Result;
                addGradeToStudentDto.TeacherId = teacher.Id;
                _gradeService.AddGradeToStudent(addGradeToStudentDto);
                return RedirectToAction("Details", "Student", new { studentId = addGradeToStudentDto.StudentId });
            }
            return View();
        }


        public IActionResult SendEmailToParent(int studentId)
        {
            return View(new SendEmailToParent() { StudentId = studentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendEmailToParent(SendEmailToParent sendEmailToParentDto)
        {
            var teacher = _userManager.GetUserAsync(User).Result;
            sendEmailToParentDto.SenderId = teacher.Id;
            if (_teacherService.SendEmailToParent(sendEmailToParentDto))
            {
                return RedirectToAction("Index", "Student");
            }
            return View("Error");
        }
    }
}