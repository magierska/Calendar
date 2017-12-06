using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess;
using CalendarModel;

namespace Calendar
{
    public partial class Calendar : Form
    {
        List<View> views;
        List<RightMonthView> rightViews;
        DayManager dayManager;
        int calendarSelectedIndex = 0;

        public Calendar()
        {
            InitializeComponent();

            OpeningLogIn opening = new OpeningLogIn();
            if (opening.ShowDialog() != DialogResult.OK)
            {
                this.Close();
                return;
            }

            userToolStripMenuItem.Text = DataModel.ActiveUser.UserName;

            dayManager = new DayManager(this);
            CalendarModel.Day.plansToolStripComboBox = plansToolStripComboBox;

            views = new List<View>();
            views.Add(new MonthView(bigMonthPageTableLayoutPanel, monthPageTitle, dayManager));
            views.Add(new WeekView(weekPageTableLayoutPanel, weekPageTitle, weekHeaderTableLayoutPanel, dayManager));
            views.Add(new DayView(dayPageTableLayoutPanel, dayPageTitle, dayManager));
            views[0].Load(new CalendarModel.Day(DateTime.Today));

            rightViews = new List<RightMonthView>();
            rightViews.Add(new RightMonthView(nextMonthTableLayoutPanel, nextMonthTitle));
            rightViews.Add(new RightMonthView(thisMonthTableLayoutPanel, thisMonthTitle));
            rightViews.Add(new RightMonthView(previousMonthTableLayoutPanel, previousMonthTitle));
            rightViews[0].Load(new CalendarModel.Day(DateTime.Today.AddMonths(1)));
            rightViews[1].Load(new CalendarModel.Day(DateTime.Today));
            rightViews[2].Load(new CalendarModel.Day(DateTime.Today.AddMonths(-1)));

            calendarsTabControl.SelectedIndexChanged += new EventHandler(calendarsTabControl_SelectedIndexChanged);
            this.SizeChanged += new EventHandler(form_SizeChanged);
            backButton1.Click += new EventHandler(backButton_Click);
            backButton2.Click += new EventHandler(backButton_Click);
            upButton.Click += new EventHandler(upButton_Click);
            downButton.Click += new EventHandler(downButton_Click);
            okEventButton.Click += new EventHandler(okEventButton_Click);
            okEventButton.MouseEnter += new EventHandler(button_MouseEnter);
            okEventButton.MouseLeave += new EventHandler(button_MouseLeave);
            addEventButton.MouseEnter += new EventHandler(button_MouseEnter);
            addEventButton.MouseLeave += new EventHandler(button_MouseLeave);
            deleteEventButton.MouseEnter += new EventHandler(button_MouseEnter);
            deleteEventButton.MouseLeave += new EventHandler(button_MouseLeave);
            backButton1.MouseEnter += new EventHandler(button_MouseEnter);
            backButton1.MouseLeave += new EventHandler(button_MouseLeave);
            backButton2.MouseEnter += new EventHandler(button_MouseEnter);
            backButton2.MouseLeave += new EventHandler(button_MouseLeave);
            rightDayButton.MouseEnter += new EventHandler(button_MouseEnter);
            rightDayButton.MouseLeave += new EventHandler(button_MouseLeave);
            rightWeekButton.MouseEnter += new EventHandler(button_MouseEnter);
            rightWeekButton.MouseLeave += new EventHandler(button_MouseLeave);
            rightMonthButton.MouseEnter += new EventHandler(button_MouseEnter);
            rightMonthButton.MouseLeave += new EventHandler(button_MouseLeave);
            leftDayButton.MouseEnter += new EventHandler(button_MouseEnter);
            leftDayButton.MouseLeave += new EventHandler(button_MouseLeave);
            leftWeekButton.MouseEnter += new EventHandler(button_MouseEnter);
            leftWeekButton.MouseLeave += new EventHandler(button_MouseLeave);
            leftMonthButton.MouseEnter += new EventHandler(button_MouseEnter);
            leftMonthButton.MouseLeave += new EventHandler(button_MouseLeave);
            downButton.MouseEnter += new EventHandler(button_MouseEnter);
            downButton.MouseLeave += new EventHandler(button_MouseLeave);
            upButton.MouseEnter += new EventHandler(button_MouseEnter);
            upButton.MouseLeave += new EventHandler(button_MouseLeave);
            inviteButton.MouseEnter += new EventHandler(button_MouseEnter);
            inviteButton.MouseLeave += new EventHandler(button_MouseLeave);
            respondInvitationButton.MouseEnter += new EventHandler(button_MouseEnter);
            respondInvitationButton.MouseLeave += new EventHandler(button_MouseLeave);
            addEventButton.Click += new EventHandler(addEventButton_Click);
            deleteEventButton.Click += new EventHandler(deleteEventButton_Click);
            plansToolStripComboBox.SelectedIndexChanged += new EventHandler(plansToolStripComboBox_IndexChanged);
            settingsToolStripMenuItem.Click += new EventHandler(settingsToolStripMenuItem_Click);
            logOffToolStripMenuItem.Click += new EventHandler(logOffToolStripMenuItem_Click);
            inviteButton.Click += new EventHandler(inviteButton_Click);
            respondInvitationButton.Click += new EventHandler(respondInvitationButton_Click);
            userToolStripMenuItem.DropDownOpened += new EventHandler(userToolStripMenuItem_DropDownOpened);
            userToolStripMenuItem.DropDownClosed += new EventHandler(userToolStripMenuItem_DropDownClosed);

            typeEventComboBox.DataSource = DataModel.Types;

            plansToolStripComboBox.Items.AddRange(DataModel.Plans.ToArray());
            plansToolStripComboBox.SelectedIndex = 0;
        }

        private void calendarsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            views[calendarsTabControl.SelectedIndex].Load(views[calendarSelectedIndex].SelectedDay);
            calendarSelectedIndex = calendarsTabControl.SelectedIndex;
            rightTabControl.SelectedIndex = 0;
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            foreach (RightMonthView rightView in rightViews)
                rightView.LoadPrevious();
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            foreach (RightMonthView rightView in rightViews)
                rightView.LoadNext();
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            views[calendarsTabControl.SelectedIndex].LoadNext();
            rightTabControl.SelectedIndex = 0;
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            views[calendarsTabControl.SelectedIndex].LoadPrevious();
            rightTabControl.SelectedIndex = 0;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void form_SizeChanged(object sender, EventArgs e)
        {
            views[calendarsTabControl.SelectedIndex].ShowDays();
        }

        private void addEventButton_Click(object sender, EventArgs e)
        {
            dayManager.ShowEventEditor();
        }

        private void okEventButton_Click(object sender, EventArgs e)
        {
            dayManager.AcceptEditedEvent();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            dayManager.BackToPreviousPage();
        }

        private void deleteEventButton_Click(object sender, EventArgs e)
        {
            dayManager.DeleteEvent();
        }

        private void plansToolStripComboBox_IndexChanged(object sender, EventArgs e)
        {
            views[calendarsTabControl.SelectedIndex].Load(views[calendarsTabControl.SelectedIndex].SelectedDay);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settingsForm = new Settings();
            settingsForm.ShowDialog();
            while (plansToolStripComboBox.Items.Count > 1)
                plansToolStripComboBox.Items.RemoveAt(plansToolStripComboBox.Items.Count - 1);
            plansToolStripComboBox.Items.AddRange(DataModel.Plans.ToArray());
        }

        private void logOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            OpeningLogIn opening = new OpeningLogIn();
            if (opening.ShowDialog() != DialogResult.OK)
            {
                this.Close();
                return;
            }

            userToolStripMenuItem.Text = DataModel.ActiveUser.UserName;

            plansToolStripComboBox.Items.Clear();
            plansToolStripComboBox.Items.Add("All plans");
            plansToolStripComboBox.Items.AddRange(DataModel.Plans.ToArray());
            plansToolStripComboBox.SelectedIndex = 0;
            CalendarModel.Day.plansToolStripComboBox = plansToolStripComboBox;

            views[0].Load(new CalendarModel.Day(DateTime.Today));
            rightViews[0].Load(new CalendarModel.Day(DateTime.Today.AddMonths(1)));
            rightViews[1].Load(new CalendarModel.Day(DateTime.Today));
            rightViews[2].Load(new CalendarModel.Day(DateTime.Today.AddMonths(-1)));

            this.Show();
        }

        private void inviteButton_Click(object sender, EventArgs e)
        {
            dayManager.InviteToEvent();
        }

        private void respondInvitationButton_Click(object sender, EventArgs e)
        {
            if (respondInvitationButton.Text == "Going")
            {
                respondInvitationButton.ImageList = crossImageList;
                respondInvitationButton.ImageIndex = 0;
                respondInvitationButton.Text = "Not going";
                respondInvitationButton.Tag = true;
            }
            else
            {
                respondInvitationButton.ImageList = acceptImageList;
                respondInvitationButton.ImageIndex = 0;
                respondInvitationButton.Text = "Going";
                respondInvitationButton.Tag = false;
            }
        }

        public void button_MouseEnter(object sender, EventArgs e)
        {
            (sender as Button).ImageIndex = 0;
            (sender as Button).ForeColor = Color.White;
        }

        public void button_MouseLeave(object sender, EventArgs e)
        {
            (sender as Button).ImageIndex = 1;
            (sender as Button).ForeColor = Color.Purple;
        }

        private void userToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            userToolStripMenuItem.ForeColor = Color.Tomato;
        }

        private void userToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            userToolStripMenuItem.ForeColor = Color.White;
        }

        public TabControl RightTabControl
        {
            get
            {
                return rightTabControl;
            }
        }

        public TableLayoutPanel DayEventsTableLayoutPanel
        {
            get
            {
                return dayEventsTableLayoutPanel;
            }
        }

        public TableLayoutPanel EventEditorTableLayoutPanel
        {
            get
            {
                return eventEditorTableLayoutPanel;
            }
        }

        public TextBox NameEventTextBox
        {
            get
            {
                return nameEventTextBox;
            }
        }

        public ComboBox TypeEventComboBox
        {
            get
            {
                return typeEventComboBox;
            }
        }

        public DateTimePicker StartDateTimePicker
        {
            get
            {
                return startDateTimePicker;
            }
        }

        public DateTimePicker EndDateTimePicker
        {
            get
            {
                return endDateTimePicker;
            }
        }

        public TableLayoutPanel PlansTableLayoutPanel
        {
            get
            {
                return plansTableLayoutPanel;
            }
        }

        public ErrorProvider ErrorProvider1
        {
            get
            {
                return errorProvider1;
            }
        }

        public ImageList AcceptImageList
        {
            get
            {
                return acceptImageList;
            }
        }

        public CheckedListBox PlansCheckedListBox
        {
            get
            {
                return plansCheckedListBox;
            }
        }

        public ListView InvitedListView
        {
            get
            {
                return invitedListView;
            }
        }

        public Button RespondInvitationButton
        {
            get
            {
                return respondInvitationButton;
            }
        }

        public ImageList CrossImageList
        {
            get
            {
                return crossImageList;
            }
        }

        private void CalendarClosing(object sender, FormClosingEventArgs e)
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                db.SaveChanges();
                db.Dispose();
            }
        }

        public static void SetWatermarkTextBox(TextBox textBox, string text)
        {
            SendMessage(textBox.Handle, 0x1501, 1, text);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
    }
}
