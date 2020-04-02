using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolRegister.BLL.Entities;
using SchoolRegisterSystem.Services.Interfaces;
using SchoolRegisterSystem.ViewModel;
using SchoolRegisterSystem.ViewModel.VMs;

namespace SchoolRegisterSystem.Controllers
{
    public class StudentController : Controller
    {
        public IGradeService gradeService;

        private readonly IStudentService _studentService;
     //   private readonly IGroupService _groupService;
        private readonly UserManager<User> _userManager;
        private readonly ITeacherService _teacherService;
        private readonly IGradeService _gradeService;

        public StudentController(IStudentService studentService, UserManager<User> userManager, ITeacherService teacherService, IGradeService gradeService)
        {
            _studentService = studentService;
            //_groupService = groupService;
            _userManager = userManager;
            _teacherService = teacherService;
            _gradeService = gradeService;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            IEnumerable<StudentVm> studentsVm = null;
            if (_userManager.IsInRoleAsync(user, "Parent").Result)
            {
                var parent = _userManager.GetUserAsync(User).Result;
                studentsVm = _studentService.GetStudents(x => x.ParentId == parent.Id);
            }
            if (_userManager.IsInRoleAsync(user, "Teacher").Result)
            {
                var teacher = _userManager.Users.OfType<Teacher>().FirstOrDefault(x => x.UserName == User.Identity.Name);
                var student = teacher.Subjects.SelectMany(x => x.SubjectGroups.SelectMany(y => y.Group.Students));
                studentsVm = Mapper.Map<IEnumerable<StudentVm>>(student);
            }

            return View(studentsVm);
        }

        
        public IActionResult Details(int studentId)
        {
            var getGradesDto = new GetGradeDto
            {
                StudentId = studentId,
                GetterUserId = _userManager.GetUserAsync(User).Result.Id
            };
            var studentGradesReport = _gradeService.GetGradesReportForStudent(getGradesDto);
            if (studentGradesReport == null) { throw new ArgumentNullException("studentVm is null"); }


            return View(studentGradesReport);
        }


    }
}