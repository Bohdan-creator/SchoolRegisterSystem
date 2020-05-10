using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SchoolRegister.BLL.Entities;
using SchoolRegister.Services.Interfaces;
using SchoolRegisterSystem.DAL.EF;
using SchoolRegisterSystem.Extensions;
using SchoolRegisterSystem.Services.Interfaces;
using SchoolRegisterSystem.ViewModel.DTos;

namespace SchoolRegisterSystem.Controllers
{
    [Authorize(Roles="Teacher,Admin")]
    public class SubjectController:BaseController<SubjectController>  {

        private readonly ISubjectService subjectService;
        private readonly ITeacherService teacherService;
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext db;
       
        public SubjectController(ISubjectService _subjectService, ITeacherService _teacherService, UserManager<User> _userManager,
            IStringLocalizer<SubjectController> localizer, ILoggerFactory loggerFactory, ApplicationDbContext _db) : base(localizer, loggerFactory)
        {
            subjectService = _subjectService;
            teacherService = _teacherService;
            userManager = _userManager;
            db = _db;
        }
        public IActionResult Index(string filterValue=null)
        {
            Expression<Func<Subject, bool>> filterPredicate = null;

            if (!string.IsNullOrWhiteSpace(filterValue))
            {
                filterPredicate = x => x.Name.Contains(filterValue);
            }


            bool isAjax = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";
            var user = userManager.GetUserAsync(User).Result;


            if (userManager.IsInRoleAsync(user, "Admin").Result)
            {

                var subjects = db.Subjects;

                if (isAjax)
                {
                    var subjectVm = subjectService.GetSubjects(filterPredicate, subjects);
                    return PartialView("_SubjectsTableDataPartial", subjectVm);
                }
                return View(subjectService.GetSubjects());
            }
            if (userManager.IsInRoleAsync(user, "Teacher").Result)
            {
                var teacher = userManager.GetUserAsync(User).Result as Teacher;
                Expression<Func<Subject, bool>> filterTeacher = x => x.TeacherId == teacher.Id;

                var finalExpression = filterPredicate != null ? Expression.Lambda<Func<Subject, bool>>(
                    Expression.AndAlso(filterPredicate.Body, new ExpressionParameterReplacer(
                    filterTeacher.Parameters, filterPredicate.Parameters).Visit(filterTeacher.Body)
                    ), filterPredicate.Parameters) : filterTeacher;

                var subjectsVm = subjectService.GetSubjects(finalExpression);
                if (isAjax)
                {
                    return PartialView("_SubjectsTableDataPartial", subjectsVm);
                }

                //      var man = role.FindByIdAsync(user.FirstName).Result;

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
                ViewBag.ActionType = _localizer["Edit"];
                return View(Mapper.Map<AddOrUpdateSubjectDto>(subject));
            }
            ViewBag.ActionType = _localizer["Add"];
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