using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess;
using CalendarModel;

namespace Calendar
{
    public class DayManager
    {
        Calendar calendar;
        int controlHeight = 30;
        CalendarModel.Day shownDay;
        int editedEventIndex;
        List<int> usersToInvite;
        Color notAcceptedEventColor = Color.Gray;

        public DayManager(Calendar _calendar)
        {
            calendar = _calendar;
        }

        public void Show(CalendarModel.Day day)
        {
            shownDay = day;
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.RowCount = day.Events.Count + 2;
            for (int i = 0; i < panel.RowCount; i++)
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label label = new Label();
            label.Dock = DockStyle.Fill;
            label.Height = controlHeight;
            label.Text = $"{day.Number} {day.MonthName} {day.Year}";
            panel.Controls.Add(label, 0, 0);

            for (int i = 0; i < day.Events.Count; i++)
            {
                Button button = new Button();
                button.Click += new EventHandler(button_Click);
                button.Dock = DockStyle.Fill;
                button.Height = controlHeight;
                button.Text = day.Events[i].ToString();
                button.ForeColor = DataModel.IsEventAccepted(day.Events[i].Id) ? day.Events[i].Type.Color.Color : notAcceptedEventColor;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.MouseOverBackColor = Color.Lavender;
                button.FlatAppearance.BorderSize = 0;
                button.Tag = i;
                panel.Controls.Add(button, 0, i + 1);
            }

            if (calendar.DayEventsTableLayoutPanel.Controls.Count >= 2)
                calendar.DayEventsTableLayoutPanel.Controls.RemoveAt(1);
            calendar.DayEventsTableLayoutPanel.Controls.Add(panel, 0, 1);
            calendar.RightTabControl.SelectedIndex = 1;
        }

        private void button_Click(object sender, EventArgs e)
        {
            editedEventIndex = (int)(sender as Button).Tag;
            this.ShowEventEditor(shownDay.Events[editedEventIndex]);
        }

        public void ShowEventEditor(Event myEvent = null)
        {
            while (calendar.PlansCheckedListBox.Items.Count > 0)
                calendar.PlansCheckedListBox.Items.RemoveAt(0);
            calendar.PlansCheckedListBox.Items.AddRange(DataModel.Plans.ToArray());

            calendar.ErrorProvider1.SetError(calendar.NameEventTextBox, "");
            calendar.ErrorProvider1.SetError(calendar.EndDateTimePicker, "");
            calendar.ErrorProvider1.SetError(calendar.TypeEventComboBox, "");

            usersToInvite = new List<int>();
            calendar.InvitedListView.Items.Clear();
            calendar.InvitedListView.Items.Add(new ListViewItem("me"));

            if (myEvent != null)
            {
                calendar.NameEventTextBox.Text = myEvent.Name;
                calendar.TypeEventComboBox.SelectedItem = DataModel.Types.FindAll(t => t.Id == myEvent.Type.Id).First();
                calendar.StartDateTimePicker.Value = myEvent.Start;
                calendar.EndDateTimePicker.Value = myEvent.End;
                List<Plan> plans = myEvent.Plans.ToList();
                foreach (Plan p in plans)
                {
                    for (int i = 0; i < calendar.PlansCheckedListBox.Items.Count; i++)
                        if ((calendar.PlansCheckedListBox.Items[i] as Plan).Id == p.Id)
                        {
                            calendar.PlansCheckedListBox.SetItemChecked(i, true);
                            break;
                        }
                }
                List<EventApproval> approvals = DataModel.EventApprovals.Where(ea => ea.EventId == myEvent.Id).ToList();
                foreach (EventApproval approval in approvals)
                {
                    if (approval.UserId != DataModel.ActiveUser.Id)
                        calendar.InvitedListView.Items.Add(new ListViewItem(DataModel.Users.Where(u => u.Id == approval.UserId).First().UserName));
                }
                if (DataModel.EventApprovals.Any(ea => ea.EventId == myEvent.Id && ea.UserId == DataModel.ActiveUser.Id && ea.Accepted))
                {
                    calendar.RespondInvitationButton.ImageList = calendar.CrossImageList;
                    calendar.RespondInvitationButton.ImageIndex = 1;
                    calendar.RespondInvitationButton.Text = "Not going";
                    calendar.RespondInvitationButton.Tag = true;
                }
                else
                {
                    calendar.RespondInvitationButton.ImageList = calendar.AcceptImageList;
                    calendar.RespondInvitationButton.ImageIndex = 1;
                    calendar.RespondInvitationButton.Text = "Going";
                    calendar.RespondInvitationButton.Tag = false;
                }
            }
            else
            {
                editedEventIndex = -1;
                calendar.NameEventTextBox.Text = string.Empty;
                Calendar.SetWatermarkTextBox(calendar.NameEventTextBox, "Please type here...");
                calendar.TypeEventComboBox.SelectedIndex = -1;
                calendar.StartDateTimePicker.Value = new DateTime(shownDay.Year, shownDay.Month, shownDay.Number);
                calendar.EndDateTimePicker.Value = new DateTime(shownDay.Year, shownDay.Month, shownDay.Number);
            }

            calendar.RightTabControl.SelectedIndex = 2;
        }

        public void AcceptEditedEvent()
        {
            if (!this.ValidateName() || !this.ValidateType() || !this.ValidateDates())
                return;
            if (editedEventIndex == -1)
                DataModel.AddEvent(
                    calendar.NameEventTextBox.Text,
                    calendar.TypeEventComboBox.SelectedItem as DataAccess.Type,
                    calendar.StartDateTimePicker.Value,
                    calendar.EndDateTimePicker.Value,
                    this.GetCheckedPlansId(),
                    usersToInvite,
                    (bool)calendar.RespondInvitationButton.Tag);
            else
                DataModel.EditEvent(
                    shownDay.Events[editedEventIndex],
                    calendar.NameEventTextBox.Text,
                    calendar.TypeEventComboBox.SelectedItem as DataAccess.Type,
                    calendar.StartDateTimePicker.Value,
                    calendar.EndDateTimePicker.Value,
                    this.GetCheckedPlansId(), 
                    usersToInvite,
                    (bool)calendar.RespondInvitationButton.Tag);

            shownDay.GetEvents();
            this.Show(shownDay);
        }

        private bool ValidateName()
        {
            if (calendar.NameEventTextBox.Text == string.Empty)
            {
                calendar.ErrorProvider1.SetError(calendar.NameEventTextBox, "Name cannot be empty");
                return false;
            }
            calendar.ErrorProvider1.SetError(calendar.NameEventTextBox, "");
            return true;
        }

        private bool ValidateType()
        {
            if (calendar.TypeEventComboBox.SelectedIndex == -1)
            {
                calendar.ErrorProvider1.SetError(calendar.TypeEventComboBox, "Type not selected");
                return false;
            }
            calendar.ErrorProvider1.SetError(calendar.TypeEventComboBox, "");
            return true;
        }

        private bool ValidateDates()
        {
            if (calendar.EndDateTimePicker.Value < calendar.StartDateTimePicker.Value)
            {
                calendar.ErrorProvider1.SetError(calendar.EndDateTimePicker, "End date cannot be before begin date");
                return false;
            }
            calendar.ErrorProvider1.SetError(calendar.EndDateTimePicker, "");
            return true;
        }
        private List<int> GetCheckedPlansId()
        {
            List<int> plansId = new List<int>();
            foreach (object item in calendar.PlansCheckedListBox.CheckedItems)
                plansId.Add((item as Plan).Id);
            return plansId;
        }

        public void DeleteEvent()
        {
            if (editedEventIndex == -1)
            {
                this.BackToPreviousPage();
                return;
            }
            DataModel.DeleteEvent(shownDay.Events[editedEventIndex]);
            shownDay.GetEvents();
            this.Show(shownDay);
        }

        public void BackToPreviousPage()
        {
            calendar.RightTabControl.SelectedIndex--;
        }

        public void InviteToEvent()
        {
            Invitation invitation = new Invitation(editedEventIndex == -1 ? null : shownDay.Events[editedEventIndex]);
            if (invitation.ShowDialog() == DialogResult.OK)
            {
                foreach (int id in invitation.UsersToInvite)
                {
                    calendar.InvitedListView.Items.Add(new ListViewItem(DataModel.Users.Where(u => u.Id == id).First().UserName));
                    if (!usersToInvite.Any(i => i == id))
                        usersToInvite.Add(id);
                }
            }
        }
    }
}
