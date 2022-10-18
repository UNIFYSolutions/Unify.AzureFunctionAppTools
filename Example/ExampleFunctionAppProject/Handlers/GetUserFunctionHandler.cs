using ExampleFunctionAppProject.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockUserSource;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools;
using UserData = MockUserSource.User;
using UserModel = ExampleFunctionAppProject.Models.User;

namespace ExampleFunctionAppProject
{
    /// <summary>
    /// The class that handles the performing of the function app logic, as well as error and validation failure.
    /// Is provided though dependency injection, any resources needed can be registered to DI in the `Startup` class and 
    /// added to the constructors parameters.
    /// The methods this class contains are free form and can be customized as requried.
    /// </summary>
    public class GetUserFunctionHandler : RequestHandlerBase
    {
        private readonly IUserSource _UserSource;
        private readonly ILogger<GetUserFunctionHandler> _Log;

        /// <summary>
        /// Constructor for the handler.
        /// </summary>
        /// <param name="userSource">A user source.</param>
        /// <param name="unhandledErrorFactory">Handler for unhandled errors.</param>
        /// <param name="log">The log writer to use when logging status messages.</param>
        public GetUserFunctionHandler(IUserSource userSource,
            IServiceProvider serviceProvider,
            ILogger<GetUserFunctionHandler> log) : base(serviceProvider, true)
        {
            _Log = log ?? throw new ArgumentNullException(nameof(userSource));
            _UserSource = userSource ?? throw new ArgumentNullException(nameof(userSource));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> HandleRequest(FunctionRequestContext context)
        {
            try
            {
                var userId = new Guid(context.QueryParameter[FunctionConstants.UserIdParamName][0]);    // Safe, as already validated in the request reader.

                UserData user = _UserSource.GetUser(userId);

                if (user == null) return new NotFoundResult();

                return new OkObjectResult(new GetUserResponseBody
                {
                    User = new UserModel
                    {
                        Id = user.Id.ToString(),
                        Name = user.Name,
                        Birthday = user.Birthday
                    }
                });
            }
            catch (DivideByZeroException e)
            {
                // Handle any expected exception here. Any unexpected excpetion should be let through to be caught on the base
                // class and handled by the uncaught error handler. 
                _Log.LogError($"Error occured handling get user request: {e}");
                return new BadRequestObjectResult(new MessageResponseBody { Message = "Why did you divide by zero?" });
            }
        }


        /// <inheritdoc />
        public override async Task<IActionResult> HandleValidationError(FunctionRequestContext context)
        {
            return new BadRequestObjectResult(new MessageResponseBody
            {
                Message = $"An error occured validating the request."
            });
        }

        /// <inheritdoc />
        public override async Task<IActionResult> HandleValidationFailure(FunctionRequestContext context)
        {

            _Log.LogError($"An error occured validating the request. Exception: {context.Exception}");
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
