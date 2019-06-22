using MapOnly;
using MapOnlyExample.Model;
using MapOnlyExample.Service.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapOnlyExample.Service
{
    public class UserService
    {
        public Dictionary<string,string> Errors { get; set; }

        public UserService()
        {
            Errors = new Dictionary<string, string>();
            // Init data
            var UserViewModel = new UserViewModel
            {
                Id = new Guid(),
                FirstName = "John",
                LastName = "Madam",
                UserName = "Jonh",
                Password = "1234567",
                RePassword = "1234567",
                Address = "Singapore",
                Birthday = new DateTime(2000, 1, 1),
                Email = "jonh@gmail.com",
                Gender = "Male"
            };
            this.Register(UserViewModel);
        }

        public bool Register(UserViewModel userViewModel)
        {
            #region Validate 
            Errors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(userViewModel.UserName))
            {
                Errors["UserName"] = "UserName can not empty!";
            }

            if (DbContext.Users.Any(x => x.UserName.ToLower() == userViewModel.UserName.ToLower()))
            {
                Errors["UserName"] = "UserName duplecate!";
            }

            if (string.IsNullOrEmpty(userViewModel.Password))
            {
                Errors["Password"] = "Password can not empty!";
            }

            if (userViewModel.Password != userViewModel.RePassword)
            {
                Errors["RePassword"] = "Password not match!";
            }

            if (string.IsNullOrEmpty(userViewModel.Gender))
            {
                Errors["Gender"] = "Gender can not empty!";
            }

            if (string.IsNullOrEmpty(userViewModel.Email))
            {
                Errors["Email"] = "Email can not empty!";
            }

            if (string.IsNullOrEmpty(userViewModel.FirstName))
            {
                Errors["FirstName"] = "FirstName can not empty!";
            }

            if (string.IsNullOrEmpty(userViewModel.LastName))
            {
                Errors["LastName"] = "LastName can not empty!";
            }

            if (Errors.Count() > 0) return false;
            #endregion

            var user = userViewModel.Map(new User());

            user.Id = Guid.NewGuid();
            user.IsActive = false;
            user.UpdatedDate = DateTime.Now;
            user.UpdatedUser = "Current User";
            user.CreatedDate = DateTime.Now;
            user.CreatedUser = "Current User";

            // do insert user into database
            DbContext.Users.Add(user);
            return true;
        }

        public UserDetailsViewModel GetUserDetails(Guid id)
        {
            // get user
            var user = DbContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) throw new ArgumentNullException();
            // return model
            var viewModel = user.Map(new UserDetailsViewModel());

            return viewModel;
        }

        public List<UserDetailsViewModel> GetUserListing(string username)
        {
            // get user, filter
            var users = DbContext.Users
                .Where(u => string.IsNullOrEmpty(username) || u.UserName.Contains(username));

            var userListing = users
                .Select(user => user.Map(new UserDetailsViewModel()))
                .ToList();

            return userListing;
        }
    }
}