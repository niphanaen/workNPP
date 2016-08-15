using System;
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

namespace PPcore.Controllers
{
    public class AccountController : Controller
    {
        private readonly PalangPanyaDBContext _context;
        private readonly IEmailSender _emailSender;
        private IConfiguration _configuration;
        private IHostingEnvironment _env;

        public AccountController(PalangPanyaDBContext context, IEmailSender emailSender, IConfiguration configuration, IHostingEnvironment env)
        {
            _context = context;
            _emailSender = emailSender;
            _configuration = configuration;
            _env = env;
        }

        public void SendEmail(string email, string username, string password)
        {
            var title = "พลังปัญญา";
            var body = "ชื่อผู้ใช้งาน: " + username + "\nรหัสผ่าน: " + password;
            _emailSender.SendEmailAsync(email, title, body);
        }

        [HttpPost]
        public IActionResult Create(string birthdate, string cid_card, string email, string fname, string lname, string mobile, string mem_photo, string cid_card_pic)
        {
            DateTime bd = Convert.ToDateTime(birthdate);
            //birthdate = (bd.Year).ToString() + bd.Month.ToString() + bd.Day.ToString();
            birthdate = (bd.Year).ToString() + bd.ToString("MMdd");
            string password = cid_card.Substring(cid_card.Length - 4);
            try
            {
                if ((!string.IsNullOrEmpty(mem_photo)) && (mem_photo.Substring(0, 1) != "M"))
                {
                    var fileName = mem_photo.Substring(9);
                    var fileExt = Path.GetExtension(fileName);
                    pic_image m = new pic_image();
                    m.image_code = "M" + DateTime.Now.ToString("yyMMddhhmmssfffffff") + fileExt;
                    m.x_status = "Y";
                    m.image_name = fileName;

                    m.ref_doc_type = "member";
                    m.ref_doc_code = cid_card; //member_code;
                    fileName = m.image_code;
                    _context.pic_image.Add(m);
                    _context.SaveChanges();

                    var s = Path.Combine(Path.Combine(_env.WebRootPath, _configuration.GetSection("Paths").GetSection("images_upload").Value), mem_photo);
                    var d = Path.Combine(Path.Combine(_env.WebRootPath, _configuration.GetSection("Paths").GetSection("images_member").Value), fileName);
                    System.IO.File.Copy(s, d, true);
                    System.IO.File.Delete(s);
                    //clearImageUpload();

                    mem_photo = m.image_code;
                }
                if ((!string.IsNullOrEmpty(cid_card_pic)) && (cid_card_pic.Substring(0, 1) != "C"))
                {
                    var fileName = cid_card_pic.Substring(9);
                    var fileExt = Path.GetExtension(fileName);
                    pic_image pic_image = new pic_image();
                    pic_image.image_code = "C" + DateTime.Now.ToString("yyMMddhhmmssfffffff") + fileExt;
                    pic_image.x_status = "Y";
                    pic_image.image_name = fileName;
                    pic_image.ref_doc_type = "cidcard";
                    pic_image.ref_doc_code = cid_card; //member_code;
                    fileName = pic_image.image_code;
                    _context.pic_image.Add(pic_image);
                    _context.SaveChanges();

                    var s = Path.Combine(Path.Combine(_env.WebRootPath, _configuration.GetSection("Paths").GetSection("images_upload").Value), cid_card_pic);
                    var d = Path.Combine(Path.Combine(_env.WebRootPath, _configuration.GetSection("Paths").GetSection("images_member").Value), fileName);
                    System.IO.File.Copy(s, d, true);
                    System.IO.File.Delete(s);
                    //clearImageUpload();

                    cid_card_pic = pic_image.image_code;
                }
                _context.Database.ExecuteSqlCommand("INSERT INTO member (member_code,cid_card,birthdate,fname,lname,mobile,email,x_status,mem_password,mem_photo,cid_card_pic) VALUES ('" + cid_card + "','" + cid_card + "','" + birthdate + "',N'" + fname + "',N'" + lname + "','" + mobile + "','" + email + "','Y','" + password + "','" + mem_photo + "','" + cid_card_pic + "')");
                SendEmail(email, cid_card, password);
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

        [HttpPost]
        public IActionResult Login(string uname, string upwd)
        {
            var mb = _context.member.FromSql("select cid_card from member where cid_card = '" + uname + "' and mem_password = '" + upwd + "' and x_status = 'Y'");
            if (mb.Count() == 0)
            {
                return Json(new { result = "fail" });
            }
            else
            {
                return Json(new { result = "success" });
            }
        }

        [HttpPost]
        public IActionResult ForgetPwd(string uname, string upwd)
        {
            member mb = _context.member.SingleOrDefault(m => (m.cid_card == uname) && (m.x_status == "Y"));
            if (mb != null)
            {
                SendEmail(mb.email, uname, mb.mem_password);
                return Json(new { result = "success" });
            }
            else
            {
                return Json(new { result = "fail" });
            }
        }
    }
}