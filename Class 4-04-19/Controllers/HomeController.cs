using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Class_4_04_19.Models;
using System.Web.Security;

namespace Class_4_04_19.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Load()
        {
            PassWordManager pm = new PassWordManager(Properties.Settings.Default.Const);
            ViewModel2 vm = new ViewModel2();
            vm.IsAuthenticated = User.Identity.IsAuthenticated;
            if (vm.IsAuthenticated)
            {
                Users u = pm.GetEmail(User.Identity.Name);
                vm.id = u.id;
            }

            return View(vm);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddUser(Users user)
        {
            PassWordManager pm = new PassWordManager(Properties.Settings.Default.Const);
            pm.AddUser(user);
            return Redirect("/home/login");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LoginToAccount(string email, string password)
        {
            PassWordManager pm = new PassWordManager(Properties.Settings.Default.Const);
            Users user = pm.Login(email, password);
            if (user == null)
            {
                return Redirect("/home/login");
            }

            FormsAuthentication.SetAuthCookie(email, true);
            return Redirect("/home/load");
        }

        public ActionResult Upload(HttpPostedFileBase image, Images images, string password, int userid)
        {
            string ext = Path.GetExtension(image.FileName);
            string filename = $"{Guid.NewGuid()}{ext}";
            string fullpath = $"{Server.MapPath("/UploadedImages")}\\{filename}";
            image.SaveAs(fullpath);
            PassWordManager pm = new PassWordManager(Properties.Settings.Default.Const);
            images.FileName = filename;
            int id = pm.AddImage(images, password, userid);
            images.id = id;
            return View(images);
        }

        public ActionResult EnterPassword(int id, string text)
        {
            PassWordManager pm = new PassWordManager(Properties.Settings.Default.Const);
            Images i = pm.GetImage(id, text);
            ViewModel vm = new ViewModel();
            vm.Image = i;

            if (Session["password"] == null)
            {
                Session["password"] = new List<int>();
            }

            List<int> ids = (List<int>)Session["password"];
            if (ids.Contains(id))
            {
                text = i.Password;
            }
            else if (text == i.Password)
            {
                ids.Add(id);
            }

            if (text != i.Password)
            {
                vm.Password = text;
                vm.IncorrectPassword = true;
            }
            else
            {
                vm.Password = text;
                vm.IncorrectPassword = false;
                int x = pm.Count(id);
                int y = pm.AddToCount(x, id);
                vm.Image.Count = y;
            }
            return View(vm);
        }

        public ActionResult MyAccount(int userid)
        {
            PassWordManager pm = new PassWordManager(Properties.Settings.Default.Const);
            IEnumerable<Images>img= pm.MyAccount(userid);
            return View(img);
        }
    }
}