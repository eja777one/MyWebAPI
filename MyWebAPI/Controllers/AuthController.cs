using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Attributes;
using MyWebAPI.Dto;
using MyWebAPI.Dto.User;
using MyWebAPI.Services.Interfaces;

namespace MyWebAPI.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController
    {
        private readonly IUsersService _usersService;

        public AuthController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>Try user login to system</summary>
        /// <response code="204">If login and password are correct</response>
        /// <response code="400">If the inputModel has incorrect values</response>
        /// <response code="401">If the password or login is wrong</response>
        [HttpPost]
        [Route("/auth/login")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult?> Login(LoginInputDto dto)
        {
            var isLogined = await _usersService.Login(dto);
            return isLogined ? TypedResults.NoContent() : TypedResults.Unauthorized();
        }
    }
}
