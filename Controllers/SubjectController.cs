using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SchoolRegister.BLL.Entities;
using SchoolRegister.Services.Interfaces;
using SchoolRegisterSystem.DAL.EF;
using SchoolRegisterSystem.Services.Interfaces;
using SchoolRegisterSystem.ViewModel.DTos;

namespace SchoolRegisterSystem.Controllers
{
  //  [Authorize(Roles = "Teacher")]
    public class SubjectController : Controller
    {

        private readonly ISubjectService subjectService;
        private readonly ITeacherService teacherService;
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext db;
        public SubjectController(ISubjectService _subjectService, ITeacherService _teacherService, UserManager<User> _userManager) 
        {
            subjectService = _subjectService;
            teacherService = _teacherService;
            userManager = _userManager;
        }

        public IActionResult Index()
        {
            var user = userManager.GetUserAsync(User).Result; 



            if (userManager.IsInRoleAsync(user, "Admin").Result)
            {
                return View(subjectService.GetSubjects());
            }
            if (userManager.IsInRoleAsync(user, "Teacher").Result)
            {
               // var teacher = userManager.GetUserAsync(User).Result as Teacher;
                return View(subjectService.GetSubjects(x => x.TeacherId ==user.Id));
            }
        

            return View();
        }
        
        [HttpGet]
      
        public IActionResult AddOrEditSubject(int? id )
        {
            var teachersVm = teacherService.GetTeachers();
            ViewBag.TeachersSelectList = new SelectList(teachersVm.Select(t => new
            {
                Text = $"{t.FirstName} {t.LastName}",
                Value = t.Id
            })
                , "Value", "Text");




            if (id.HasValue)
            {
                var subject = subjectService.GetSubject(x => x.Id == id);
                ViewBag.ActionType = "Edit";
                return View(Mapper.Map<AddOrUpdateSubjectDto>(subject));
            }
            ViewBag.ActionType = "Add";
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditSubject(AddOrUpdateSubjectDto addOrUpdateSubjectDto)
        {
            if (ModelState.IsValid)
            {
                subjectService.AddOrUpdate(addOrUpdateSubjectDto);
                return RedirectToAction("Index");
            }
            return View();

        }

    }
}