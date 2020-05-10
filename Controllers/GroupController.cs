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
using SchoolRegisterSystem.ViewModel.DTos;
using SchoolRegisterSystem.ViewModel.VMs;

namespace SchoolRegisterSystem.Controllers
{
   
    public class GroupController : Controller
    {
        private IGroupService groupService;
        private readonly UserManager<User> userManager;
        private readonly ITeacherService teacherService;


        public GroupController(IGroupService _groupservice, UserManager<User> _userManager, ITeacherService _teacherService)
        {
            groupService = _groupservice;
            userManager = _userManager;
            teacherService = _teacherService;
        }

        [Authorize(Roles = "Teacher,Admin")]
        public IActionResult Index()
        {
            
            var user = userManager.GetUserAsync(User).Result;

            var teacherGroups = new GetTeacherGroupsDto();
            if (userManager.IsInRoleAsync(user, "Teacher").Result)
                teacherGroups.Id = user.Id;
            if (userManager.IsInRoleAsync(user, "Admin").Result)
            { }

            return View (teacherService.GetTeacherGroups(teacherGroups));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddOrUpdateGroup(int? Id)
        {

            if (Id.HasValue)
            {
                var group = groupService.GetGroup(x => x.Id == Id);
                var groupVm = Mapper.Map<AddOrUpdateGroupDto>(group);
                return View(groupVm);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateGroup(AddOrUpdateGroupDto addOrUpdateGroupDto)
        {
            if (ModelState.IsValid)
            {
                groupService.AddOrUpdate(addOrUpdateGroupDto);
               return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Details(int id)
        {
            var group = groupService.GetGroup(x=>x.Id==id);
            return View(group);
        }


    }
}