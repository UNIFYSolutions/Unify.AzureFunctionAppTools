using ExampleFunctionAppProject;
using ExampleFunctionAppProject.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using MockUserSource;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools;

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
        /// Given: A valid User
        /// When: That user is submitted
        /// Then: Ensure they've been properly added
        /// </summary>
        [Test]
        public async Task HandlerSubmitUserTest()
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

            (SubmitUserFunctionHandler handler, Mock<IUserSource> userSourceMock, List<User> addedUserList) = HelperMethods.GetSut();
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
    }
}