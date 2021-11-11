using ExampleFunctionAppProject;
using Moq;
using NUnit.Framework;
using System;
using Unify.AzureFunctionAppTools;
using Unify.AzureFunctionAppTools.ExceptionHandling;
using Unify.AzureFunctionAppTools.Preprocessing;

namespace ExampleFunctionApp.UnitTests
{
    /// <summary>
    /// Tests related to <see cref="SubmitUserRequestContextFactory"/>.
    /// Note: Example code is not not exaustivly tested by this test fixture.
    /// </summary>
    [TestFixture]
    public class SubmitUserContextFactoryTestFixture
    {
        /// <summary>
        /// Test: Request body is correctly validated.
        /// </summary>
        [Test]
        public void ValidBodyValidationTest()
        {
            var contextFactory = new SubmitUserRequestContextFactory(
                Mock.Of<IUnhandledErrorFactory>(),
                Mock.Of<RequestPreprocessorCollection<SubmitUserRequestContextFactory>>());

            var testBody = new SubmitUserRequestBody
            {
                Name = "Jimmy",
                Age = 9,
                Start = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero)
            };

            RequestValidationResult result = contextFactory.ValidateBody(testBody);

            Assert.AreEqual(result.Status, RequestValidationStatus.Passed);
        }

        /// <summary>
        /// Test: Body fails validation when one is provided empty.
        /// </summary>
        [Test]
        public void InvalidUserIdQueryParamValidationTest()
        {
            var contextFactory = new SubmitUserRequestContextFactory(
                Mock.Of<IUnhandledErrorFactory>(),
                Mock.Of<RequestPreprocessorCollection<SubmitUserRequestContextFactory>>());

            var testBody = new SubmitUserRequestBody();

            RequestValidationResult result = contextFactory.ValidateBody(testBody);

            Assert.AreEqual(result.Status, RequestValidationStatus.Failed);
        }
    }
}