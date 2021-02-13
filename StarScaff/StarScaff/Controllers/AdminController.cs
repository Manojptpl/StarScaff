using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using StarScaff.Models;
using StarScaff.Database_Access_Layer;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections.Specialized;
using System.Text;
using System.Web.Helpers;

namespace StarScaff.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin                
        string SuccessMsg = "", ErrorMsg = "", sender="", result ="";
        string OTPCode, status;
        public static string to;
        db loginLayer = new db();
        Account accModel = new Account();
        DataTable dt = new DataTable(); 
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Verify(Account acc)
        {
            return View();
        }
        [HttpPost]
        public JsonResult SubmitLoginData(string UserName, string Password )
        {
            try
            {
                Account um = new Account();
                dt = loginLayer.LoginDetails(UserName, Password);
                if (dt.Rows.Count >0)
                {
                    Session["ID"] = um.ID.ToString();                 
                    FormsAuthentication.SetAuthCookie(UserName, false);
                    accModel.SuccessMsg = "Successfully Login.";
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Username And Password.");
                    accModel.ErrorMsg = "Invalid Username And Password.";
                }

            }
            catch (Exception ex)
            {
                accModel.ErrorMsg = ex.Message;                
            }
            return Json(accModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
            Response.Expires = -1500;
            Response.CacheControl = "no-cache";
            //FormsAuthentication.SignOut();
            //return RedirectToAction("Login");
            return View();
        }
        
        #region Change Password        
        public ActionResult ChangePassword()
        {
            return View();
        }        
        [HttpPost]
        public JsonResult changepassword(string old_pwd, string new_pwd, string c_pwd)
        {
            string SuccessMsg = "", ErrorMsg = "";
            try
            {
                string msg = loginLayer.changepassword(old_pwd, new_pwd, c_pwd);
                string[] response = msg.Split(',');
                if (response[0] == "Success")
                {                    
                        SuccessMsg = response[1];                    
                }
                else
                {
                    ErrorMsg = response[1]; 
                }
                return Json(new { SuccessMsg, ErrorMsg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorMsg = "Error," + ex.ToString();
                return Json(new { SuccessMsg, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        public JsonResult CreateUserRegistraion(UserModel objUserModel)
        {            
            List<UserModel> UserRegistrationlist = new List<UserModel>();
            UserRegistrationlist.Add(objUserModel);
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable dt = new DataTable();
            dt = converter.ToDataTable(UserRegistrationlist);
            
            Account mm = new Account();
            string res = "";
            try
            {
                res = loginLayer.CreateUserRegistration(dt);
                
                string[] response = res.Split(',');
                if (response[0] == "Success")
                {
                    mm.SuccessMsg = response[1];
                }
                else if (response[0] == "Error")
                {
                    mm.ErrorMsg = response[1];
                }                
            }
            catch (Exception ex)
            {
                mm.ErrorMsg = ex.Message;
            }
            return Json(mm, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public JsonResult Forgot_Password(string tomail)
        {
            string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
            string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();           
            string messageBody = "";
            Random rand = new Random();
            OTPCode = (rand.Next(999999)).ToString();
            MailMessage message = new MailMessage();            
            to = tomail;            
            message.IsBodyHtml = false;
            message.BodyEncoding = UTF8Encoding.UTF8;
            messageBody = "Your reset OPT Code is:" + OTPCode;
            message.To.Add(to);
            message.From = new MailAddress(senderEmail);            
            message.Body = messageBody;
            message.Subject = "Forgot password code";            
            SmtpClient smtp = new SmtpClient("smtp.mail.yahoo.com", 465);
            smtp.EnableSsl = true;
            smtp.Timeout = 100000;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.Send(message);
            try
            {                
                SuccessMsg = "Code send successfully.";
            }
            catch (SmtpException ex)
            {
                ErrorMsg = ex.Message;
            }
            return Json(new { SuccessMsg, ErrorMsg }, JsonRequestBehavior.AllowGet);
        }        
        public ActionResult VerifyCode(string tomail, string vcode)
        {
            Account accModel = new Account();
            if (OTPCode == vcode.ToString())
            {
                to = tomail.ToString();
            }
            else
            {
                ErrorMsg = "Wrong Code.";
            }
            return RedirectToAction("/Admin/ChangePassword");
        }        
    }
}