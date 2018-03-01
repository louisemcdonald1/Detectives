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
    public class BooksController : Controller
    {
        private FictionalDetectivesEntities2 db = new FictionalDetectivesEntities2();

        // GET: Books
        public ActionResult Index(string sortOrder, string searchString, string currentFilter)
        {
            //create IQueryable variable
            var books = db.Books.Include(b => b.Author).Include(b => b.Detective);

            //filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }

            //modify IQueryable variable based on current sort
            switch (sortOrder)
            {
                case "nameDescending":
                    //detectives = detectives.OrderByDescending(s => s.Name);
                    books = SortByTitle(books, "descending");
                    break;
                case "yearAscending":
                    books = books.OrderBy(s => s.Year);
                    break;
                case "yearDescending":
                    books = books.OrderByDescending(s => s.Year);
                    break;
                default:
                    //detectives = detectives.OrderBy(s => s.Name);
                    books = SortByTitle(books, "ascending");
                    break;
            }

            //set column heading hyperlinks to the opposite values to the current sort
            //if no sort order chosen, set column heading hyperlink to name sort in descending order
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "nameDescending" : "";
            //if sort order is year, then column heading hyperlink should be year in descending order
            ViewBag.YearSortParam = sortOrder == "yearAscending" ? "yearDescending" : "yearAscending";

            //pass sorted list to view (query executed here)
            
            return View(books);
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "Name");
            ViewBag.DetectiveId = new SelectList(db.Detectives, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Year,Plot,Spoilers,Image,AuthorId,DetectiveId")] Book book)
        {
            if (book.Image == null)
            {
                book.Image = "~\\Content\\book.png";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Books.Add(book);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "Name", book.AuthorId);
            ViewBag.DetectiveId = new SelectList(db.Detectives, "Id", "Name", book.DetectiveId);
            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "Name", book.AuthorId);
            ViewBag.DetectiveId = new SelectList(db.Detectives, "Id", "Name", book.DetectiveId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Year,Plot,Spoilers,Image,AuthorId,DetectiveId")] Book book)
        {
            if (book.Image == null)
            {
                book.Image = "~\\Content\\book.png";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(book).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "Name", book.AuthorId);
            ViewBag.DetectiveId = new SelectList(db.Detectives, "Id", "Name", book.DetectiveId);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IQueryable<Book> SortByTitle(IQueryable<Book> books, string sortOrder)
        {
            string[] titles = new string[100];
            int i = 0;
            int j = 0;

            //if (books == null)
            //{
            //    //tested here - seems okay
            //}

            //get the last word in each name string as the last name and make sure they're not spaces
            //if the first word is 'a', 'an' or 'the' (case-insensitive), remove it from sortable part of title
            foreach (Book b in books)
            {
                string[] temp = b.Title.Split(' ');
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
                //Array.Sort(titles);
                titles = titles.OrderBy(c => c).ToArray();
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

            //Update db with sort order
            int sortIndex = 0;
            //2 titles can get the same sort order value if one title
            //completely contains another title.  This is hard to avoid
            //because .Contains is needed for titles that have had a, an
            //or the deleted.  It's not going to be a problem very often,
            //so I'm leaving it as is for now, though I will fix it if it 
            //causes frequent problems.  
            foreach (string title in titlesList)
            {
                foreach (Book book in books)
                {
                    if (book.Title.Contains(title))
                    {
                        book.SortOrder = sortIndex;
                        db.Entry(book).State = EntityState.Modified;
                    }
                }
                sortIndex++;
            }
            
            db.SaveChanges();

            books = books.OrderBy(x => x.SortOrder);

            return books;
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
