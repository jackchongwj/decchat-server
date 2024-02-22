using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.SignalR;

namespace ChatroomB_Backend.Service
{
    public class ErrorHanldeServices : IErrorHandleService
    {
        private readonly IErrorHandleRepo _repo;

        public ErrorHanldeServices(IErrorHandleRepo _repository)
        {
            _repo = _repository;
        }

        public async Task LogError(string controllerName, string errorMessage)
        {
            await _repo.LogError(controllerName, errorMessage);
        }
    }
}
