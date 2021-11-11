using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleFunctionAppProject.Models.Responses
{
    /// <summary>
    /// Response body for the get user function.
    /// </summary>
    public class GetUserResponseBody
    {
        public User User { get; set; }
    }
}
