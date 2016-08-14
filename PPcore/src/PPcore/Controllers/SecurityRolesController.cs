using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPcore.Models;
using System.Data.SqlClient;
using PPcore.ViewModels.SecurityRoles;

namespace PPcore.Controllers
{
    public class SecurityRolesController : Controller
    {
        private readonly SecurityDBContext _scontext;
        private readonly PalangPanyaDBContext _context;

        public SecurityRolesController(PalangPanyaDBContext context, SecurityDBContext scontext)
        {
            _context = context;
            _scontext = scontext;    
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([Bind("RoleName")] SecurityRoles securityRoles)
        {
            if (ModelState.IsValid)
            {
                securityRoles.RoleId = Guid.NewGuid();
                securityRoles.CreatedBy = Guid.NewGuid();
                securityRoles.CreatedDate = DateTime.Now;
                securityRoles.EditedBy = Guid.NewGuid();
                securityRoles.x_status = "Y";
                securityRoles.RoleName = securityRoles.RoleName.Trim();

                _scontext.Add(securityRoles);
                try
                {
                    await _scontext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    //"Violation of UNIQUE KEY constraint 'UK_SecurityRoles'. Cannot insert duplicate key in object 'dbo.SecurityRoles'. The duplicate key value is (??????).\r\nThe statement has been terminated."
                    if (e.InnerException.Message.Contains("UNIQUE"))
                    {
                        return Json(new { result = "dup", RoleName = securityRoles.RoleName });
                    }
                    else
                    {
                        return Json(new { result = "fail" });
                    }
                }
                return Json(new { result = "success" });
            }
            return Json(new { result = "fail" });
        }

        public async Task<IActionResult> DetailsAsBlock()
        {
            List<memberViewModel> ms = new List<memberViewModel>();
            var srs = await _scontext.SecurityRoles.Where(srr => srr.x_status != "N").OrderBy(srr => srr.CreatedDate).ToListAsync();
            foreach (SecurityRoles sr in srs)
            {
                memberViewModel m = new memberViewModel();
                m.SecurityRoles = sr;
                m.memberCount = _scontext.SecurityMemberRoles.Where(smrr => smrr.RoleId == sr.RoleId).Count();
                ms.Add(m);
            }
            return View(ms);
        }

        public async Task<IActionResult> DetailsAsSideMenu()
        {
            List<memberViewModel> ms = new List<memberViewModel>();
            var srs = await _scontext.SecurityRoles.Where(srr => srr.x_status != "N").OrderBy(srr => srr.CreatedDate).ToListAsync();
            foreach (SecurityRoles sr in srs)
            {
                memberViewModel m = new memberViewModel();
                m.SecurityRoles = sr;
                m.memberCount = _scontext.SecurityMemberRoles.Where(smrr => smrr.RoleId == sr.RoleId).Count();
                ms.Add(m);
            }
            return View(ms);
        }
    }
}
