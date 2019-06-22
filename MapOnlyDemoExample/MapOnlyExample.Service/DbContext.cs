using MapOnlyExample.Model;
using System.Collections.Generic;

namespace MapOnlyExample.Service
{
    public static class DbContext
    {
        private static List<User> _users { get; set; }

        public static List<User> Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new List<User>();
                }

                return _users;
            }
        }
    }
}