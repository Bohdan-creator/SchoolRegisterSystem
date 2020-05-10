using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace SchoolRegisterSystem.Controllers
{
    public class BaseController<TController> : Controller
    {
        protected readonly IStringLocalizer<TController> _localizer;
        protected readonly Microsoft.Extensions.Logging.ILogger _logger;

        public BaseController(IStringLocalizer<TController> localizer, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            _localizer = localizer;
            _logger = loggerFactory.CreateLogger(GetType());
        }
    }
}