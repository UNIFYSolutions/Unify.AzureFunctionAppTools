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
    /// Tests related to <see cref="GetUserFunctionHandler"/>.
    /// Note: Example code is not not exaustivly tested by this test fixture.
    /// </summary>
    [TestFixture]
    public class GetUserFunctionHandlerTestFixture
    {
        /// <summary>
        /// Test: User can be retrieved.
        /// </summary>
        [Test]
        public async void HandlerGetUserTest()
        {
            (GetUserFunctionHandler handler, Guid testUserId, Mock<IUserSource> userSourceMock) = GetSut();

            var context = new FunctionRequestContext
            {
                QueryParameter = new Dictionary<string, string[]> { [FunctionConstants.UserIdParamName] = new[] { testUserId.ToString() } }
            };

            IActionResult response = await handler.HandleRequest(context);

            // Check result type type.
            Assert.AreEqual(response.GetType().Name, nameof(OkObjectResult));

            // Get response body.
            var responseBody = (response as OkObjectResult)?.Value as GetUserResponseBody;

            // Check response.
            Assert.NotNull(responseBody);
            Assert.AreEqual(responseBody.User.Id, testUserId.ToString());
            Assert.AreEqual(responseBody.User.Name, "Jimmy");
            Assert.AreEqual(responseBody.User.Birthday, new DateTime(2020, 1, 1));

            // Verify call to user source was made.
            userSourceMock.Verify(m => m.GetUser(testUserId), Times.Once);
        }

        /// <summary>
        /// Test: Not found response is unknown user id is provided.
        /// </summary>
        [Test]
        public async void HandlerGetUnknownUserTest()
        {
            Guid searchId = Guid.NewGuid();

            (GetUserFunctionHandler handler, Guid testUserId, Mock<IUserSource> userSourceMock) = GetSut();
            
            var context = new FunctionRequestContext
            {
                QueryParameter = new Dictionary<string, string[]> { [FunctionConstants.UserIdParamName] = new[] { searchId.ToString() } }
            };

            IActionResult response = await handler.HandleRequest(context);

            // Check result type type.
            Assert.AreEqual(response.GetType().Name, nameof(NotFoundResult));

            // Verify call to user source was made, but only with the id being searched on.
            userSourceMock.Verify(m => m.GetUser(testUserId), Times.Never);
            userSourceMock.Verify(m => m.GetUser(searchId), Times.Once);
        }

        private (GetUserFunctionHandler, Guid, Mock<IUserSource>) GetSut()
        {
            var testUserId = Guid.NewGuid();
            var testUser = new User
            {
                Name = "Jimmy",
                Birthday = new DateTime(2020, 1, 1),
                Id = testUserId
            };

            var userSourceMock = new Mock<IUserSource>();
            userSourceMock.Setup(a => a.GetUser(testUserId)).Returns(() => testUser);

            return (
                new GetUserFunctionHandler(
                    userSourceMock.Object, 
                    Mock.Of<IUnhandledErrorFactory>(),
                    Mock.Of<HandlerPreprocessorCollection<GetUserFunctionHandler>>(),
                    Mock.Of<ILogger<GetUserFunctionHandler>>()),
                testUserId,
                userSourceMock
            );

        }
    }
}