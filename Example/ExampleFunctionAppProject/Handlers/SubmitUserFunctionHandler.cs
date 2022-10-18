using ExampleFunctionAppProject.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using MockUserSource;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools;
using UserData = MockUserSource.User;

namespace ExampleFunctionAppProject
{

    /// <summary>
    /// The class that handles the performing of the function app logic, as well as error and validation failure.
    /// Is provided though dependency injection, any resources needed can be registered to DI in the `Startup` class and 
    /// added to the constructors parameters.
    /// The methods this class contains are free form and can be customized as requried.
    /// </summary>
    public class SubmitUserFunctionHandler : RequestHandlerBase<SubmitUserRequestBody>
    {
        private readonly IUserSource _UserSource;

        /// <summary>
        /// Constructor for the handler.
        /// </summary>
        /// <param name="userSource">A user source.</param>
        /// <param name="unhandledErrorFactory">Handler for unhandled errors.</param>
        /// <param name="log">The log writer to use when logging status messages.</param>
        public SubmitUserFunctionHandler(
            IUserSource userSource,
            IServiceProvider serviceProvider)
                : base(serviceProvider)
        {
            _UserSource = userSource ?? throw new ArgumentNullException(nameof(userSource));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> HandleRequest(FunctionRequestContext<SubmitUserRequestBody> context)
        {
            try
            {
                UserData user = new UserData
                {
                    Id = Guid.NewGuid(),
                    Name = context.RequestBody.Name,
                    Birthday = DateTime.UtcNow - TimeSpan.FromDays(365 * context.RequestBody.Age)   // Let's pretend this works.
                };

                _UserSource.AddUser(user);

                return new OkObjectResult(new SubmitUserResponseBody { UserId = user.Id });
            }
            catch (DivideByZeroException)
            {
                // This method should only catch and handle exception that require special handling. Bad requests inputs should already be handled, 
                // and the exception handling middleware will deal with any non-caught exceptions from this method and handle generically.
                return new BadRequestObjectResult(new MessageResponseBody { Message = "Why did you divide by zero?" });
            }
        }



        /// <inheritdoc />
        public override async Task<IActionResult> HandleBodyDeserializationError(FunctionRequestContext<SubmitUserRequestBody> context)
        {
            return new BadRequestObjectResult(new MessageResponseBody
            {
                Message = "An error occurred reading the request body."
            });
        }

        /// <inheritdoc />
        public override async Task<IActionResult> HandleBodyReadError(FunctionRequestContext<SubmitUserRequestBody> context)
        {
            return new BadRequestObjectResult(new MessageResponseBody
            {
                Message = "An error occurred deserializing the request body."
            });
        }

        /// <inheritdoc />
        public override async Task<IActionResult> HandleValidationError(FunctionRequestContext<SubmitUserRequestBody> context)
        {
            return new BadRequestObjectResult(new MessageResponseBody
            {
                Message = $"An error occurred validating the request."
            });
        }

        /// <inheritdoc />
        public override async Task<IActionResult> HandleValidationFailure(FunctionRequestContext<SubmitUserRequestBody> context)
        {
            return new BadRequestObjectResult(new ValidationFailureResponseBody
            {
                Issues = context.HeaderValidationResult.Issues
                    .Concat(context.QueryParameterValidationResult.Issues)
                    .ToArray(),
                Message = "Request validation failed."
            });
        }
    }
}
