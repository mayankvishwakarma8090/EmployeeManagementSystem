using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace finalProject.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=LAPTOP-DOH2OEOO\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;");
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string name, string email, long mobile, string message)
        {
            string Query = $"insert into tbl_enquiry(name,email,mobile,message,enqdate) values('{name}','{email}',{mobile},'{message}','{DateTime.Now.ToString("yyyy-MM-dd")}')";
            SqlCommand cmd = new SqlCommand(Query, con);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('Thanks! We have received your message.\\nOur team will contact you soon');location.href='/home/contact'</script>");
        }
        public ActionResult Team()
        {
            return View();
        }
        public ActionResult Opening()
        {
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_opening order by id desc", con);
            DataTable data = new DataTable();
            sda.Fill(data);
            ViewBag.opening = data;
            return View();
        }
        public ActionResult Apply(int? jobid)
        {
            if(jobid.HasValue)
            {
                return View();
            }
            else
            {
                return  Content("<script>alert('please Select a job ');location.href='/home/opening'</script>");
            }
        }
        [HttpPost]
        public ActionResult Apply(int? jobid, string name,string mobno,string email,string address,string exp, int? salary,string quali,string gender,HttpPostedFileBase resume,HttpPostedFileBase profile)
        {
            string command = $"insert into tbl_application values({jobid},'{name}','{mobno}','{email}','{address}','{quali}','{exp}',{salary},'{gender}','{resume.FileName}','{profile.FileName}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}',1, 0, 0)";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            resume.SaveAs(Server.MapPath("/Content/Resume/")+resume.FileName);
            profile.SaveAs(Server.MapPath("/Content/profile/")+profile.FileName);

            return Content("<script>alert('Successfully Applied. Please Wait for Admin Response.');location.href='/home/opening'</script>");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login( string userid, string password, string remember)
        {
            SqlDataAdapter adapter = new SqlDataAdapter($"select * from tbl_adminlogin where userid='{userid}' and password='{password}'",con);
            DataTable data= new DataTable();
            adapter.Fill(data);
            if(data.Rows.Count >0)
            {
                Session["admin_email"] = data.Rows[0]["email"].ToString();
                Session["admin"] = userid;
                // ✅ Remember Me start
                if (remember == "on")
                {
                    HttpCookie idCookie = new HttpCookie("userid", userid);
                    HttpCookie passCookie = new HttpCookie("userpass", password);

                    idCookie.Expires = DateTime.Now.AddDays(7);
                    passCookie.Expires = DateTime.Now.AddDays(7);

                    Response.Cookies.Add(idCookie);
                    Response.Cookies.Add(passCookie);
                }
                else
                {
                    //Remember Me untick cookies delete
                    if (Request.Cookies["userid"] != null)
                    {
                        Response.Cookies["userid"].Expires = DateTime.Now.AddDays(-1);
                    }
                    if (Request.Cookies["userpass"] != null)
                    {
                        Response.Cookies["userpass"].Expires = DateTime.Now.AddDays(-1);
                    }
                }
                //Remember me end
                return Content("<script>alert('Welcome Admin');location.href='/Admin/dashboard'</script>");
            }
            else
            {
                return Content("<script>alert('User Id or Password is Incorrect');location.href='/home/login'</script>");
            }
        }
        public ActionResult Emplogin()
        {
                return View();
        }
        [HttpPost]
        public ActionResult Emplogin(string userid,string password,string remember)
        {
            SqlDataAdapter adapter = new SqlDataAdapter($"select * from tbl_emplogin where email='{userid}'and password='{password}'",con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            if (data.Rows.Count > 0)
            {
                Session["emp"] = userid;
                // ✅ Remember Me start
                if (remember == "on")
                {
                    HttpCookie idCookie = new HttpCookie("empid", userid);
                    HttpCookie passCookie = new HttpCookie("emppass", password);

                    idCookie.Expires = DateTime.Now.AddDays(7);
                    passCookie.Expires = DateTime.Now.AddDays(7);

                    Response.Cookies.Add(idCookie);
                    Response.Cookies.Add(passCookie);
                }
                else
                {
                    //Remember Me untick cookies delete
                    if (Request.Cookies["empid"] != null)
                    {
                        Response.Cookies["empid"].Expires = DateTime.Now.AddDays(-1);
                    }
                    if (Request.Cookies["emppass"] != null)
                    {
                        Response.Cookies["emppass"].Expires = DateTime.Now.AddDays(-1);
                    }
                }
                //Remember me end
                return RedirectToAction("dashboard", "employee");
            }
            else
            {
                return Content("<script>alert('Invalid Id or Password.');location.href='/home/emplogin'</script>");
            }

             
        }
        public ActionResult ForgetPasswordForAdmin()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ForgetPasswordForAdmin(string userid)
        {
            SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM tbl_adminlogin WHERE userid='{userid}'", con);
            DataTable data = new DataTable();
            adapter.Fill(data);

            if (data.Rows.Count > 0)
            {
                string email = data.Rows[0]["email"].ToString();  // assuming column name is 'email'

                //Generate OTP
                string otp = new Random().Next(100000, 999999).ToString();

                //Send OTP Email
                //MailMessage mail = new MailMessage("ms0969910@gmail.com", email);
                MailMessage mail = new MailMessage();
                // Sender (From)
                mail.From = new MailAddress("ms0969910@gmail.com", "HireNext Support");
                // Receiver (To)
                mail.To.Add(new MailAddress(email));
                mail.Subject = "Admin Password Reset OTP";
                mail.Body = $@"
                <div style='font-size:14px; color:#333;'>
                <h2>Password Reset Request</h2>
                <p>Hello <strong>{userid}</strong>,</p>
                <p>We received a request to reset your password. Use the OTP below to verify your identity:</p>
                <h3 style='color:#1a73e8;'>{otp}</h3>
                <p>This OTP is valid for <strong>2 minutes</strong>. Please do not share it with anyone.</p>
                <p>If you did not request this code, you can safely ignore this email. Someone else might have typed your ID by mistake.</p>
                <p>Thanks,<br>HireNext</p>
                </div>";
                mail.IsBodyHtml = true;
                try
                {
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential("ms0969910@gmail.com", "royu qrui mwro shkb");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                    // Store in session
                    Session["OTP"] = otp;
                    Session["OTP_Expiry"] = DateTime.Now.AddMinutes(2);
                    Session["admin"] = userid;
                    Session["adminEmail"] = email;

                    //Redirect to OTP verification page
                    return RedirectToAction("VerifyOTPForAdmin", "Home");
                }
                catch (SmtpException ex)
                {
                    // Log the error if needed
                    return Content("<script>alert('Unable to send OTP.\\nPlease check your internet connection and try again.');location.href='/home/ForgetPasswordForAdmin'</script>");
                }
            }
            else
            {
                return Content("<script>alert('Invalid UserId!');location.href='/home/ForgetPasswordForAdmin'</script>");
            }
        }
        [HttpPost]
        public ActionResult ResendOtpForAdmin()
        {
            if (Session["OTP"] == null || Session["admin"] == null)
            {
                return Content("<script>alert('Please enter your Id first.');location.href='/home/ForgetPasswordForAdmin'</script>");
            }
            string userid = Session["admin"].ToString();
            string email = Session["adminEmail"].ToString();
            string newOtp = new Random().Next(100000, 999999).ToString();
            if (Session["OTP_Resend_Count"] == null)
                Session["OTP_Resend_Count"] = 0;

            int resendCount = (int)Session["OTP_Resend_Count"];
            if (resendCount >= 3)
            {
                return Content("<script>alert('You have reached maximum OTP resend attempts.');location.href='/home/ForgetPasswordForAdmin'</script>");
            }
            Session["OTP_Resend_Count"] = resendCount + 1;
            MailMessage mail = new MailMessage();
            // Sender (From)
            mail.From = new MailAddress("ms0969910@gmail.com", "HireNext Support");
            // Receiver (To)
            mail.To.Add(new MailAddress(email));
            mail.Subject = "Password Reset OTP";
            mail.Body = $@"
                <div style='font-size:14px; color:#333;'>
                <h2>Password Reset Request</h2>
                <p>Hello <strong>{userid}</strong>,</p>
                <p>We received a request to reset your password. Use the OTP below to verify your identity:</p>
                <h3 style='color:#1a73e8;'>{newOtp}</h3>
                <p>This OTP is valid for <strong>2 minutes</strong>. Please do not share it with anyone.</p>
                <p>If you did not request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p>
                <p>Thanks,<br>HireNext</p>
                </div>";
            mail.IsBodyHtml = true;
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("ms0969910@gmail.com", "royu qrui mwro shkb");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                // Store in session
                Session["OTP_Expiry"] = DateTime.Now.AddMinutes(2);
                Session["OTP"] = newOtp;
                Session["adminEmail"] = email;

                //Redirect to OTP verification page
                return RedirectToAction("VerifyOtpForAdmin", "Home");
            }
            catch (SmtpException ex)
            {
                return Content("<script>alert('Unable to resend OTP.\\nPlease check your internet connection and try again.');location.href='/home/ForgetPasswordForAdmin'</script>");
            }
        }
        public ActionResult VerifyOtpForAdmin()
        {
            if (Session["OTP"] == null || Session["admin"] == null)
            {
                return Content("<script>alert('Please enter your userid first.');location.href='/home/ForgetPasswordForAdmin'</script>");
            }
            int remaining = 0;
            if (Session["OTP_Expiry"] != null)
            {
                remaining = (int)((DateTime)Session["OTP_Expiry"] - DateTime.Now).TotalSeconds;
                if (remaining < 0) remaining = 0;
            }
            ViewBag.RemainingSeconds = remaining;
            return View();
        }
        [HttpPost]
        public ActionResult VerifyOtpForAdmin(string enteredOtp)
        {
            if (Session["OTP"] != null && Session["OTP_Expiry"] != null)
            {
                string savedOtp = Session["OTP"].ToString();
                DateTime expiry = (DateTime)Session["OTP_Expiry"];

                if (DateTime.Now > expiry)
                {
                    Session.Remove("OTP_Expiry"); // OTP expire hone par session clear
                    return Content("<script>alert('OTP expired! Please Resend OTP');location.href='/home/VerifyOtpForAdmin'</script>");
                }
                if (enteredOtp == savedOtp)
                {
                    // OTP Verified
                    Session.Remove("OTP");
                    Session.Remove("OTP_Expiry");

                    // ✅ Naya flag set
                    Session["OTP_Verified"] = true;

                    return RedirectToAction("ResetPasswordForAdmin");
                }
                else
                {
                    return Content("<script>alert('Invalid OTP!');location.href='/home/VerifyOTPForAdmin'</script>");
                }
            }

            return Content("<script>alert('Session expired!');location.href='/home/ForgetPasswordForAdmin'</script>");
        }

        // GET: Show Reset Password form
        [HttpGet]
        public ActionResult ResetPasswordForAdmin()
        {
            if (Session["OTP_Verified"] == null || !(bool)Session["OTP_Verified"])
            {
                return Content("<script>alert('Please verify OTP first');location.href='/home/ForgetPasswordForAdmin'</script>");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ResetPasswordForAdmin(string newPassword, string confirmPassword)
        {
            if (Session["OTP_Verified"] == null || !(bool)Session["OTP_Verified"])
            {
                return Content("<script>alert('Unauthorized access!');location.href='/home/ForgetPasswordForAdmin'</script>");
            }

            if (newPassword != confirmPassword)
            {
                return Content("<script>alert('Password do not match');location.href='/home/ResetPasswordForAdmin'</script>");
            }

            string userid = Session["admin"].ToString(); // admin userid
                                                         // Update password in DB
            SqlCommand cmd = new SqlCommand("UPDATE tbl_adminlogin SET password=@pass WHERE userid=@userid", con);
            cmd.Parameters.AddWithValue("@pass", newPassword);
            cmd.Parameters.AddWithValue("@userid", userid);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            // Clear session completely
            Session.Clear();

            return Content("<script>alert('Password reset successful!');location.href='/home/login'</script>");
        }
        public ActionResult Application(string email)      
        {
            if(email!=null)
            {
                string command = $"select * from tbl_application where emailid='{email}'";
                SqlDataAdapter sda = new SqlDataAdapter(command, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                ViewBag.app = dt;
            }
            return View();
        }
        public ActionResult Services()
        {
            return View();
        }
        public ActionResult ForgetPassword()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(string empid)
        {
            SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM tbl_emplogin WHERE email='{empid}'", con);
            DataTable data = new DataTable();
            adapter.Fill(data);

            if (data.Rows.Count > 0)
            {
                //Generate OTP
                string otp = new Random().Next(100000, 999999).ToString();

                //Send OTP Email
                //MailMessage mail = new MailMessage("ms0969910@gmail.com", empid);
                MailMessage mail = new MailMessage();
                // Sender (From)
                mail.From = new MailAddress("ms0969910@gmail.com", "HireNext Support");
                // Receiver (To)
                mail.To.Add(new MailAddress(empid));
                mail.Subject = "Password Reset OTP";
                mail.Body = $@"
          <div style='font-size:14px; color:#333;'>
          <h2>Password Reset Request</h2>
          <p>Hello <strong>{empid}</strong>,</p>
          <p>We received a request to reset your password. Use the OTP below to verify your identity:</p>
          <h3 style='color:#1a73e8;'>{otp}</h3>
          <p>This OTP is valid for <strong>2 minutes</strong>. Please do not share it with anyone.</p>
          <p>If you did not request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p>
          <p>Thanks,<br>HireNext</p>
          </div>";
                mail.IsBodyHtml = true;
                try
                {
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential("ms0969910@gmail.com", "royu qrui mwro shkb");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                    // Store in session
                    Session["OTP"] = otp;
                    Session["OTP_Expiry"] = DateTime.Now.AddMinutes(2);
                    Session["UserEmail"] = empid;

                    //Redirect to OTP verification page
                    return RedirectToAction("VerifyOTP", "Home");
                }
                catch (SmtpException ex)
                {
                    return Content("<script>alert('Unable to send OTP.\\nPlease check your internet connection and try again.');location.href='/home/ForgetPassword'</script>");
                }
            }
            else
            {
                return Content("<script>alert('Invalid EmpId!');location.href='/home/ForgetPassword'</script>");
            }
        }
        [HttpPost]
        public ActionResult ResendOtp()
        {
            if (Session["OTP"] == null || Session["UserEmail"] == null)
            {
                return Content("<script>alert('Please enter your email first.');location.href='/home/ForgetPassword'</script>");
            }
            string empid = Session["UserEmail"].ToString();
            string newOtp = new Random().Next(100000, 999999).ToString();
            if (Session["OTP_Resend_Count"] == null)
                Session["OTP_Resend_Count"] = 0;

            int resendCount = (int)Session["OTP_Resend_Count"];
            if (resendCount >= 3)
            {
                return Content("<script>alert('You have reached maximum OTP resend attempts.');location.href='/home/ForgetPassword'</script>");
            }
            Session["OTP_Resend_Count"] = resendCount + 1;
            MailMessage mail = new MailMessage();
            // Sender (From)
            mail.From = new MailAddress("ms0969910@gmail.com", "HireNext Support");
            // Receiver (To)
            mail.To.Add(new MailAddress(empid));
            mail.Subject = "Password Reset OTP";
            mail.Body = $@"
          <div style='font-size:14px; color:#333;'>
          <h2>Password Reset Request</h2>
          <p>Hello <strong>{empid}</strong>,</p>
          <p>We received a request to reset your password. Use the OTP below to verify your identity:</p>
          <h3 style='color:#1a73e8;'>{newOtp}</h3>
          <p>This OTP is valid for <strong>2 minutes</strong>. Please do not share it with anyone.</p>
          <p>If you did not request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p>
          <p>Thanks,<br>HireNext</p>
          </div>";
            mail.IsBodyHtml = true;
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("ms0969910@gmail.com", "royu qrui mwro shkb");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                // Store in session
                Session["OTP_Expiry"] = DateTime.Now.AddMinutes(2);
                Session["OTP"] = newOtp;
                Session["UserEmail"] = empid;

                //Redirect to OTP verification page
                return RedirectToAction("VerifyOtp", "Home");
            }
            catch (SmtpException ex)
            {
                return Content("<script>alert('Unable to resend OTP.\\nPlease check your internet connection and try again.');location.href='/home/ForgetPassword'</script>");
            }
        }
        public ActionResult VerifyOtp()
        {
            if (Session["OTP"] == null || Session["UserEmail"] == null)
            {
                return Content("<script>alert('Please enter your email first.');location.href='/home/ForgetPassword'</script>");
            }
            int remaining = 0;
            if (Session["OTP_Expiry"] != null)
            {
                remaining = (int)((DateTime)Session["OTP_Expiry"] - DateTime.Now).TotalSeconds;
                if (remaining < 0) remaining = 0;
            }
            ViewBag.RemainingSeconds = remaining;
            return View();
        }
        [HttpPost]
        public ActionResult VerifyOtp(string enteredOtp)
        {
            if (Session["OTP"] != null && Session["OTP_Expiry"] != null)
            {
                string savedOtp = Session["OTP"].ToString();
                DateTime expiry = (DateTime)Session["OTP_Expiry"];

                if (DateTime.Now > expiry)
                {
                    Session.Remove("OTP_Expiry"); // OTP expire hone par session clear
                    return Content("<script>alert('OTP expired! Please Resend OTP');location.href='/home/verifyOtp'</script>");
                }

                if (enteredOtp == savedOtp)
                {
                    // OTP Verified
                    Session.Remove("OTP");
                    Session.Remove("OTP_Expiry");

                    //new flag set
                    Session["OTP_Verified"] = true;

                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    return Content("<script>alert('Invalid OTP!');location.href='/home/VerifyOTP'</script>");
                }
            }

            return Content("<script>alert('Session expired!');location.href='/home/ForgetPassword'</script>");
        }
        // GET: Show Reset Password form
        [HttpGet]
        public ActionResult ResetPassword()
        {
            if (Session["OTP_Verified"] == null || !(bool)Session["OTP_Verified"])
            {
                return Content("<script>alert('Please verify OTP first');location.href='/home/ForgetPassword'</script>");
            }
            return View();
        }
        // POST: Receive new password and update in DB
        [HttpPost]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (Session["OTP_Verified"] == null || !(bool)Session["OTP_Verified"])
            {
                return Content("<script>alert('Unauthorized access!');location.href='/home/ForgetPassword'</script>");
            }

            if (newPassword != confirmPassword)
            {
                return Content("<script>alert('Password do not match');location.href='/home/ResetPassword'</script>");
            }

            string email = Session["UserEmail"].ToString();

            SqlCommand cmd = new SqlCommand("UPDATE tbl_emplogin SET password=@pass WHERE email=@em", con);
            cmd.Parameters.AddWithValue("@pass", newPassword);
            cmd.Parameters.AddWithValue("@em", email);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result > 0)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("ms0969910@gmail.com", "HireNext Support");
                    mail.To.Add(new MailAddress(email));
                    mail.Subject = "Password Reset Successfully!";
                    mail.Body = $@"
                  <div style='font-family:Segoe UI, Arial, sans-serif; color:#333; max-width:600px; margin:auto; border:1px solid #e0e0e0; border-radius:8px; padding:20px; background:#fafafa;'>
                  <div style='text-align:center; border-bottom:2px solid #2F5496; padding-bottom:10px; margin-bottom:20px;'>
                      <h2 style='color:#2F5496; margin:0;'>HireNext</h2>
                  </div>

                  <p style='font-size:16px;'>Hello <b>Employee</b>,</p>

                  <p style='font-size:15px;'>
                      This is a confirmation that your <b>HireNext account password</b> was changed successfully on 
                      <b>{DateTime.Now.ToString("f")}</b>.
                  </p>

                  <p style='font-size:15px; color:#d9534f;'>
                      If you did <u>not</u> make this change, please reset your password immediately or contact our support team.
                  </p>

                  <div style='margin:20px 0;'>
                  <a href='' 
                  style='background:#1a73e8; color:white; text-decoration:none; padding:12px 20px; border-radius:5px; font-size:15px;'>
                  Reset Password
                  </a>
                 </div>

              <p style='font-size:14px; color:#555;'>If you made this change, no further action is needed.</p>

              <br/>
              <p style='font-size:14px;'>Regards,<br/><b>HireNext Team</b></p>

               <hr style='margin:20px 0; border:0; border-top:1px solid #ddd;'/>
              <p style='font-size:12px; text-align:center; color:#888;'>
               © {DateTime.Now.Year} HireNext. All rights reserved.
                  </p>
              </div>";
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential("ms0969910@gmail.com", "royu qrui mwro shkb");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    // Agar email fail ho jaaye toh ignore karna hai
                }
            }
            Session.Clear(); // Clear all session

            return Content("<script>alert('Password reset successful!');location.href='/home/emplogin'</script>");
        }
        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Feedback(string fullname, string email, string type, string message)
        {
            try
            {
                string Querry = $"insert into tbl_feedback values('{fullname}','{email}','{type}','{message}','{DateTime.Now.ToString("yyyy-MM-dd")}')";
                SqlCommand cmd = new SqlCommand(Querry, con);
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("ms0969910@gmail.com", "HireNext Team");
                mail.To.Add(new MailAddress(email));
                mail.Subject = "Thanks for your feedback - HireNext Team";
                mail.Body = $@"
         <div style='font-family:Segoe UI, Arial, sans-serif; font-size:15px; color:#333; line-height:1.6;'>
         <h2 style='margin-bottom:20px; font-size:22px; color:#2F5496;'>
             <b>HireNext</b>
         </h2>
     
         <p style='font-size:15px;'>
             Dear <b>{fullname}</b>,
         </p>
     
         <p style='font-size:15px;'>
         ✅ Thank you for submitting your valuable <b>feedback</b>.
         </p>
         <p style='font-size:15px;'>
             Your feedback helps us improve our system and provide a better experience.
             Please note that your feedback <b>may also be visible publicly</b>, so kindly avoid sharing any sensitive information.
         </p>
         <p style='font-size:15px;'>
             We truly appreciate your time and effort.🙏
         </p>
         <hr style='margin:30px 0; border:0; border-top:1px solid #ccc;' />
     
         <div style='font-size:12px; color:#999999; margin-top:20px;'>&copy; 2025 HireNext. All rights reserved.</div>
         </div>";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("ms0969910@gmail.com", "royu qrui mwro shkb");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return Content("<script>alert('Feedback Submited Successfully!');location.href='/home/feedback'</script>");
            }
            catch (Exception ex)
            {
                return Content("<script>alert('Opps! Something went wrong');location.href='/home/feedback'</script>");
            }
        }
        public ActionResult ER()
        {
            return View();
        }

    }
}