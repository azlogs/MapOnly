using System;
using System.Runtime.Remoting.Messaging;
using MapOnly;

namespace MapOnlyExample.Service.ViewModel
{
    public class UserDetailsViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        
        public string Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [Map(Ignored = true)]
        public string BirthdayDisplay {
            get
            {
                if (Birthday == null) return string.Empty;

                return Birthday.Value.ToString("dd/MM/yyyy");
            }
        }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Map(Ignored = true)]
        public string DisplayName => FirstName + " " + LastName; 

        public string Address { get; set; }
    }
}