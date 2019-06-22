using System;

namespace MapOnlyExample.Service.ViewModel
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string RePassword { get; set; }

        public string Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; } 
    }
}