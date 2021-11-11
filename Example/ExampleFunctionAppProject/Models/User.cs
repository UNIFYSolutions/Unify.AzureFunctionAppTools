using System;

namespace ExampleFunctionAppProject.Models
{
    /// <summary>
    /// User model.
    /// </summary>
    public class User
    {        
        /// <summary>
        /// ID if the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The users birthday.
        /// </summary>
        public DateTime Birthday { get; set; }
    }
}
