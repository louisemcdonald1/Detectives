using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Detectives2.Models;
using PagedList;
using PagedList.Mvc;

namespace Detectives2.Controllers
{
    public class HomeController : Controller
    {
        private FictionalDetectivesEntities2 db = new FictionalDetectivesEntities2();

        // GET: Home
        public ActionResult Index(string sortOrder, string searchString, string currentFilter)
        {
            ViewBag.MyErrorMessage = "";
            
            //create IQueryable variable
            var detectives = from d in db.Detectives
                             select d;

            //filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                detectives = detectives.Where(s => s.Name.Contains(searchString));
            }

            //modify IQueryable variable based on current sort
            switch (sortOrder)
            {
                case "nameDescending":
                    //detectives = detectives.OrderByDescending(s => s.Name);
                    detectives = SortByLastName(detectives, "descending");
                    break;
                case "yearAscending":
                    detectives = detectives.OrderBy(s => s.Year);
                    break;
                case "yearDescending":
                    detectives = detectives.OrderByDescending(s => s.Year);
                    break;
                default:
                    detectives = SortByLastName(detectives, "ascending");
                    break;
            }

            //set column heading hyperlinks to the opposite values to the current sort
            //if no sort order chosen, set column heading hyperlink to name sort in descending order
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "nameDescending" : "";
            //if sort order is year, then column heading hyperlink should be year in descending order
            ViewBag.YearSortParam = sortOrder == "yearAscending" ? "yearDescending" : "yearAscending";

            //pass sorted list to view (query executed here)
            return View(detectives);
        }

        public IQueryable<Detective> SortByLastName(IQueryable<Detective> detectives, string sortOrder)
        {
            string[] lastNames = new string[100];
            int i = 0;
            int j = 0;

            //if (detectives == null)
            //{
            //    //tested in Books - seems okay
            //}

            //get the last word in each name string as the last name and make sure they're not spaces
            foreach (Detective d in detectives)
            {
                string[] temp = d.Name.Split(' ');
                //skip past spaces
                i = temp.Length - 1;
                while (temp[i] == " ")
                {
                    i--;
                }
                //get last name - the last word in the string
                lastNames[j] = temp[i];
                j++;
            }
            //sort the array of last names in ascending or descending order
            if (sortOrder == "ascending")
            {
                Array.Sort(lastNames);
            }
            else if (sortOrder == "descending")
            {
                lastNames = lastNames.OrderByDescending(c => c).ToArray();
            }


            //get rid of null values by transferring actual values in the 
            //array into a list
            List<string> lastNamesList = new List<string>();
            for (int k = 0; k < lastNames.Length; k++)
            {
                if (lastNames[k] != null)
                {
                    lastNamesList.Add(lastNames[k]);
                }
            }

            //Update db with sort order
            int sortIndex = 0;
            foreach (string lastName in lastNamesList)
            {
                foreach(Detective detective in detectives)
                {
                    if (detective.Name.Contains(lastName))
                    {
                        detective.SortOrder = sortIndex;
                        db.Entry(detective).State = EntityState.Modified;
                    }
                }
                sortIndex++;
            }

            db.SaveChanges();

            detectives = detectives.OrderBy(x => x.SortOrder);

            return detectives;
        }

        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Detective detective = db.Detectives.Find(id);
            if (detective == null)
            {
                return HttpNotFound();
            }
            return View(detective);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description,Year,Category,Image")] Detective detective)
        {
            if (detective.Image == null)
            {
                detective.Image = @"~\Content\detective2.jpg"; 
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Detectives.Add(detective);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            

            return View(detective);
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Detective detective = db.Detectives.Find(id);
            if (detective == null)
            {
                return HttpNotFound();
            }
            return View(detective);
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Year,Category,Image")] Detective detective)
        {
            if (detective.Image == null)
            {
                detective.Image = @"~\Content\detective2.jpg";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(detective).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            return View(detective);
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError=false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Helps manage error reporting.  saveChangesError == true if method
            //was called after a failure to save changes, so error message is displayed
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed.  Try again, and if the problem persists, contact your system administrator.";
            }
            Detective detective = db.Detectives.Find(id);
            if (detective == null)
            {
                return HttpNotFound();
            }
            return View(detective);
        }

        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {  
            int numAssociatedBooks = 0;
            int numAssociatedFilmsOrTvShows = 0;
            
            try
            {
                //get detective record and find out if there are associated books or films
                Detective detective = db.Detectives.Find(id);
                numAssociatedBooks = detective.Books.Count();
                numAssociatedFilmsOrTvShows = detective.FilmOrTvs.Count();
                //only allow delete if there are no books or films associated with detective
                if (numAssociatedBooks == 0 && numAssociatedFilmsOrTvShows == 0)
                {
                    db.Detectives.Remove(detective);
                    db.SaveChanges();
                }
                else
                {
                    TempData["MyErrorMessage"] = "You cannot delete this detective while there are books, films or TV shows linked to them.";
                    TempData["DeletionError"] = true;
                    return RedirectToAction("Delete", new { id = id });
                }
                
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
