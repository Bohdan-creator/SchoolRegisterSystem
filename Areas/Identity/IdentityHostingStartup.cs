using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolRegister.BLL.Entities;
using SchoolRegisterSystem.DAL.EF;

[assembly: HostingStartup(typeof(SchoolRegisterSystem.Areas.Identity.IdentityHostingStartup))]
namespace SchoolRegisterSystem.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {

            });
        }
    }
}