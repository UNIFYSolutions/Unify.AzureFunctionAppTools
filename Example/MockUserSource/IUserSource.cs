using System;

namespace MockUserSource
{
    /// <summary>
    /// A mock user source that supports adding and fetching by id.
    /// </summary>
    public interface IUserSource
    {
        /// <summary>
        /// Add a user.
        /// </summary>
        /// <param name="user">The user to add.</param>
        void AddUser(User user);

        /// <summary>
        /// Get a user.
        /// </summary>
        /// <param name="id">Id of the user to add.</param>
        /// <returns>The user.</returns>
        User GetUser(Guid id);
    }
}
