using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalendarModel;
using System.Net.Mail;


namespace Calendar
{
    public partial class OpeningLogIn : Form
    {
        Button currentHeaderButton;

        public OpeningLogIn()
        {
            InitializeComponent();

            this.DialogResult = DialogResult.No;

            currentHeaderButton = loginButton;

            loginPasswordTextBox.PasswordChar = '\u2022';
            signupPasswordTextBox1.PasswordChar = '\u2022';
            signupPasswordTextBox2.PasswordChar = '\u2022';

            loginButton.Click += new EventHandler(headerButton_Click);
            signupButton.Click += new EventHandler(headerButton_Click);
            loginAcceptButton.Click += new EventHandler(loginAccept_Click);
            signupAcceptButton.Click += new EventHandler(signupAccept_Click);
            loginAcceptButton.MouseEnter += new EventHandler(button_MouseEnter);
            loginAcceptButton.MouseLeave += new EventHandler(button_MouseLeave);
            signupAcceptButton.MouseEnter += new EventHandler(button_MouseEnter);
            signupAcceptButton.MouseLeave += new EventHandler(button_MouseLeave);
            signupEmailTextBox.LostFocus += new EventHandler(signupEmailTextBox_LostFocus);
            signupPasswordTextBox1.LostFocus += new EventHandler(signupPasswordTextBox_LostFocus);
            signupPasswordTextBox2.LostFocus += new EventHandler(signupPasswordTextBox_LostFocus);

        }

        private void headerButton_Click(object sender, EventArgs e)
        {
            currentHeaderButton.ForeColor = Color.Black;
            currentHeaderButton = sender as Button;
            mainTabControl.SelectedIndex = currentHeaderButton.TabIndex;
            currentHeaderButton.ForeColor = Color.Purple;
        }

        private void loginAccept_Click(object sender, EventArgs e)
        {
            if (DataModel.LogIn(loginUsernameTextBox.Text, loginPasswordTextBox.Text))
            {
                this.errorProvider1.SetError(loginPasswordTextBox, "");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.errorProvider1.SetError(loginPasswordTextBox, "Username or password is incorrect!");
            }
        }

        private void signupAccept_Click(object sender, EventArgs e)
        {
            if (!ValidateUsernameTextBox())
                return;
            if (!ValidateEmailTextBox())
                return;
            if (!ValidatePasswordsTextBoxes())
                return;
            if (DataModel.SignUp(signupUsernameTextBox.Text, signupEmailTextBox.Text, signupPasswordTextBox2.Text))
            {
                this.errorProvider1.SetError(signupUsernameTextBox, "");
                this.errorProvider1.SetError(signupEmailTextBox, "");
                this.errorProvider1.SetError(signupPasswordTextBox2, "");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.errorProvider1.SetError(signupUsernameTextBox, "This username or email is already in use!");
            }
        }

        private bool ValidateUsernameTextBox()
        {
            if (signupUsernameTextBox.Text == string.Empty)
            {
                this.errorProvider1.SetError(signupUsernameTextBox, "Password cannot be empty!");
                return false;
            }
            this.errorProvider1.SetError(signupUsernameTextBox, "");
            return true;
        }

        private void signupEmailTextBox_LostFocus(object sender, EventArgs e)
        {
            ValidateEmailTextBox();
        }

        private bool ValidateEmailTextBox()
        {
            string email = signupEmailTextBox.Text;
            try
            {
                if (email == string.Empty)
                    throw new FormatException();
                email = new MailAddress(email).Address;
            }
            catch (FormatException)
            {
                this.errorProvider1.SetError(signupEmailTextBox, "Email address is not correct!");
                return false;
            }
            this.errorProvider1.SetError(signupEmailTextBox, "");
            return true;
        }

        private void signupPasswordTextBox_LostFocus(object sender, EventArgs e)
        {
            ValidatePasswordsTextBoxes();
        }

        private bool ValidatePasswordsTextBoxes()
        {
            if (signupPasswordTextBox1.Text != signupPasswordTextBox2.Text)
            {
                this.errorProvider1.SetError(signupPasswordTextBox2, "Passwords do not match!");
                return false;
            }
            if (signupPasswordTextBox1.Text == string.Empty)
            {
                this.errorProvider1.SetError(signupPasswordTextBox2, "Password cannot be empty!");
                return false;
            }
            this.errorProvider1.SetError(signupPasswordTextBox2, "");
            return true;
        }

        private void button_MouseEnter(object sender, EventArgs e)
        {
            (sender as Button).ImageIndex = 0;
        }

        private void button_MouseLeave(object sender, EventArgs e)
        {
            (sender as Button).ImageIndex = 1;
        }
    }
}
