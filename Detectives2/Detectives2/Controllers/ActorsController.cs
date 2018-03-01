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
    public class ActorsController : Controller
    {
        private FictionalDetectivesEntities2 db = new FictionalDetectivesEntities2();

        // GET: Actors
        public ActionResult Index(string sortOrder, string searchString, string currentFilter)
        {
            //create IQueryable variable
            var actors = from a in db.Actors
                         select a;

            //filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                actors = actors.Where(s => s.Name.Contains(searchString));
            }

            //modify IQueryable variable based on current sort
            switch (sortOrder)
            {
                case "nameDescending":
                    //detectives = detectives.OrderByDescending(s => s.Name);
                    actors = SortByLastName(actors, "descending");
                    break;
                case "yearAscending":
                    actors = actors.OrderBy(s => s.Years);
                    break;
                case "yearDescending":
                    actors = actors.OrderByDescending(s => s.Years);
                    break;
                default:
                    //detectives = detectives.OrderBy(s => s.Name);
                    actors = SortByLastName(actors, "ascending");
                    break;
            }

            //set column heading hyperlinks to the opposite values to the current sort
            //if no sort order chosen, set column heading hyperlink to name sort in descending order
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "nameDescending" : "";
            //if sort order is year, then column heading hyperlink should be year in descending order
            ViewBag.YearSortParam = sortOrder == "yearAscending" ? "yearDescending" : "yearAscending";

            //pass sorted list to view (query executed here)
            return View(actors);
        }
    

        // GET: Actors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actor actor = db.Actors.Find(id);
            if (actor == null)
            {
                return HttpNotFound();
            }
            return View(actor);
        }

        // GET: Actors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Years,Biography,Image")] Actor actor)
        {
            if (actor.Image == null)
            {
                actor.Image = "~\\Content\\dramaMasks.png";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Actors.Add(actor);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actor actor = db.Actors.Find(id);
            if (actor == null)
            {
                return HttpNotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Years,Biography,Image")] Actor actor)
        {
            if (actor.Image == null)
            {
                actor.Image = "~\\Content\\dramaMasks.png";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(actor).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            return View(actor);
        }

        // GET: Actors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actor actor = db.Actors.Find(id);
            if (actor == null)
            {
                return HttpNotFound();
            }
            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int numAssociatedFilmsOrTvShows = 0;

            try
            {
                //get actor's record and find out if there are associated books or films
                Actor actor = db.Actors.Find(id);
                numAssociatedFilmsOrTvShows = actor.FilmOrTvs.Count();
                //only allow delete if there are no books or films associated with detective
                if (numAssociatedFilmsOrTvShows == 0)
                {
                    db.Actors.Remove(actor);
                    db.SaveChanges();
                }
                else
                {
                    TempData["MyErrorMessage"] = "You cannot delete this actor while there are films or TV shows linked to them.";
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

        public IQueryable<Actor> SortByLastName(IQueryable<Actor> actors, string sortOrder)
        {
            //string[] firstNames = new string[10];
            string[] lastNames = new string[100];
            int i = 0;
            int j = 0;

            //if (actors == null)
            //{
            //    //tested in Books - seems okay
            //}

            //get the last word in each name string as the last name and make sure they're not spaces
            foreach (Actor a in actors)
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
                foreach (Actor actor in actors)
                {
                    if (actor.Name.Contains(lastName))
                    {
                        actor.SortOrder = sortIndex;
                        db.Entry(actor).State = EntityState.Modified;
                    }
                }
                sortIndex++;
            }

            db.SaveChanges();

            actors = actors.OrderBy(x => x.SortOrder);

            return actors;
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
