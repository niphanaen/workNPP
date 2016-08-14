﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PPcore.Models;
using PPcore.Services;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace PPcore.Controllers
{
    public class SecurityController : Controller
    {
        private readonly SecurityDBContext _context;
        private readonly IEmailSender _emailSender;
        private IConfiguration _configuration;
        private IHostingEnvironment _env;
        private readonly ILogger _logger;

        public SecurityController(SecurityDBContext context, IEmailSender emailSender, IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _context = context;
            _emailSender = emailSender;
            _configuration = configuration;
            _env = env;
            _logger = loggerFactory.CreateLogger<SecurityController>();
        }

        [HttpGet]
        public IActionResult ManageUsers()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ManageRoles()
        {
            return View();
        }

        public void SendEmail(string email, string username, string password)
        {
            var title = "พลังปัญญา";
            var body = "ชื่อผู้ใช้งาน: " + username + "\nรหัสผ่าน: " + password;
            _emailSender.SendEmailAsync(email, title, body);
        }

        public IActionResult RegisterMember()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(string uname, string upwd, string ReturnUrl)
        //{
        //    //ApplicationUser au = new ApplicationUser();
        //    //au.UserName = uname;

        //    //var result = await _signInManager.PasswordSignInAsync(uname, upwd, false, lockoutOnFailure: false);
        //    //if (result.Succeeded)
        //    //{
        //    //    _logger.LogInformation(1, "User logged in.");
        //    //    //return RedirectToAction(nameof(HomeController.Index), "Home");
        //    //    return Json(new { result = "success" });
        //    //    //if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        //    //    //{
        //    //    //    //Re-check this if in case user try to get on page with correct authentication but wrong authorization
        //    //    //    return Redirect(ReturnUrl);

        //    //    //}
        //    //    //else
        //    //    //{
        //    //    //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //    //    //    //return Json(new { result = "success" });
        //    //    //}

        //    //}
        //    //else
        //    //{
        //    //    return Json(new { result = "fail" });
        //    //}

        //    return Json(new { result = "success" });
        //}

        [HttpPost]
        public async Task<IActionResult> Create(string birthdate, string cid_card, string email, string fname, string lname, string mobile, string mem_photo, string cid_card_pic)
        {
            DateTime bd = Convert.ToDateTime(birthdate);
            //birthdate = (bd.Year).ToString() + bd.Month.ToString() + bd.Day.ToString();
            birthdate = (bd.Year).ToString() + bd.ToString("MMdd");
            string password = cid_card.Substring(cid_card.Length - 4);
            try
            {

                _context.Database.ExecuteSqlCommand("INSERT INTO member (member_code,cid_card,birthdate,fname,lname,mobile,email,x_status,mem_password,mem_photo,cid_card_pic) VALUES ('" + cid_card + "','" + cid_card + "','" + birthdate + "',N'" + fname + "',N'" + lname + "','" + mobile + "','" + email + "','Y','" + password + "','" + mem_photo + "','" + cid_card_pic + "')");

                //var user = new ApplicationUser { UserName = cid_card, Email = email };
                //var result = await _userManager.CreateAsync(user, password);
                
                //if (result.Succeeded)
                //{
                //    await _userManager.AddToRoleAsync(user, "Members");
                //    System.Security.Claims.Claim cl = new System.Security.Claims.Claim("fullName", fname + " " + lname);
                //    await _userManager.AddClaimAsync(user, cl);
                //    //await _signInManager.SignInAsync(user, isPersistent: false);
                //    _logger.LogInformation(3, "User created a new account with password.");
                //    SendEmail(email, cid_card, password);
                //}
                //else
                //{
                //    return Json(new { result = "fail", error_code = -1, error_message = "userManaget cannot create user!" });
                //}
            }
            catch (SqlException ex)
            {
                var errno = ex.Number; var msg = "";
                if (errno == 2627) //Violation of primary key. Handle Exception
                {
                    msg = "duplicate";
                }
                return Json(new { result = "fail", error_code = errno, error_message = msg });
            }
            catch (Exception ex)
            {
                var errno = ex.HResult; var msg = "";
                if (ex.InnerException.Message.IndexOf("PRIMARY KEY") != -1)
                {
                    msg = "duplicate";
                }
                return Json(new { result = "fail", error_code = errno, error_message = msg });
            }

            return Json(new { result = "success" });
        }

        //[HttpGet]
        //public async Task<IActionResult> CreateRole(string roleName)
        //{
        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_appcontext), null, null, null, null, null);
        //    if (!await roleManager.RoleExistsAsync("Administrators"))
        //    {
        //        await roleManager.CreateAsync(new IdentityRole("Administrators"));
        //    }
        //    if (!await roleManager.RoleExistsAsync("Operators"))
        //    {
        //        await roleManager.CreateAsync(new IdentityRole("Operators"));
        //    }
        //    if (!await roleManager.RoleExistsAsync("Members"))
        //    {
        //        await roleManager.CreateAsync(new IdentityRole("Members"));
        //    }
        //    if (!await roleManager.RoleExistsAsync(roleName))
        //    {
        //        await roleManager.CreateAsync(new IdentityRole(roleName));
        //    }
        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}

        //[HttpGet]
        //public async Task<IActionResult> LogOff()
        //{
        //    //var user = new ApplicationUser { UserName = "admin", Email = "info@palangpanya.com" };
        //    //var result = await _userManager.CreateAsync(user, "admin1");
        //    //if (result.Succeeded)
        //    //{
        //    //    await _userManager.AddToRoleAsync(user, "Administrators");
        //    //}

        //    await _signInManager.SignOutAsync();
        //    _logger.LogInformation(4, "User logged out.");
        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}

        [HttpPost]
        public IActionResult ForgetPwd(string uname, string upwd)
        {
            //member mb = _context.member.SingleOrDefault(m => (m.cid_card == uname) && (m.x_status == "Y"));
            //if (mb != null)
            //{
            //    SendEmail(mb.email, uname, mb.mem_password);
            //    return Json(new { result = "success" });
            //}
            //else
            //{
            //    return Json(new { result = "fail" });
            //}
            return Json(new { result = "fail" });
        }



        [HttpGet]
        public IActionResult DetailsAsTable(string type)
        {
            if (type == "A") //Administrators
            {

            }
            else if (type == "O") //Operators
            {

            }
            else if (type == "M") //Members
            {

            }
            //var cg = _context.course_group.OrderBy(m => m.cgroup_code);
            //return View(cg.ToList());
            return View();
        }
    }
}