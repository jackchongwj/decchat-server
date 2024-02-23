﻿
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

        public async Task LogError(string controllerName, string errorMessage)
        {
            var errorHandle = new ErrorHandle
            {
                ErrorMessage = errorMessage,
                ControlelrName = controllerName,
                Timestamp = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")),
        };

            await _collection.InsertOneAsync(errorHandle);
        }
    }
}