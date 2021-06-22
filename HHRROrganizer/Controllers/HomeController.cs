using HHRROrganizer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HHRROrganizer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles ="admin")]
        public async Task<IActionResult> ModifyPermissions()
        {
            List<AppUser> users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DegradeRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                if(await _userManager.IsInRoleAsync(user, "admin"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "admin");
                    await _userManager.AddToRoleAsync(user, "manager");

                }
                else if (await _userManager.IsInRoleAsync(user, "manager"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "manager");
                    await _userManager.AddToRoleAsync(user, "guest");
                }
            }
            return RedirectToAction(nameof(ModifyPermissions));
        }

        //Here, we create the UpgradeRole method, so that the administrator, in the ModifyPermissions tab, can upgrade the user's status
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpgradeRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                if (await _userManager.IsInRoleAsync(user, "guest"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "guest");
                    await _userManager.AddToRoleAsync(user, "manager");
                }
                else if (await _userManager.IsInRoleAsync(user, "manager"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "manager");
                    await _userManager.AddToRoleAsync(user, "admin");
                }
            }
            return RedirectToAction(nameof(ModifyPermissions));
        }

        // Method to remove users from the application
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(ModifyPermissions));
        }

    }
}
