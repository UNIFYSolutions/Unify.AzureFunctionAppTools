using ExampleFunctionAppProject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unify.AzureFunctionAppTools;

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
        /// Given: A Valid Query Parameter
        /// When: ValidateQueryParameters is triggered
        /// Then: Return A RequestValidationStatus.Passed result
        /// </summary>
        [Test]
        public void ValidQueryParamsValidationTest()
        {

            var contextFactory = new GetUserRequestContextFactory(
                HelperMethods.MockUnhandledErrorFactoryProvider());

            IDictionary<string, string[]> testParams = new Dictionary<string, string[]>
            {
                [FunctionConstants.UserIdParamName] = new[] { Guid.NewGuid().ToString() }
            };

            RequestValidationResult result = contextFactory.ValidateQueryParameters(testParams);

            Assert.AreEqual(result.Status, RequestValidationStatus.Passed);
        }

        /// <summary>
        /// Given: An invalid query parameter
        /// When: ValidateQueryParameters is triggered
        /// Then: Return A RequestValidationStatus.Failed result
        /// </summary>
        [Test]
        [TestCase("notAGuid", TestName = "InvalidUserIdQueryParamValidationTest_NotAGuid")]
        [TestCase("", TestName = "InvalidUserIdQueryParamValidationTest_EmptyString")]
        [TestCase(null, TestName = "InvalidUserIdQueryParamValidationTest_Null")]
        public void InvalidUserIdQueryParamValidationTest(string invalidValue)
        {

            var contextFactory = new GetUserRequestContextFactory(HelperMethods.MockUnhandledErrorFactoryProvider());

            IDictionary<string, string[]> testParams = new Dictionary<string, string[]>
            {
                [FunctionConstants.UserIdParamName] = new[] { invalidValue }
            };

            RequestValidationResult result = contextFactory.ValidateQueryParameters(testParams);

            Assert.AreEqual(result.Status, RequestValidationStatus.Failed);
        }
    }
}