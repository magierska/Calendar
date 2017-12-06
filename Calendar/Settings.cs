using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess;
using CalendarModel;

namespace Calendar
{
    public partial class Settings : Form
    {
        Button currentHeaderButton;
        List<Plan> newPlans;

        public Settings()
        {
            InitializeComponent();

            while (plansCheckedListBox.Items.Count > 0)
                plansCheckedListBox.Items.RemoveAt(0);
            plansCheckedListBox.Items.AddRange(DataModel.Plans.ToArray());
            newPlans = DataModel.Plans;

            Calendar.SetWatermarkTextBox(newPlanTextBox, "Please type here...");

            currentHeaderButton = plansButton;

            mailAddressTextBox.Text = DataModel.EmailAddress;
            if (mailAddressTextBox.Text == string.Empty)
                Calendar.SetWatermarkTextBox(mailAddressTextBox, "Please type here...");

            plansButton.Click += new EventHandler(headerButton_Click);
            accountButton.Click += new EventHandler(headerButton_Click);
            addPlanButton.Click += new EventHandler(addPlanButton_Click);
            deletePlansButton.Click += new EventHandler(deletePlansButton_Click);
            acceptButton1.Click += new EventHandler(acceptButton1_Click);
            acceptButton2.Click += new EventHandler(acceptButton2_Click);
            cancelButton1.Click += new EventHandler(cancelButton1_Click);
            cancelButton2.Click += new EventHandler(cancelButton2_Click);
            acceptButton1.MouseEnter += new EventHandler(button_MouseEnter);
            acceptButton1.MouseLeave += new EventHandler(button_MouseLeave);
            acceptButton2.MouseEnter += new EventHandler(button_MouseEnter);
            acceptButton2.MouseLeave += new EventHandler(button_MouseLeave);
            cancelButton1.MouseEnter += new EventHandler(cancelButton_MouseEnter);
            cancelButton1.MouseLeave += new EventHandler(cancelButton_MouseLeave);
            cancelButton2.MouseEnter += new EventHandler(cancelButton_MouseEnter);
            cancelButton2.MouseLeave += new EventHandler(cancelButton_MouseLeave);
            addPlanButton.MouseEnter += new EventHandler(button_MouseEnter);
            addPlanButton.MouseLeave += new EventHandler(button_MouseLeave);
            deletePlansButton.MouseEnter += new EventHandler(button_MouseEnter);
            deletePlansButton.MouseLeave += new EventHandler(button_MouseLeave);

        }

        private void headerButton_Click(object sender, EventArgs e)
        {
            currentHeaderButton.ForeColor = Color.Black;
            currentHeaderButton = sender as Button;
            mainTabControl.SelectedIndex = currentHeaderButton.TabIndex;
            currentHeaderButton.ForeColor = Color.Purple;
        }

        private void addPlanButton_Click(object sender, EventArgs e)
        {
            if (newPlanTextBox.Text == string.Empty)
                errorProvider1.SetError(newPlanTextBox, "Name your plan");
            else
            {
                errorProvider1.SetError(newPlanTextBox, "");
                Plan newPlan = new Plan()
                {
                    Name = newPlanTextBox.Text
                };
                plansCheckedListBox.Items.Add(newPlan);
                newPlans.Add(newPlan);
                newPlanTextBox.Text = string.Empty;
            }
        }

        private void deletePlansButton_Click(object sender, EventArgs e)
        {
            List<int> indices = new List<int>();
            foreach (int index in plansCheckedListBox.CheckedIndices)
                indices.Insert(0, index);
            foreach (int index in indices)
            {
                plansCheckedListBox.Items.RemoveAt(index);
                newPlans.RemoveAt(index);
            }
        }

        private void acceptButton1_Click(object sender, EventArgs e)
        {
            DataModel.EditPlans(newPlans);
        }

        private void acceptButton2_Click(object sender, EventArgs e)
        {
            string address = mailAddressTextBox.Text;
            try
            {
                if (address == string.Empty)
                    throw new FormatException();
                address = new MailAddress(address).Address;
            }
            catch (FormatException)
            {
                errorProvider1.SetError(mailAddressTextBox, "Mail address is incorrect");
                return;
            }
            errorProvider1.SetError(mailAddressTextBox, "");
            DataModel.EditEmailAddress(address);
        }

        private void cancelButton1_Click(object sender, EventArgs e)
        {
            newPlanTextBox.Text = string.Empty;
            Calendar.SetWatermarkTextBox(newPlanTextBox, "Please type here...");
            errorProvider1.SetError(newPlanTextBox, "");
            while (plansCheckedListBox.Items.Count > 0)
                plansCheckedListBox.Items.RemoveAt(0);
            plansCheckedListBox.Items.AddRange(DataModel.Plans.ToArray());
            newPlans = DataModel.Plans;
        }

        private void cancelButton2_Click(object sender, EventArgs e)
        {
            errorProvider1.SetError(mailAddressTextBox, "");
            mailAddressTextBox.Text = DataModel.EmailAddress;
            if (mailAddressTextBox.Text == string.Empty)
                Calendar.SetWatermarkTextBox(mailAddressTextBox, "Please type here...");
        }

        private void button_MouseEnter(object sender, EventArgs e)
        {
            (sender as Button).ImageIndex = 0;
        }

        private void button_MouseLeave(object sender, EventArgs e)
        {
            (sender as Button).ImageIndex = 1;
        }

        private void cancelButton_MouseEnter(object sender, EventArgs e)
        {
            (sender as Button).ForeColor = Color.White;
        }

        private void cancelButton_MouseLeave(object sender, EventArgs e)
        {
            (sender as Button).ForeColor = Color.Purple;
        }
    }
}
