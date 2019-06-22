using MapOnlyExample.Service;
using MapOnlyExample.Service.ViewModel;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MapOnlyExample.WinformDemo
{
    public partial class fmMain : Form
    {
        private UserService userService;

        public fmMain()
        {
            userService = new UserService();
            InitializeComponent();
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            dgUser.AutoGenerateColumns = false;
            dgUser.DataSource = userService.GetUserListing(string.Empty);
            dgUser.Refresh();
        }

        private void btnAddnew_Click(object sender, EventArgs e)
        {
            var UserViewModel = new UserViewModel
            {
                Id = new Guid(),
                FirstName = txtFirstname.Text,
                LastName = txtLastname.Text,
                UserName = txtUserName.Text,
                Password = txtPassword.Text,
                RePassword = txtRePassword.Text,
                Address = txtAddress.Text,
                Birthday = dtpBirthday.Value,
                Email = txtEmail.Text,
                Gender = cbGender.Text
            };

            if (userService.Register(UserViewModel))
            {
                dgUser.DataSource = userService.GetUserListing(string.Empty);
                dgUser.Refresh();
                Reset();
            }
            else
            {
                MessageBox.Show(userService.Errors.First().Value);
            }
        }

        private void Reset()
        {
            txtFirstname.Text = "";
            txtLastname.Text = "";
            txtUserName.Text = "";
            txtPassword.Text = "";
            txtRePassword.Text = "";
            txtAddress.Text = "";
            dtpBirthday.Value = DateTime.Now;
            txtEmail.Text = "";
            cbGender.Text = "";
        }

        private void btnClearForm_Click(object sender, EventArgs e)
        {
            Reset();
        } 
    }
}