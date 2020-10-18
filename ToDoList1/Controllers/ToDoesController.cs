using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDoList1.Models;

namespace ToDoList1.Controllers
{
    public class ToDoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ToDoes
        public ActionResult Index()
        {
            return View(); //!!!!!!!!!!!!!!!ulepszeniem będzie jak załaduję listę, a PartialView będą tylko zmiany, 
            //skróci to czas ładowania listy za pierwszym razem, bo teraz jest w js, a on ładuje się ostatni zawsze
        }

        private IEnumerable<ToDo> GetMyToDoes()
        {
            string currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            IEnumerable<ToDo> myToDoes = db.ToDos.ToList().Where(x => x.User == currentUser);

            int completeCount = 0;
            foreach (ToDo toDo in myToDoes)
            {
                if(toDo.IsDone)
                { completeCount++; }
            }

            ViewBag.Percent = Math.Round(100f * ((float)completeCount / (float)myToDoes.Count()));

            return myToDoes;
            //user ma prawo tylko do swoich itemów
            //!!!!!!!!!!!!!Jak sie wyloguje to nadal widac itemy nie przypisane do zadnego usera
        }

        public ActionResult BuildToDoTable()
        {          
            return PartialView("_ToDoTable", GetMyToDoes());
        }

        // GET: ToDoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // GET: ToDoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x=>x.Id == currentUserId); //łączę obecnego usera z itemem ktory tworzy
                toDo.User = currentUser; // W Bind Create metody nie ustalamy wartości property User, dopiero teraz; to jest jakaś tam forma zabezpieczenia przed hackerem który próbowałby wrzucić innemu userowi nowy item somehow
                db.ToDos.Add(toDo);
                db.SaveChanges();
                return RedirectToAction("Index"); //=>Po stworzeniu i dodaniu nowego itemu strona automatycznie tam mnie przekierowuje
            }

            return View(toDo);
        }

        public ActionResult AJAXCreate([Bind(Include = "Id,Description")] ToDo toDo) 
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId); //łączę obecnego usera z itemem ktory tworzy
                toDo.User = currentUser; // W Bind Create metody nie ustalamy wartości property User, dopiero teraz; takie to proste
                toDo.IsDone = false;
                db.ToDos.Add(toDo);
                db.SaveChanges();
                
            }

            return PartialView("_ToDoTable", GetMyToDoes());
        }

        // GET: ToDoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            if (toDo.User != currentUser) //Zabezpieczenie przed userem, który probuje tak dostac sie do taskow z DB bez autoryzacji
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(toDo);

        }

        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDo);
        }

        [HttpPost]
        //!!!!!!!!!!!!![ValidateAntiForgeryToken]
        public ActionResult AJAXEdit(int? id, bool value)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            else
            {
                toDo.IsDone = value;
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("_ToDoTable", GetMyToDoes());
            }
                      
        }

            // GET: ToDoes/Delete/5
            public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToDo toDo = db.ToDos.Find(id);
            db.ToDos.Remove(toDo);
            db.SaveChanges();
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
