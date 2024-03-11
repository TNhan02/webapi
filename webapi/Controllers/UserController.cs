using Microsoft.AspNetCore.Mvc;
using webapi.DataHandler;
using webapi.Models.DTO;
using webapi.Services;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationFilter filter)
        {
            if (User.HasClaim("Role", UserTypes.Admin))
            {
                var users = await _userService.GetUsers(filter);
                return Json(users);
            }
            else
            {
                return BadRequest("Admin role is required! Please check your token again!");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _userService.GetUser(id);
            return Json(user);
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] DTOUser newUser)
        {
            var createdUser = await _userService.PostUser(newUser);
            return Json(createdUser);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] DTOUser editUser)
        {
            if(User.HasClaim("Role", UserTypes.Admin))
            {
                var editedUser = await _userService.AdminPatchUser(id, editUser);
                return Json(editedUser);
            }
            else if(User.HasClaim("Role", UserTypes.Worker))
            {
                var editedUser = await _userService.WorkerPatchUser(id, editUser);
                return Json(editedUser);
            }
            else
            {
                return BadRequest("Admin or Worker role is required! Please check your token again");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deletedUser = await _userService.DeleteUser(id);
            return NoContent();
        }
    }
}


