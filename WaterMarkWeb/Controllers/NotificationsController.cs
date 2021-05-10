using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterMarkWeb.Hubs;

namespace WaterMarkWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationsController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<IActionResult> ComplateWatermarkProcess(string connectionId)
        {
           await _hubContext.Clients.Client(connectionId).SendAsync("NotifyCompleteWatermarkProcess");

            return Ok();
        }
    }
}
