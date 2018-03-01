using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Detectives2.Models;
using PagedList;

namespace Detectives2.Controllers
{
    public class AuthorsController : Controller
    {
        private FictionalDetectivesEntities2 db = new FictionalDetectivesEntities2();

        // GET: Authors
        public ActionResult Index(string sortOrder, string searchString, string currentFilter)
        {
            //create IQueryable variable
            var authors = from a in db.Authors
                             select a;

            //filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                authors = authors.Where(s => s.Name.Contains(searchString));
            }

            //modify IQueryable variable based on current sort
            switch (sortOrder)
            {
                case "nameDescending":
                    //detectives = detectives.OrderByDescending(s => s.Name);
                    authors = SortByLastName(authors, "descending");
                    break;
                case "yearAscending":
                    authors = authors.OrderBy(s => s.Years);
                    break;
                case "yearDescending":
                    authors = authors.OrderByDescending(s => s.Years);
                    break;
                default:
                    //detectives = detectives.OrderBy(s => s.Name);
                    authors = SortByLastName(authors, "ascending");
                    break;
            }

            //set column heading hyperlinks to the opposite values to the current sort
            //if no sort order chosen, set column heading hyperlink to name sort in descending order
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "nameDescending" : "";
            //if sort order is year, then column heading hyperlink should be year in descending order
            ViewBag.YearSortParam = sortOrder == "yearAscending" ? "yearDescending" : "yearAscending";
         
            return View(authors);
        }

        // GET: Authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: Authors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Years,Biography,Image")] Author author)
        {
            if (author.Image == null)
            {
                author.Image = @"~\Content\writer.jpg";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Authors.Add(author);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Years,Biography,Image")] Author author)
        {
            if (author.Image == null)
            {
                author.Image = @"~\Content\writer.jpg";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(author).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            //Need to delete reference to book if author is deleted ***
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int numAssociatedBooks = 0;

            try
            {
                //get author record and find out if there are associated books or films
                Author author = db.Authors.Find(id);
                numAssociatedBooks = author.Books.Count();
                //only allow delete if there are no books or films associated with detective
                if (numAssociatedBooks == 0)
                {
                    db.Authors.Remove(author);
                    db.SaveChanges();
                }
                else
                {
                    TempData["MyErrorMessage"] = "You cannot delete this author while there are books linked to them.";
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

        public IQueryable<Author> SortByLastName(IQueryable<Author> authors, string sortOrder)
        {
            string[] lastNames = new string[100];
            int i = 0;
            int j = 0;

            //if (authors == null)
            //{
            //    //tested for Books - seems okay
            //}
            //need to deal with trailing spaces - in all surname sorts ***
            //get the last word in each name string as the last name and make sure they're not spaces
            foreach (Author a in authors)
            {
                string[] temp = a.Name.Split(' ');
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
                foreach (Author author in authors)
                {
                    if (author.Name.Contains(lastName))
                    {
                        author.SortOrder = sortIndex;
                        db.Entry(author).State = EntityState.Modified;
                    }
                }
                sortIndex++;
            }

            db.SaveChanges();

            authors = authors.OrderBy(x => x.SortOrder);

            return authors;
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
