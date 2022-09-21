using APIJWT.NET5.Models.ViewModels;
using APIJWT.NET5.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIJWT.NET5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthRepo _authRepo;
        public AccountController(IAuthRepo authRepo)
        {
            _authRepo = authRepo;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authRepo.RegisterAsync(model);
            if (result.IsAutheticated == false)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { token = result.Token , expireOn = result.ExpireOn });
        }

        [HttpPost("Token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] GetTokenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authRepo.GetTokenAsync(model);
            if (result.IsAutheticated == false)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { token = result.Token, expireOn = result.ExpireOn });
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authRepo.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }

            return Ok(model);
        }



    }
}
