using ChatroomB_Backend.Repository;

namespace ChatroomB_Backend.Service
{
    public class ErrorHanldeServices : IErrorHandleService
    {
        private readonly IErrorHandleRepo _repo;

        public ErrorHanldeServices(IErrorHandleRepo _repository)
        {
            _repo = _repository;
        }

        public async Task LogError(string controllerName, int userId, string errorMessage)
        {
            await _repo.LogError(controllerName, userId, errorMessage);
        }
    }
}
