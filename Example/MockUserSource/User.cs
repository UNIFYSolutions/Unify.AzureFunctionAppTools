using System;

namespace MockUserSource
{
    /// <summary>
    /// A user record for the mock user store.
    /// </summary>
    public class User
    {
        /// <summary>
        /// ID if the user.
        /// </summary>
        public Guid Id { get; set; }

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
