using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ExampleFunctionAppProject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace ExampleFunctionApp.UnitTests
{
    internal class PreProcessorTestFixture
    {
        /// <summary>
        /// Given: An authenticated HTTP request
        /// When: AuthPreprocessor is triggered
        /// Then: Return A continue status
        /// </summary>
        [Test]
        public async Task AuthenticatedRequestUserPreprocessLetContinue()
        {
            HttpContext context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal();
            context.User.AddIdentity(new ClaimsIdentity(new List<Claim>(), "Authentication"));
            HttpRequest request = new DefaultHttpRequest(context);

            var preProcess = new AuthPreprocessor();
            var result = await preProcess.Process(request, new NullLogger<PreProcessorTestFixture>());


            Assert.AreEqual(result.ShouldContinue, true);
        }

        /// <summary>
        /// Given: A non authenticated HTTP request
        /// When: AuthPreprocessor is triggered
        /// Then: Do not continue
        /// </summary>
        [Test]
        public async Task AuthenticatedUserPreprocessStopExecution()
        {
            HttpContext context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal();
            HttpRequest request = new DefaultHttpRequest(context);

            var preProcess = new AuthPreprocessor();
            var result = await preProcess.Process(request, new NullLogger<PreProcessorTestFixture>());

            Assert.AreEqual(result.ShouldContinue, false);
        }
    }
}