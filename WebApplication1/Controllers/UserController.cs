﻿using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {

        dbemarketingEntities1 db = new dbemarketingEntities1();

        public ActionResult Home(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
            //         return View();
            //}
        }

            public ActionResult Location()
        {
            return View();
        }


        public ActionResult Image()
        {
            return View();
        }
        // GET: User
        public ActionResult Index(int ?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }
        public ActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(tbl_user uvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_email = uvm.u_email;
                u.u_password = uvm.u_password;
                u.u_image = path;
                u.u_contract = uvm.u_contract;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Home");
            
            }

            return View();
        } //method......................... end.....................

        public ActionResult login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult login(tbl_user avm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (ad != null)
            {

                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.error = "Invalid username or password";

            }

            return View();
        }


        [HttpGet]
        public ActionResult CreateAd()
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");

            return View();
        }

        [HttpPost]
        public ActionResult CreateAd(tbl_product pvm, HttpPostedFileBase imgfile, HttpPostedFileBase imgfile1)
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            
            
         string path = uploadimgfile(imgfile);
         string path1 = uploadimgfile(imgfile1);

            if (path.Equals("-1"))
        {
            ViewBag.error = "Image could not be uploaded....";
        }
        else
        {
            tbl_product p = new tbl_product();
            p.pro_name = pvm.pro_name;
            p.pro_Sname = pvm.pro_Sname;
            p.pro_image = path;
            p.pro_fk_cat = pvm.pro_fk_cat;
            p.pro_des = pvm.pro_des;
            p.pro_place = pvm.pro_place;
            p.pro_requiremnet = path1;
            //p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
            db.tbl_product.Add(p);
            db.SaveChanges();
            Response.Redirect("index");

        }
            
            return View();
        }


        public ActionResult Ads(int ?id, int?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x=>x.pro_fk_cat==id).OrderByDescending(x=>x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);

           
        }

        [HttpPost]
        public ActionResult Ads(int? id, int? page,string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }


        public ActionResult ViewAd(int? id)
        {
            Adviewmodel ad = new Adviewmodel();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_name;
            ad.pro_image = p.pro_image;
            ad.pro_Sname = p.pro_Sname;
            ad.pro_des = p.pro_des;
            ad.pro_place = p.pro_place;
            ad.pro_requiremnet = p.pro_requiremnet;
            tbl_category cat = db.tbl_category.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ////ad.u_name = u.u_name;
            ////ad.u_image = u.u_image;
            ////ad.u_contract = u.u_contract;
            ////ad.pro_fk_user = u.u_id;




            return View(ad);
        }


        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }



        public ActionResult DeleteAd(int ?id)
        {

            tbl_product p = db.tbl_product.Where(x => x.pro_id ==id).SingleOrDefault();
            db.tbl_product.Remove(p);
            db.SaveChanges();

            return RedirectToAction("Index");
        }









        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }




        //public string uploadimgfile1(HttpPostedFileBase file1)
        //{
        //    Random r = new Random();
        //    string path1 = "-1";
        //    int random = r.Next();
        //    if (file1 != null && file1.ContentLength > 0)
        //    {
        //        string extension = Path.GetExtension(file1.FileName);
        //        if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
        //        {
        //            try
        //            {

        //                path1 = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file1.FileName));
        //                file1.SaveAs(path1);
        //                path1= "~/Content/upload/" + random + Path.GetFileName(file1.FileName);

        //                //    ViewBag.Message = "File uploaded successfully";
        //            }
        //            catch (Exception ex)
        //            {
        //                path1= "-1";
        //            }
        //        }
        //        else
        //        {
        //            Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
        //        }
        //    }

        //    else
        //    {
        //        Response.Write("<script>alert('Please select a file'); </script>");
        //        path1 = "-1";
        //    }



        //    return path1;
        //}








    }
}