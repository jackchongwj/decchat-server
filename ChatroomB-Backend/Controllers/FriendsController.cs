using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChatroomB_Backend.Data;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : Controller
    {
        //    private readonly ChatroomB_BackendContext _context;

        //    public FriendsController(ChatroomB_BackendContext context)
        //    {
        //        _context = context;
        //    }

        //    // GET: Friends
        //    public async Task<IActionResult> Index()
        //    {
        //        var chatroomB_BackendContext = _context.Friends.Include(f => f.Receiver).Include(f => f.Sender);
        //        return View(await chatroomB_BackendContext.ToListAsync());
        //    }

        //    // GET: Friends/Details/5
        //    public async Task<IActionResult> Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var friends = await _context.Friends
        //            .Include(f => f.Receiver)
        //            .Include(f => f.Sender)
        //            .FirstOrDefaultAsync(m => m.RequestId == id);
        //        if (friends == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(friends);
        //    }

        //    // GET: Friends/Create
        //    public IActionResult Create()
        //    {
        //        ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "Password");
        //        ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "Password");
        //        return View();
        //    }

        //    // POST: Friends/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("RequestId,SenderId,ReceiverId,Status")] Friends friends)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(friends);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "Password", friends.ReceiverId);
        //        ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "Password", friends.SenderId);
        //        return View(friends);
        //    }

        //    // GET: Friends/Edit/5
        //    public async Task<IActionResult> Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var friends = await _context.Friends.FindAsync(id);
        //        if (friends == null)
        //        {
        //            return NotFound();
        //        }
        //        ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "Password", friends.ReceiverId);
        //        ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "Password", friends.SenderId);
        //        return View(friends);
        //    }

        //    // POST: Friends/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int? id, [Bind("RequestId,SenderId,ReceiverId,Status")] Friends friends)
        //    {
        //        if (id != friends.RequestId)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(friends);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!FriendsExists(friends.RequestId))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "Password", friends.ReceiverId);
        //        ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "Password", friends.SenderId);
        //        return View(friends);
        //    }

        //    // GET: Friends/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var friends = await _context.Friends
        //            .Include(f => f.Receiver)
        //            .Include(f => f.Sender)
        //            .FirstOrDefaultAsync(m => m.RequestId == id);
        //        if (friends == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(friends);
        //    }

        //    // POST: Friends/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int? id)
        //    {
        //        var friends = await _context.Friends.FindAsync(id);
        //        if (friends != null)
        //        {
        //            _context.Friends.Remove(friends);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool FriendsExists(int? id)
        //    {
        //        return _context.Friends.Any(e => e.RequestId == id);
        //    }

        private readonly IFriendService _FriendService ;
        private readonly IChatRoomService _ChatRoomService;
        private readonly IHubContext<ChatHub> _hub;

        public FriendsController(IFriendService Fservice, IChatRoomService CService, IHubContext<ChatHub> _hubContext)
        {
            _FriendService = Fservice;
            _ChatRoomService = CService;
            _hub = _hubContext;
        }

        //POST: Friends/Create
        [HttpPost("AddFriend")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Friends friends)
        {
            if (ModelState.IsValid)
            {
                await _FriendService.AddFriends(friends);

                //update friend status
                await _hub.Clients.User(friends.ReceiverId.ToString()).SendAsync("ReceiveFriendRequestNotification");
            }

            return Ok(friends);
        }

        [HttpPost("UpdateFriendRequest")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFriendRequest(FriendRequest request)
        {
            if (ModelState.IsValid)
            {
               int result =  await _FriendService.UpdateFriendRequest(request);

                if (request.Status == 1)
                {
                    await _ChatRoomService.AddChatRoom(request);
                }
            }
            return Ok();
        }

    }
}
