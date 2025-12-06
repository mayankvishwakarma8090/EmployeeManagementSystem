using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace finalProject.Controllers
{
    public class EmployeeController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=LAPTOP-DOH2OEOO\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;");
        // GET: Employee
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Myprofile()
        {
            string email = "";
            if (Session["emp"] != null)
            {
                email = Session["emp"].ToString();
            }
            else
            {
                return Content("<script>alert('Login first');location.href='/home/emplogin'</script>");
            }
            string command = $"select * from tbl_application where emailid='{email}' and ishired=1";
            SqlDataAdapter adapter = new SqlDataAdapter(command, con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            ViewBag.user = dt;
            return View();
        }
        public ActionResult Changepassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Changepassword(string opass, string npass, string cpass)
        {
            if(npass.Equals(cpass))
            {
                if(npass.Equals(opass))
                {
      return Content("<script>alert('New Password and old password should be different');location.href='/employee/Changepassword'</script>");
                }
                else
                {
                    //chnaGE THE PASSWORD
                    string email = Session["emp"].ToString();
                    string command = $"update tbl_emplogin set password='{npass}' where email='{email}' and password='{opass}'";
                    SqlCommand cmd = new SqlCommand(command, con);
                    con.Open();
                   int result= cmd.ExecuteNonQuery();
                    con.Close();
                    if(result>0)
                    {
                        Session.RemoveAll();
                        return Content("<script>alert('Password updated');location.href='/home/emplogin'</script>");
                    }
                    else
                    {
                        return Content("<script>alert('Password could not changed. Old password is incorrect');location.href='/employee/changepassword'</script>");
                    }
                }
            }
            else
            {
   return Content("<script>alert('Password and Confirm password should match');location.href='/employee/Changepassword'</script>");
            }
                return View();
        }
        public ActionResult Leaveapplication()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Leaveapplication(string subject, string details,DateTime fromdate,DateTime todate,HttpPostedFileBase file)
        {
            string filename = "";
            if (file != null)
            {
                filename = file.FileName; 
                file.SaveAs(Server.MapPath("/Content/leavefile/" + file.FileName));
            }
            int totaldays = 0;
            TimeSpan time = todate - fromdate;
            totaldays = time.Days;
            string userid = Session["emp"].ToString();
            string command = $"insert into tbl_leave values('{userid}','{subject}','{details}',@fromdate,@todate,'{DateTime.Now.ToString("yyyy-MM-dd")}',{totaldays},null,'{filename}')";
            SqlCommand cmd = new SqlCommand(command, con);
            cmd.Parameters.AddWithValue("@fromdate", Convert.ToDateTime(fromdate));
            cmd.Parameters.AddWithValue("@todate", Convert.ToDateTime(todate));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('Application Send Successfully.\\n Please Wait for approval');location.href='/employee/leaveapplicationStatus'</script>");
        }
        public ActionResult Attendance()
        {
            string email = "";
            if (Session["emp"] != null)
            {
                email = Session["emp"].ToString();
            }
            else
            {
                return Content("<script>alert('Login first');location.href='/home/emplogin'</script>");
            }
            string command = $"select * from tbl_attendance where empid='{email}' and adate='{DateTime.Now.ToString("yyyy-MM-dd")}'";
            SqlDataAdapter adapter = new SqlDataAdapter(command, con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            ViewBag.data = dt;
            return View();
        }
        [HttpPost]
        public ActionResult Attendance(int? id)
        {
            string command = $"insert into tbl_attendance values('{Session["emp"]}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToShortTimeString()}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}')";
            SqlCommand cmd = new SqlCommand(command,con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('Your attendance is marked for today');location.href='/employee/Attendance'</script>");         
        }
        public ActionResult LeaveapplicationStatus()
        {
            string email = "";
            if (Session["emp"] != null)
            {
                 email = Session["emp"].ToString();
            }
            else
            {
                return Content("<script>alert('Login first');location.href='/home/emplogin'</script>");
            }
                string command = $"select * from tbl_leave  where empid='{email}' order by id desc";
            SqlDataAdapter adapter = new SqlDataAdapter(command,con);
            DataTable data = new DataTable();
            adapter.Fill(data);

            ViewBag.leave = data;
            return View();
        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("emplogin","home");
        }
    }
}