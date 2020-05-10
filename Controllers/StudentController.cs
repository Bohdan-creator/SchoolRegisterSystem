using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolRegister.BLL.Entities;
using SchoolRegisterSystem.DAL.EF;
using SchoolRegisterSystem.Extensions;
using SchoolRegisterSystem.Services.Interfaces;
using SchoolRegisterSystem.ViewModel;
using SchoolRegisterSystem.ViewModel.DTos;
using SchoolRegisterSystem.ViewModel.VMs;

namespace SchoolRegisterSystem.Controllers
{
    public class StudentController : Controller
    {
        public IGradeService gradeService;

        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly UserManager<User> _userManager;
        private readonly ITeacherService _teacherService;
        private readonly IGradeService _gradeService;
        private readonly ApplicationDbContext _db;
        public StudentController(ApplicationDbContext db,IStudentService studentService, UserManager<User> userManager, IGroupService groupService, ITeacherService teacherService, IGradeService gradeService)
        {
            _db = db;
            _studentService = studentService;
            _groupService = groupService;
            _userManager = userManager;
            _teacherService = teacherService;
            _gradeService = gradeService;
        }
        [Authorize(Roles="Teacher,Admin,Parent")]
        public IActionResult Index(string filterValue=null)
        {
            bool isAjax = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";
            Expression<Func<Student, bool>> filterPredicate = null;

            if (!string.IsNullOrWhiteSpace(filterValue))
            {
                filterPredicate = x => x.FirstName.Contains(filterValue);
            }


            var user = _userManager.GetUserAsync(User).Result;
            IEnumerable<StudentVm> studentsVm = null;
            if (_userManager.IsInRoleAsync(user, "Parent").Result)
            {
                var parent = _userManager.GetUserAsync(User).Result;
                studentsVm = _studentService.GetStudents(x => x.ParentId == parent.Id);
                if (isAjax)
                {
                   var studen  = _db.Users.OfType<Student>().Where(x=>x.ParentId == parent.Id);
                    

                    studentsVm = _studentService.GetStudents(filterPredicate,studen);
                    return PartialView("_StudentsTableDataPartial", studentsVm);
                }
            }
            if (_userManager.IsInRoleAsync(user, "Teacher").Result)
            {

                var teacher = _userManager.Users.OfType<Teacher>().FirstOrDefault(x => x.UserName == User.Identity.Name);

                var student = teacher.Subjects.SelectMany(x => x.SubjectGroups.SelectMany(y => y.Group.Students)).Distinct().AsQueryable();

                var students = _studentService.GetStudents(filterPredicate,student);

                studentsVm = Mapper.Map<IEnumerable<StudentVm>>(students);
                if (isAjax)
                {
                    return PartialView("_StudentsTableDataPartial", studentsVm);
                }
            }
            if (_userManager.IsInRoleAsync(user, "Admin").Result)
            {
                var students = _db.Users.OfType<Student>();
                if (isAjax)
                {

                    var studentSort = _studentService.GetStudents(filterPredicate, students);
                    studentsVm = Mapper.Map<IEnumerable<StudentVm>>(studentSort);
                    return PartialView("_StudentsTableDataPartial", studentsVm);
                }
                studentsVm = Mapper.Map<IEnumerable<StudentVm>>(students);

            }
            return View(studentsVm);
        }

        [Authorize(Roles ="Parent,Teacher,Student,Admin")]
        public IActionResult Details(int studentId)
        {

            var user = _userManager.GetUserAsync(User).Result;
            var getGradesDto = new GetGradeDto
            {
                StudentId = user.Id,
                GetterUserId = _userManager.GetUserAsync(User).Result.Id
            };


            if (studentId != 0)
            {

                getGradesDto = new GetGradeDto
                {
                    StudentId = studentId,
                    GetterUserId = _userManager.GetUserAsync(User).Result.Id
                };
              
            }
            var studentGradesReport = _gradeService.GetGradesReportForStudent(getGradesDto);

            return View(studentGradesReport);
        }

        [Authorize(Roles = "Teacher,Admin")]
        public IActionResult AttachStudentToGroup(int studentId)
            {
                ViewBag.ActionType = "Attach";
                return AttachDetachGetView(studentId);
            }

        [Authorize(Roles = "Teacher,Admin")]
        public IActionResult DetachStudentToGroup(int studentId)
        {
            ViewBag.ActionType = "Detach";
            return AttachDetachGetView(studentId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetachStudentToGroup(AttachDetachStudentToGroup attachDetachStudentToGroupDto)
        {
            if (ModelState.IsValid)
            {
                _groupService.DetachStudentToGroup(attachDetachStudentToGroupDto);
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AttachStudentToGroup(AttachDetachStudentToGroup attachDetachStudentToGroupDto)
        {
            if (ModelState.IsValid)
            {
                _groupService.AttachStudentToGroup(attachDetachStudentToGroupDto);
                return RedirectToAction("Index");
            }

            return View();
        }
        public IActionResult AttachDetachGetView(int studentId)
        {
            var students = _studentService.GetStudents();
            var groups = _groupService.GetGroups();
            var currentStudent = students.FirstOrDefault(x => x.Id == studentId);
            if (currentStudent == null)
            {
                throw new ArgumentNullException("studentId not exists.");
            }

            var attachDetachStudentToGroup = new AttachDetachStudentToGroup
            {
                StudentId = currentStudent.Id
            };

            ViewBag.StudentList = new SelectList(students.Select(s => new
            {
                Text = $"{s.FirstName}{s.LastName}",
                Value = s.Id,
                Selected = s.Id
            }), "Value", "Text");

            ViewBag.GroupsList = new SelectList(groups.Select(g => new
            {
                Text = g.Name,
                Value = g.Id,
                Selected = g.Id
            }), "Value", "Text");
            return View("AttachStudentToGroup", attachDetachStudentToGroup);

        }

    }
}