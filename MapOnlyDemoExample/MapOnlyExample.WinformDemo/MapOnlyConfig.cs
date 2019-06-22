using MapOnly;
using MapOnlyExample.Model;
using MapOnlyExample.Service.ViewModel;

namespace MapOnlyExample.WinformDemo
{
    public static class MapOnlyConfig
    {
        public static void Register()
        {
            //MapExtension.Create<User, UserViewModel>();
            //MapExtension.Create<User, UserDetailsViewModel>(); --> No need

            MapExtension.Create<UserViewModel, User>()
                .Ignore(u => u.CreatedDate)
                .Ignore(u => u.CreatedUser)
                .Ignore(u => u.IsActive)
                .Ignore(u => u.UpdatedDate)
                .Ignore(u => u.UpdatedUser);
        }
    }
}