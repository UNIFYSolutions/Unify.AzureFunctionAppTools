using ExampleFunctionAppProject;
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
    /// Tests related to <see cref="GetUserRequestContextFactory"/>.
    /// Note: Example code is not not exaustivly tested by this test fixture.
    /// </summary>
    [TestFixture]
    public class GetUserContextFactoryTestFixture
    {
        /// <summary>
        /// Test: Query parameters are correctly validated.
        /// </summary>
        [Test]
        public void ValidQueryParamsValidationTest()
        {
            var contextFactory = new GetUserRequestContextFactory(
                Mock.Of<IUnhandledErrorFactory>(),
                Mock.Of<RequestPreprocessorCollection<GetUserRequestContextFactory>>());

            IDictionary<string, string[]> testParams = new Dictionary<string, string[]>
            {
                [FunctionConstants.UserIdParamName] = new[] { Guid.NewGuid().ToString() }
            };

            RequestValidationResult result = contextFactory.ValidateQueryParameters(testParams);

            Assert.AreEqual(result.Status, RequestValidationStatus.Passed);
        }

        /// <summary>
        /// Test: User id query parameter fails validation when it is provided as an invalid format.
        /// </summary>
        [Test]
        [TestCase("notAGuid", TestName = "InvalidUserIdQueryParamValidationTest_NotAGuid")]
        [TestCase("", TestName = "InvalidUserIdQueryParamValidationTest_EmptyString")]
        [TestCase(null, TestName = "InvalidUserIdQueryParamValidationTest_Null")]
        public void InvalidUserIdQueryParamValidationTest(string invalidValue)
        {
            var contextFactory = new GetUserRequestContextFactory(
                Mock.Of<IUnhandledErrorFactory>(),
                Mock.Of<RequestPreprocessorCollection<GetUserRequestContextFactory>>());

            IDictionary<string, string[]> testParams = new Dictionary<string, string[]>
            {
                [FunctionConstants.UserIdParamName] = new[] { invalidValue }
            };

            RequestValidationResult result = contextFactory.ValidateQueryParameters(testParams);

            Assert.AreEqual(result.Status, RequestValidationStatus.Failed);
        }
    }
}