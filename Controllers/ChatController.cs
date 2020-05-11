using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SchoolRegisterSystem.DAL.EF;
using SchoolRegisterSystem.Services.Interfaces;
using SchoolRegisterSystem.ViewModel.VMs;

namespace SchoolRegisterSystem.Controllers
{
    public class ChatController : BaseController<ChatController>
    {
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        public ChatController(IStringLocalizer<ChatController> localizer, IStudentService studentService,
            IGroupService groupService,ILoggerFactory loggerFactory):base(localizer,loggerFactory)
        {
            _studentService = studentService;
            _groupService = groupService;
        }
        public IActionResult Index()
        {
            var students = _studentService.GetStudents().ToList();
            students.Add(new StudentVm()
            {
                UserName="All"
            });
            var chatGroups = _groupService.GetGroups();
            var studentListItems = Mapper.Map<IEnumerable<SelectListItem>>(students);
            var chatGroupListItems = Mapper.Map<IEnumerable<SelectListItem>>(chatGroups);
            return View(new Tuple<IEnumerable<SelectListItem>,IEnumerable<SelectListItem>>(studentListItems,chatGroupListItems));
        }
    }
}