using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ChatroomB_Backend.Service
{
    public class AuthServices : IAuthService
    {

        private readonly IAuthRepo _repo;

        public AuthServices(IAuthRepo repository) 
        {
            _repo = repository;
        }

        public async Task<string> GetSalt(string username)
        {
            return await _repo.GetSalt(username);
        }

        public async Task<bool> VerifyPassword(string username, string hashedPassword)
        {
            return await _repo.VerifyPassword(username, hashedPassword);
        }

        public async Task<IActionResult> AddUser(Users user)
        {
            return await _repo.AddUser(user);
        }
    }
}
