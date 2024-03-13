
using ChatroomB_Backend.Models;
using MongoDB.Driver;

namespace ChatroomB_Backend.Repository
{
    public class ErrorHandleRepo : IErrorHandleRepo
    {

        private readonly IMongoCollection<ErrorHandle> _collection;

        public ErrorHandleRepo(IMongoCollection<ErrorHandle> collection)
        {
            _collection = collection;
        }

        public async Task LogError(string controllerName, int userId, string errorMessage)
        {
            try
            {
                var errorHandle = new ErrorHandle
                {
                    ErrorMessage = errorMessage,
                    ControllerName = controllerName,
                    UserId = userId,
                    Timestamp = DateTime.Now,
                };

                await _collection.InsertOneAsync(errorHandle);
            }
            catch 
            {
                throw new Exception("Failed to log error to MongoDB");
            }
            
        }
    }
}
