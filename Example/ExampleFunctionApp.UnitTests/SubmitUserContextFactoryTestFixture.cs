using ExampleFunctionAppProject;
using ExampleFunctionAppProject.Configurations;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using Unify.AzureFunctionAppTools;

namespace ExampleFunctionApp.UnitTests
{
    /// <summary>
    /// Tests related to <see cref="SubmitUserRequestContextFactory"/>.
    /// Note: Example code is not not exhaustively tested by this test fixture.
    /// </summary>
    [TestFixture]
    public class SubmitUserContextFactoryTestFixture
    {
        /// <summary>
        /// Test: Request body is correctly validated.
        /// Given: A Valid Body
        /// When: That body is validated
        /// Then: Return a RequestValidationStatus.Passed result
        /// </summary>
        [Test]
        public void ValidBodyValidationTest()
        {
            //Pass in Minimum date
            var settings = new SubmitUserConfiguration()
            {
                MinStart = DateTime.Parse("1/01/1950 12:00:00 AM")
            };

            IOptions<SubmitUserConfiguration> appSettingsOptions = Options.Create(settings);
            var contextFactory = new SubmitUserRequestContextFactory(appSettingsOptions, HelperMethods.MockUnhandledErrorFactoryProvider());

            var testBody = new SubmitUserRequestBody
            {
                Name = "Jimmy",
                Age = 9,
                Start = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero)
            };

            var result = contextFactory.ValidateBody(testBody);

            Assert.AreEqual(result.Status, RequestValidationStatus.Passed);
        }

        /// <summary>
        /// Given: An invalid, empty Body
        /// When: That body is validated
        /// Then: Return a RequestValidationStatus.Failed result
        /// </summary>
        [Test]
        public void InvalidUserIdQueryParamValidationTest()
        {
            var contextFactory = new SubmitUserRequestContextFactory(Mock.Of<IOptions<SubmitUserConfiguration>>(), HelperMethods.MockUnhandledErrorFactoryProvider());
            var testBody = new SubmitUserRequestBody();

            RequestValidationResult result = contextFactory.ValidateBody(testBody);
            Assert.AreEqual(result.Status, RequestValidationStatus.Failed);
        }

        /// <summary>
        /// Given: A User inputted below the MinStart time
        /// When: That body is validated
        /// Then: Return a RequestValidationStatus.Failed result
        /// </summary>
        [Test]
        public void InputUserBelowMinStart()
        {
            //Pass in Minimum date
            var settings = new SubmitUserConfiguration
            {
                MinStart = DateTime.Parse("1/01/1950 12:00:00 AM")
            };

            IOptions<SubmitUserConfiguration> appSettingsOptions = Options.Create(settings);
            var contextFactory = new SubmitUserRequestContextFactory(appSettingsOptions, HelperMethods.MockUnhandledErrorFactoryProvider());

            var testBody = new SubmitUserRequestBody
            {
                Name = "Genghis Khan",
                Age = 45,
                Start = new DateTimeOffset(1162, 8, 25, 0, 0, 0, TimeSpan.Zero)
            };

            var result = contextFactory.ValidateBody(testBody);
            Assert.AreEqual(result.Status, RequestValidationStatus.Failed);
        }
    }
}