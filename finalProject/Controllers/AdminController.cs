using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
namespace finalProject.Controllers
{
    public class AdminController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=LAPTOP-DOH2OEOO\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;");
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Addopening()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Addopening(string title,string detail,string type,string city,int? minsalary,int? maxsalary,string gender,string shift,string exp,string education,int? vacancy,DateTime lastdate)
        {
            string command = $"insert into tbl_opening values('{title}','{detail}','{type}','{city}',{minsalary},{maxsalary},'{gender}','{shift}','{exp}','{education}','{vacancy}','{lastdate.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("yyyy-MM-dd")}',1)";
            SqlCommand cmd = new SqlCommand(command,con);
            con.Open();
            int result=cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('Opening added.');location.href='/admin/addopening'</script>");
        }
        public ActionResult Applicationdetails()
        {
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_application order by id asc", con);
            DataTable data = new DataTable();
            sda.Fill(data);
            ViewBag.application = data;
            return View();
        }
        public ActionResult leaveapplication()
        {
            string command = $"select * from tbl_leave  order by id desc";
            SqlDataAdapter adapter = new SqlDataAdapter(command, con);
            DataTable data = new DataTable();
            adapter.Fill(data);

            ViewBag.leave = data;
            return View();
        }
        public ActionResult emplist()
        {
            SqlDataAdapter adapter = new SqlDataAdapter("select * from tbl_application where ishired=1", con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.employee = data;
            return View();
        }
        public ActionResult salaryslip(string email, DateTime? fromdate, DateTime? todate)
        {
            //select all hired employees 
            SqlDataAdapter adapter = new SqlDataAdapter("select * from tbl_application where ishired=1", con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.employee = data;

            if(email!=null && fromdate.HasValue && todate.HasValue)
            {
                string command = $"select * from tbl_attendance where empid='{email}' and adate between '{fromdate.Value.ToString("yyyy-MM-dd")}' and '{todate.Value.ToString("yyyy-MM-dd")}'";
                SqlDataAdapter sda = new SqlDataAdapter(command, con);
                DataTable attend = new DataTable();
                sda.Fill(attend);
                ViewBag.attend = attend;
            }

            return View();
        }
        public ActionResult OpeningList()
        {
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_opening", con);
            DataTable data = new DataTable();
            sda.Fill(data);
            ViewBag.opening = data;
            
            return View();
        }
        public ActionResult EnquiryList()
        {
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_enquiry", con);
            DataTable data = new DataTable();
            sda.Fill(data);
            ViewBag.opening = data;

            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string opass, string npass, string cpass)
        {
            if (npass.Equals(cpass))
            {
                if (npass.Equals(opass))
                {
                    return Content("<script>alert('New Password and Old Password should be different');location.href='/admin/Changepassword'</script>");
                }
                else
                {
                    //Change the password
                    string userid = Session["admin"].ToString();
                    string command = $"update tbl_adminlogin set password='{npass}' where userid='{userid}' and password='{opass}'";
                    SqlCommand cmd = new SqlCommand(command, con);
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    con.Close();
                    if (result > 0)
                    {
                        try
                        {
                            string email = Session["admin_email"].ToString();
                            //from admin login 
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("ms0969910@gmail.com", "HireNext Support");
                            mail.To.Add(new MailAddress(email));
                            mail.Subject = "Password Change Successfully!";
                            mail.Body = $@"
                 <div style='font-family:Segoe UI, Arial, sans-serif; color:#333; max-width:600px; margin:auto; border:1px solid #e0e0e0; border-radius:8px; padding:20px; background:#fafafa;'>
                 <div style='text-align:center; border-bottom:2px solid #2F5496; padding-bottom:10px; margin-bottom:20px;'>
                     <h2 style='color:#2F5496; margin:0;'>HireNext</h2>
                 </div>

                 <p style='font-size:16px;'>Hello <b>Admin</b>,</p>

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
                            smtp.Credentials = new NetworkCredential("ms0969910@gmail.com", "jldl xfxz yyie lmuq");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            // Agar email fail ho jaaye toh ignore karna hai
                        }
                        Session.RemoveAll();
                        return Content("<script>alert('Password Updated');location.href='/home/login'</script>");
                    }
                    else
                    {
                        return Content("<script>alert('Password could not change.\\nOld password is incorrect');location.href='/admin/changepassword'</script>");
                    }
                }
            }
            else
            {
                return Content("<script>alert('Password and Confirm password should match');location.href='/admin/Changepassword'</script>");
            }
        }
        public ActionResult hired(string email, int? appid)
        {
            if (email == null)
            {
                return Content("<script>alert('Please Select a profile');location.href='/admin/applicationdetails'</script>");
            }
            else
            {
                string command = $"update tbl_application set ishired=1 where id={appid}";
                SqlCommand cmd = new SqlCommand(command, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                //send mail to the user to congratulate that you are hired
                MailMessage mail = new MailMessage("ms0969910@gmail.com",email);
                mail.Subject = "Congratulations From Techpile-Hiring";
                mail.Body = $"Congratulations!! You are hired in our company. Here is your login Id and Password to access employee fracility.<br/><br/> Your Login Id is:{email}<br/>Your Password is: techpile<br/><br/>Fell Free to contact us on:7388078191 any time if you have any quiry.";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com",587);
                smtp.Credentials = new NetworkCredential("ms0969910@gmail.com", "jldl xfxz yyie lmuq");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                //save the login id and password of employee into tbl_emplogin
                string sqlcmd = $"insert into tbl_emplogin values('{email}','techpile',{appid})";
                SqlCommand cmd1 = new SqlCommand(sqlcmd,con);
                con.Open();
                cmd1.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("applicationdetails");
            }
        }
        public ActionResult rejected(int? appid)
        {
            string command = $"update tbl_application set isrejected=1 where id={appid}";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("applicationdetails");
        }

        public ActionResult acceptleave(int? id)
        {
            string command = $"update tbl_leave set isaccepted=1 where id={id}";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("leaveapplication");
        }

        public ActionResult rejectleave(int? id)
        {
            string command = $"update tbl_leave set isaccepted=0 where id={id}";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("leaveapplication");
        }


        public ActionResult logout()
        {
            Session.RemoveAll();
            return RedirectToAction("login", "home");
        }
    }
}