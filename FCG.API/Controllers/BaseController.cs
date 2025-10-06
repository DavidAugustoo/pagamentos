using FCG.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult CreateIActionResult<T>(DomainNotificationsResult<T> notifications)
        {
            if (notifications.HasNotifications)
            {
                return BadRequest(notifications.Notifications);
            }

            if (!notifications.HasResult)
            {
                return Ok();
            }

            return Ok(notifications.Result);
        }

        protected IActionResult CreateIActionResult(DomainNotifications notifics)
        {
            if (notifics.HasNotifications)
            {
                return BadRequest(notifics.Notifications);
            }

            return NoContent();
        }
    }
}
