using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService userService;

        [HttpPost("getById")]
        public IActionResult GetById(Guid id)
        {
            string msg = string.Empty;     
            var user = userService.GetById(id, out msg);

            if (!string.IsNullOrWhiteSpace(msg)) 
                return View(msg);


            if (user == null) 
                return View("User not found");

            return View(Ok(user));
        }
    }
}
