using ExampleFunctionAppProject;
using ExampleFunctionAppProject.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockUserSource;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unify.AzureFunctionAppTools;
using Unify.AzureFunctionAppTools.ExceptionHandling;
using Unify.AzureFunctionAppTools.Preprocessing;

namespace ExampleFunctionApp.UnitTests
{
    /// <summary>
    /// Tests related to <see cref="SubmitUserFunctionHandler"/>.
    /// Note: Example code is not not exaustivly tested by this test fixture.
    /// </summary>
    [TestFixture]
    public class SubmitUserFunctionHandlerTestFixture
    {
        /// <summary>
        /// Test: User can be retrieved.
        /// </summary>
        [Test]
        public async void HandlerSubmitUserTest()
        {
            var context = new FunctionRequestContext<SubmitUserRequestBody>
            {
                RequestBody = new SubmitUserRequestBody
                {
                    Name = "Jimmy",
                    Age = 9,
                    Start = new DateTime(2000, 1, 1)
                }
            };

            (SubmitUserFunctionHandler handler, Mock<IUserSource> userSourceMock, List<User> addedUserList) = GetSut();
            IActionResult response = await handler.HandleRequest(context);

            // Check result type type.
            Assert.AreEqual(response.GetType().Name, nameof(OkObjectResult));

            // Get response body.
            var responseBody = (response as OkObjectResult)?.Value as SubmitUserResponseBody;

            // Check response.
            Assert.NotNull(responseBody);
            Assert.AreNotEqual(responseBody.UserId, Guid.Empty);

            // Check added users.
            Assert.AreEqual(addedUserList.Count, 1);
            Assert.AreEqual(addedUserList[0].Id, responseBody.UserId);
            Assert.AreEqual(addedUserList[0].Name, "Jimmy");
        }

        private (SubmitUserFunctionHandler, Mock<IUserSource> userSourceMock, List<User>) GetSut() 
        {
            var addedUsers = new List<User>();

            var userSourceMock = new Mock<IUserSource>();
            userSourceMock.Setup(m => m.AddUser(It.IsAny<User>())).Callback<User>(user => addedUsers.Add(user));

            return (
                new SubmitUserFunctionHandler(
                    userSourceMock.Object, 
                    Mock.Of<IUnhandledErrorFactory>(),
                    Mock.Of<HandlerPreprocessorCollection<SubmitUserFunctionHandler, SubmitUserRequestBody>>(), 
                    Mock.Of<ILogger<SubmitUserFunctionHandler>>()),
                userSourceMock,
                addedUsers
            );

        }
    }
}