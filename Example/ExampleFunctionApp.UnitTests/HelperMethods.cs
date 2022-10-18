using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using ExampleFunctionAppProject;
using MockUserSource;
using Unify.AzureFunctionAppTools.ExceptionHandling;

namespace ExampleFunctionApp.UnitTests
{
    /// <summary>
    /// Helper methods to assist in the creation of user sources, Error factories, Etc.
    /// Used across meay tests
    /// </summary>
    internal static class HelperMethods
    {
        public static IServiceProvider MockUnhandledErrorFactoryProvider()
        {
            var services = new ServiceCollection();
            var mockError = Mock.Of<IUnhandledErrorFactory>();
            services.AddSingleton(mockError);
            services.AddLogging();
            return services.BuildServiceProvider();
        }


        /// <summary>
        /// Makes a fake user source for testing
        /// </summary>
        public static (Guid, IUserSource) MakeFakeUserSource()
        {
            var testUserId = Guid.NewGuid();

            var testUser = new User
            {
                Name = "Jimmy",
                Birthday = new DateTime(2020, 1, 1),
                Id = testUserId
            };

            IUserSource userSource = new UserSource();
            userSource.AddUser(testUser);


            return (
                testUserId,
                userSource
            );
        }

        /// <summary>
        /// Creates a UserFunctionHandler, and a mocked user source, used for testing.
        /// </summary>
        public static (SubmitUserFunctionHandler, Mock<IUserSource> userSourceMock, List<User>) GetSut()
        {
            var addedUsers = new List<User>();

            var userSourceMock = new Mock<IUserSource>();
            userSourceMock.Setup(m => m.AddUser(It.IsAny<User>())).Callback<User>(user => addedUsers.Add(user));

            return (
                new SubmitUserFunctionHandler(userSourceMock.Object, HelperMethods.MockUnhandledErrorFactoryProvider()),
                userSourceMock,
                addedUsers
            );

        }

    }
}
