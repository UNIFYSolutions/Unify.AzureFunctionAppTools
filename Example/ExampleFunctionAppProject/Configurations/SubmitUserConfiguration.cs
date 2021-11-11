using System;

namespace ExampleFunctionAppProject.Configurations
{
    /// <summary>
    /// Configuration for <see cref="SubmitUserRequestContextFactory"/> and <see cref="SubmitUserFunctionHandler"/>
    /// </summary>
    public class SubmitUserConfiguration
    {
        /// <summary>
        /// The minimum allowed date.
        /// </summary>
        public DateTime MinStart { get; set; }
    }
}
