using ExampleFunctionAppProject;
using ExampleFunctionAppProject.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using MockUserSource;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools;

namespace ExampleFunctionApp.UnitTests
{
    /// <summary>
    /// Tests related to <see cref="GetUserFunctionHandler"/>.
    /// Note: Example code is not not exhaustively tested by this test fixture.
    /// </summary>
    [TestFixture]
    public class GetUserFunctionHandlerTestFixture
    {
        /// <summary>
        /// Given: A fake user
        /// When: That user is requested by a correct ID
        /// Then: Ensure the retrieved information is correct
        /// </summary>
        [Test]
        public async Task HandlerGetUserTest()
        {
            (Guid testUserId, IUserSource userSourceMock) = HelperMethods.MakeFakeUserSource();

            var functionHandler = new GetUserFunctionHandler(userSourceMock, HelperMethods.MockUnhandledErrorFactoryProvider(), new NullLogger<GetUserFunctionHandler>());

            var context = new FunctionRequestContext
            {
                QueryParameter = new Dictionary<string, string[]> { [FunctionConstants.UserIdParamName] = new[] { testUserId.ToString() } }
            };

            var response = await functionHandler.HandleRequest(context);

            // Check result type type.
            Assert.AreEqual(response.GetType().Name, nameof(OkObjectResult));

            // Get response body.
            var responseBody = (response as OkObjectResult)?.Value as GetUserResponseBody;

            // Check response.
            Assert.NotNull(responseBody);
            Assert.AreEqual(responseBody.User.Id, testUserId.ToString());
            Assert.AreEqual(responseBody.User.Name, "Jimmy");
            Assert.AreEqual(responseBody.User.Birthday, new DateTime(2020, 1, 1));
        }

        /// <summary>
        /// Given: A fake user
        /// When: That user is requested by an incorrect ID.
        /// Then: Not Found result is returned
        /// </summary>
        [Test]
        public async Task HandlerGetUnknownUserTest()
        {
            Guid searchId = Guid.NewGuid();

            (Guid _, IUserSource userSourceMock) = HelperMethods.MakeFakeUserSource();

            var functionHandler = new GetUserFunctionHandler(userSourceMock, HelperMethods.MockUnhandledErrorFactoryProvider(), new NullLogger<GetUserFunctionHandler>());

            var context = new FunctionRequestContext
            {
                QueryParameter = new Dictionary<string, string[]> { [FunctionConstants.UserIdParamName] = new[] { searchId.ToString() } }
            };

            var response = await functionHandler.HandleRequest(context);

            // Check result type type.
            Assert.AreEqual(response.GetType().Name, nameof(NotFoundResult));
        }
    }
}