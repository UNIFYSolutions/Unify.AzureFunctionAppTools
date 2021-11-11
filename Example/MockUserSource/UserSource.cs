using System;
using System.Collections.Generic;

namespace MockUserSource
{
    /// <summary>
    /// A mock user source that supports adding and fetching by id.
    /// </summary>
    public class UserSource : IUserSource
    {
        private IDictionary<Guid, User> _Users = new Dictionary<Guid, User>();

        /// <inheritdoc />
        public void AddUser(User user)
        {
            _Users[user.Id] = user;
        }

        /// <inheritdoc />
        public User GetUser(Guid id)
        {
            _Users.TryGetValue(id, out User user);
            return user;
        }
    }
}
