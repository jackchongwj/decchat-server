using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomB_Backend_Test.ServiceTest
{
    public class ErrorHandleTest
    {
        private readonly Mock<IErrorHandleRepo> _mockErrorHandleRepo;
        private readonly ErrorHandleServices _service;

        public ErrorHandleTest()
        {
            _mockErrorHandleRepo = new Mock<IErrorHandleRepo>();

            _service = new ErrorHandleServices(_mockErrorHandleRepo.Object);
        }

        [Fact]
        public async Task LogError_CallsExpectedMethods()
        {
            string controllerName = "Friend";
            int userid = 1;
            string errorMessage = "Ivalid";

            _mockErrorHandleRepo.Setup(repo => repo.LogError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);


            await _service.LogError(controllerName, userid, errorMessage);

            _mockErrorHandleRepo.Verify(repo => repo.LogError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        

        }
    }
}
