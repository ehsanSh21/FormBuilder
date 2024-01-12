using FormBuilder.Data;
using FormBuilder.Models;
using FormBuilder.Validators;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly FormBuilderAPIDbContext dbContext;

        public UsersController(FormBuilderAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(dbContext.Users.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddUsers(User user)
        {

            var validator = new UserValidator();
            var validationResult = validator.Validate(user);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Ok(user);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserRequest updateUserRequest)
        {
            var existingUser = await dbContext.Users.FindAsync(id);

            if (existingUser == null)
            {
                return NotFound(); // User not found
            }

            // Update user properties
            existingUser.Address = updateUserRequest.Address;
            existingUser.FullName = updateUserRequest.FullName;
            existingUser.Email = updateUserRequest.Email;
            existingUser.Phone = updateUserRequest.Phone;

            // Save changes to the database
            await dbContext.SaveChangesAsync();

            return Ok(existingUser);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(); // User not found
            }

            return Ok(user);
        }

    }
}
