using System;

namespace ExampleFunctionAppProject
{
    /// <summary>
    /// Body model expected by the submit user function.
    /// </summary>
    public class SubmitUserRequestBody
    {
        /// <summary>
        /// Users name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Users age.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// When the user starts.
        /// </summary>
        public DateTimeOffset? Start { get; set; }
    }
}
