using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess;
using CalendarModel;

namespace Calendar
{
    public partial class Invitation : Form
    {
        Event myEvent;
        public List<int> UsersToInvite { get; private set; }
        public Invitation(Event _myEvent = null)
        {
            InitializeComponent();

            DialogResult = DialogResult.No;

            myEvent = _myEvent;
            if (myEvent == null)
                this.Text = "New event";
            else
                this.Text = myEvent.ToString();

            UsersToInvite = new List<int>();

            usersDataGridView.RowTemplate.Height = 30;

            usersDataGridView.CellClick += new DataGridViewCellEventHandler(checkBox_Click);
            searchTextBox.TextChanged += new EventHandler(searchTextBox_TextChanged);
            inviteButton.Click += new EventHandler(inviteButton_Click);
            inviteButton.MouseEnter += new EventHandler(inviteButton_MouseEnter);
            inviteButton.MouseLeave += new EventHandler(inviteButton_MouseLeave);

        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string search = searchTextBox.Text;
            List<User> users = DataModel.Users.Where(u => u.UserName.StartsWith(search) || DataModel.EmailAddresses.Where(ea => ea.Id == u.MailId).First().Address.StartsWith(search)).ToList();
            usersDataGridView.RowCount = users.Count;
            for (int i = 0; i < users.Count; i++)
            {
                usersDataGridView.Rows[i].Tag = users[i].Id;
                usersDataGridView.Rows[i].Cells["Username"].Value = users[i].UserName;
                usersDataGridView.Rows[i].Cells["Email"].Value = DataModel.EmailAddresses.Where(ev => ev.Id == users[i].MailId).First().Address;
                DataGridViewCheckBoxCell cell = usersDataGridView.Rows[i].Cells["CheckBoxes"] as DataGridViewCheckBoxCell;
                if (DataModel.ActiveUser.Id != users[i].Id && (myEvent == null || !DataModel.EventApprovals.Any(ev => ev.EventId == myEvent.Id && ev.UserId == users[i].Id)))
                {
                    cell.Value = false;
                }
                else
                {
                    cell.Value = true;
                    cell.ReadOnly = true;
                    cell.FlatStyle = FlatStyle.Flat;
                    cell.Style.ForeColor = Color.DarkGray;
                }
            }
        }

        private void checkBox_Click(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;
            DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)usersDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.ReadOnly)
                return;
            if ((bool)cell.Value)
            {
                cell.Value = false;
                UsersToInvite.Remove((int)usersDataGridView.Rows[e.RowIndex].Tag);
            }
            else
            {
                cell.Value = true;
                UsersToInvite.Add((int)usersDataGridView.Rows[e.RowIndex].Tag);
            }
        }

        private void inviteButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void inviteButton_MouseEnter(object sender, EventArgs e)
        {
            inviteButton.ImageIndex = 0;
        }

        private void inviteButton_MouseLeave(object sender, EventArgs e)
        {
            inviteButton.ImageIndex = 1;
        }
    }
}
