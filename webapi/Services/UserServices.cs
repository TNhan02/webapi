using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Context;
using webapi.Models;
using webapi.Models.DTO;

namespace webapi.Services
{
    public interface IUser
    {
        Task<IEnumerable<object>> GetUsers(PaginationFilter filter);
        Task<object> GetUser(int id);
        Task<object> AdminPatchUser(int id, [FromBody] DTOUser editUser);
        Task<object> WorkerPatchUser(int id, [FromBody] DTOUser editUser);
        Task<object> PostUser([FromBody] DTOUser newUser);
        Task<object> DeleteUser(int id);
    }

    public class UserServices : IUser
    {
        private ServerContext _context;
        private readonly IMapper _mapper;

        public UserServices(ServerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<IEnumerable<object>> GetUsers(PaginationFilter filter)
        {
            try
            {
                var users = await _context.Users.Skip((filter.pageNumber - 1) * filter.pageSize)
                                                .Take(filter.pageSize)
                                                .ToListAsync();

                var userDTOs = users.Select(user => new
                {
                    Id = user.Id,
                    UserRole = user.UserRole,
                    Username = user.Username,
                    Name = user.Name,
                    Description = user.Description,
                    AdditionalInfo = user.AdditionalInfo
                }).ToList();

                return userDTOs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

            if (user != null)
            {
                var userDTO = new
                {
                    Id = user.Id,
                    UserRole = user.UserRole,
                    Username = user.Username,
                    Name = user.Name,
                    Description = user.Description,
                    AdditionalInfo = user.AdditionalInfo
                };

                return userDTO;
            }
            else
            {
                return new NotFoundObjectResult($"User {id} not found");
            }
        }

        public async Task<object> AdminPatchUser(int id, [FromBody] DTOUser editUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
            {
                return new NotFoundObjectResult($"User {id} not found to edit");
            }

            try
            {
                AdminAdjustUserProperties(user, editUser);
                await _context.SaveChangesAsync();

                return editUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> WorkerPatchUser(int id, [FromBody] DTOUser editUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
            {
                return new NotFoundObjectResult($"User {id} not found to edit");
            }

            try
            {
                WorkerAdjustUserProperties(user, editUser);
                await _context.SaveChangesAsync();

                return editUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> PostUser([FromBody] DTOUser newUser)
        {
            if (newUser == null)
            {
                return new BadRequestObjectResult("Missing input data to create a new user");
            }

            try
            {
                var user = _mapper.Map<User>(newUser);

                // Hash password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return newUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User {id} not found to delete");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return null;
        }

        private void AdminAdjustUserProperties(User user, DTOUser editUser)
        {
            var properties = typeof(DTOUser).GetProperties();

            foreach (var property in properties)
            {
                var newValue = property.GetValue(editUser);

                if (newValue != null)
                {
                    switch (property.Name)
                    {
                        case "Password":
                            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword((string)newValue);
                            break;
                        case "UserRole":
                            if (editUser.UserRole == "Admin" || editUser.UserRole == "Worker")
                            {
                                user.UserRole = editUser.UserRole;
                            }
                            else
                            {
                                throw new Exception("An inappropriate role is set! Please check it again!");
                            }
                            break;
                        default:
                            var userProperty = user.GetType().GetProperty(property.Name);
                            userProperty.SetValue(user, newValue);
                            break;
                    }
                }
            }
        }

        private void WorkerAdjustUserProperties(User user, DTOUser editUser)
        {
            var workerEditProperty = "PasswordHash";
            var properties = typeof(DTOUser).GetProperties();

            foreach (var property in properties)
            {
                var newValue = property.GetValue(editUser);
                if (workerEditProperty == property.Name)
                {
                    if (newValue != null)
                    {
                        if (property.Name == "Password")
                        {
                            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword((string)newValue);
                        }
                        else
                        {
                            var userProperty = user.GetType().GetProperty(property.Name);
                            userProperty.SetValue(user, newValue);
                        }
                    }
                }
                else
                {
                    throw new Exception("Inappropriate properties filled! Please check your right again or contact the administrator to have more information");
                }
            }
        }
    }
}
