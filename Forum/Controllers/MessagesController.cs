using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Forum.Models;
using Microsoft.AspNet.Identity;

namespace Forum.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Messages/Create
        public ActionResult Create(int? threadId)
        {
            if (threadId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.ThreadId = threadId;
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,PublicationTime")] Message message, int threadId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();

                var owner = await db.Users.FirstOrDefaultAsync(user => user.Id == userId);
                if (owner == null)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                message.Owner = owner;

                var thread = await db.Threads.FindAsync(threadId);
                message.Thread = thread;

                message.PublicationTime = DateTime.Now;
                message.Content = message.Content;

                db.Messages.Add(message);
                await db.SaveChangesAsync();
                message.Thread = null;
                return PartialView("_Message", message);
            }

            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }

            if (User.Identity.GetUserId() != message.Owner.Id && !User.IsInRole("Admin"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,PublicationTime")] Message message)
        {
            if (User.Identity.GetUserId() != message.Owner.Id && !User.IsInRole("Admin"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "ForumThreads", new { id = message.Thread.Id});
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }

            if (User.Identity.GetUserId() != message.Owner.Id && !User.IsInRole("Admin"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Message message = await db.Messages.FindAsync(id);
            if (User.Identity.GetUserId() != message.Owner.Id && !User.IsInRole("Admin"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            var threadId = message.Thread.Id;
            db.Messages.Remove(message);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", "ForumThreads", new { id = threadId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
