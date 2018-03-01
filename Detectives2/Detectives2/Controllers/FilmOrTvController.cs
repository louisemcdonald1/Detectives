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
    public class FilmOrTvController : Controller
    {
        private FictionalDetectivesEntities2 db = new FictionalDetectivesEntities2();

        // GET: FilmOrTv
        public ActionResult Index(string sortOrder, string searchString, string currentFilter)
        {
            //create IQueryable variable
            var filmsOrTvShows = db.FilmOrTvs.Include(f => f.Actor).Include(f => f.Detective);

            //filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                filmsOrTvShows = filmsOrTvShows.Where(s => s.Title.Contains(searchString));
            }

            //modify IQueryable variable based on current sort
            switch (sortOrder)
            {
                case "nameDescending":
                    //detectives = detectives.OrderByDescending(s => s.Name);
                    filmsOrTvShows = SortByTitle(filmsOrTvShows, "descending");
                    break;
                case "yearAscending":
                    filmsOrTvShows = filmsOrTvShows.OrderBy(s => s.Year);
                    break;
                case "yearDescending":
                    filmsOrTvShows = filmsOrTvShows.OrderByDescending(s => s.Year);
                    break;
                default:
                    //detectives = detectives.OrderBy(s => s.Name);
                    filmsOrTvShows = SortByTitle(filmsOrTvShows, "ascending");
                    break;
            }

            //set column heading hyperlinks to the opposite values to the current sort
            //if no sort order chosen, set column heading hyperlink to name sort in descending order
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "nameDescending" : "";
            //if sort order is year, then column heading hyperlink should be year in descending order
            ViewBag.YearSortParam = sortOrder == "yearAscending" ? "yearDescending" : "yearAscending";
            
            //pass sorted list to view (query executed here)
            return View(filmsOrTvShows);

        }

        // GET: FilmOrTv/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmOrTv filmOrTv = db.FilmOrTvs.Find(id);
            if (filmOrTv == null)
            {
                return HttpNotFound();
            }
            return View(filmOrTv);
        }

        // GET: FilmOrTv/Create
        public ActionResult Create()
        {
            ViewBag.ActorId = new SelectList(db.Actors, "Id", "Name");
            ViewBag.DetectiveId = new SelectList(db.Detectives, "Id", "Name");
            return View();
        }

        // POST: FilmOrTv/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Year,Category,Plot,Spoilers,Image,ActorId,DetectiveId")] FilmOrTv filmOrTv)
        {
            if (filmOrTv.Image == null)
            {
                filmOrTv.Image = @"~\Content\video.png";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.FilmOrTvs.Add(filmOrTv);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }

            ViewBag.ActorId = new SelectList(db.Actors, "Id", "Name", filmOrTv.ActorId);
            ViewBag.DetectiveId = new SelectList(db.Detectives, "Id", "Name", filmOrTv.DetectiveId);
            return View(filmOrTv);
        }

        // GET: FilmOrTv/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmOrTv filmOrTv = db.FilmOrTvs.Find(id);
            if (filmOrTv == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActorId = new SelectList(db.Actors, "Id", "Name", filmOrTv.ActorId);
            ViewBag.DetectiveId = new SelectList(db.Detectives, "Id", "Name", filmOrTv.DetectiveId);
            return View(filmOrTv);
        }

        // POST: FilmOrTv/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Year,Category,Plot,Spoilers,Image,ActorId,DetectiveId")] FilmOrTv filmOrTv)
        {
            if (filmOrTv.Image == null)
            {
                filmOrTv.Image = @"~\Content\video.png";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(filmOrTv).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            ViewBag.ActorId = new SelectList(db.Actors, "Id", "Name", filmOrTv.ActorId);
            ViewBag.DetectiveId = new SelectList(db.Detectives, "Id", "Name", filmOrTv.DetectiveId);
            return View(filmOrTv);
        }

        // GET: FilmOrTv/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilmOrTv filmOrTv = db.FilmOrTvs.Find(id);
            if (filmOrTv == null)
            {
                return HttpNotFound();
            }
            return View(filmOrTv);
        }

        // POST: FilmOrTv/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FilmOrTv filmOrTv = db.FilmOrTvs.Find(id);
            db.FilmOrTvs.Remove(filmOrTv);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IQueryable<FilmOrTv> SortByTitle(IQueryable<FilmOrTv> filmsOrTvShows, string sortOrder)
        {
            string[] titles = new string[100];
            int i = 0;
            int j = 0;

            //if (filmsOrTvShows == null)
            //{
            //    //tested in Books - seems okay
            //}

            //get the last word in each name string as the last name and make sure they're not spaces
            //if the first word is 'a', 'an' or 'the' (case-insensitive), remove it from sortable part of title
            foreach (FilmOrTv f in filmsOrTvShows)
            {
                string[] temp = f.Title.Split(' ');
                //skip past spaces at start of string
                i = 0;
                while (temp[i] == " ")
                {
                    i++;
                }

                //skip 'a', 'an' or 'the' (case-insensitive)
                while (temp[i].ToLower().Equals("a") || temp[i].ToLower().Equals("an") || temp[i].ToLower().Equals("the"))
                {
                    i++;
                }

                //get rest of title and re-assemble it prior to sorting
                while (i < temp.Length)
                {
                    titles[j] += temp[i];
                    //if it's not the last word, add a space
                    if (i < temp.Length - 1)
                    {
                        titles[j] += " ";
                    }
                    i++;
                }
                j++;
            }
            //sort the array of last names in ascending or descending order
            if (sortOrder == "ascending")
            {
                Array.Sort(titles);
            }
            else if (sortOrder == "descending")
            {
                titles = titles.OrderByDescending(c => c).ToArray();
            }


            //get rid of null values by transferring actual values in the 
            //array into a list
            List<string> titlesList = new List<string>();
            for (int k = 0; k < titles.Length; k++)
            {
                if (titles[k] != null)
                {
                    titlesList.Add(titles[k]);
                }
            }
            //2 titles can get the same sort order value if one title
            //completely contains another title.  This is hard to avoid
            //because .Contains is needed for titles that have had a, an
            //or the deleted.  It's not going to be a problem very often,
            //so I'm leaving it as is for now, though I will fix it if it 
            //causes frequent problems.  

            //Update db with sort order
            int sortIndex = 0;
            foreach (string title in titlesList)
            {
                foreach (FilmOrTv filmOrTv in filmsOrTvShows)
                {
                    if (filmOrTv.Title.Contains(title))
                    {
                        filmOrTv.SortOrder = sortIndex;
                        db.Entry(filmOrTv).State = EntityState.Modified;
                    }
                }
                sortIndex++;
            }

            db.SaveChanges();
            //this line is the problem - am I going back to the db? - I thought I was ***
            filmsOrTvShows = filmsOrTvShows.OrderBy(x => x.SortOrder);

            return filmsOrTvShows;
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
