using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Dto;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;
using MyWebAPI.Attributes;
using MyWebAPI.Dto.User;

namespace MyWebAPI.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersService _usersService;

        public UsersController(IUsersRepository usersRepository, IUsersService usersService)
        {
            _usersRepository = usersRepository;
            _usersService = usersService;
        }

        /// <summary>Returns all users</summary>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageNumber">pageNumber is number of portions that should be returned</param>
        /// <param name="pageSize">pageSize is portions size that should be returned</param>
        /// <param name="searchLoginTerm">Search term for user Login: Login should contains this term in any position</param>
        /// <param name="searchEmailTerm">Search term for user Email: Email should contains this term in any position</param>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [BasicAuthorization]
        [ProducesResponseType(typeof(PaginatorDto<ViewUserDto>), StatusCodes.Status200OK)]
        public async Task<IResult> GetUsers(string sortBy = "createdAt", string sortDirection = "desc", int pageNumber = 1,
             int pageSize = 10, string searchLoginTerm = "null", string searchEmailTerm = "null")
        {
            var dto = new GetUsersQueryDto(sortBy, sortDirection, pageNumber, pageSize, searchLoginTerm, searchEmailTerm);

            var users = await _usersRepository.GetUsers(dto);
            return TypedResults.Ok(users);
        }

        /// <summary>Add new user</summary>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [BasicAuthorization]
        [ValidateModel]
        [ProducesResponseType(typeof(ViewUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult> AddUser(InputUserDto dto)
        {
            var user = await _usersService.AddUser(dto);
            return user is null ? TypedResults.BadRequest() : TypedResults.Created("", user);
        }

        /// <summary>Delete user specified by id</summary>
        /// <param name="id">Id of existing user</param>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>
        [HttpDelete]
        [Route("/users/{id}")]
        [BasicAuthorization]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> DeleteUser(int id)
        {
            var isDeleted = await _usersRepository.DeleteUser(id);
            return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}
