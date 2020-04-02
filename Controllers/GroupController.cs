using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private UserManager<User> userManager;


        public GroupController(IGroupService _groupservice, UserManager<User> _userManager)
        {
            groupService = _groupservice;
            userManager = _userManager;
        }


        public IActionResult Index()
        {

            return View();
        }
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
               // return RedirectToAction("Index");
            }
            return View();
        }

    }
}