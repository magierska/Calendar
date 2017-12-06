using DataAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalendarModel;

namespace Calendar
{
    public class MonthView : View
    {
        TableLayoutPanel bigPanel;
        Label title;
        DayManager dayManager;
        Color selectedDayBackgroundColor = Color.Lavender;
        Color daysFromDifferentMonthBackgroundColor = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(Color.Tomato)));
        Color normalDaysBackgroundColor = Color.White;

        public MonthView(TableLayoutPanel _bigPanel, Label _title, DayManager _dayManager)
        {
            bigPanel = _bigPanel;
            title = _title;
            dayManager = _dayManager;
            model = new MonthModel();
        }

        public override void ShowDays(CalendarModel.Day _selectedDay)
        {
            if (_selectedDay == null)
                _selectedDay = selectedDay;
            title.Text = $"{_selectedDay.MonthName} {_selectedDay.Year}";

            TableLayoutPanel panel = new TableLayoutPanel();
            View.SetDoubleBuffered(panel);
            panel.Dock = DockStyle.Fill;
            panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            panel.ColumnCount = 8;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
            for (int i = 0; i < panel.ColumnCount - 1; i++)
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panel.RowCount = (model as MonthModel).Days.Count + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            for (int i = 0; i < panel.RowCount; i++)
                panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            for (int i = 0; i < (model as MonthModel).Days.Count; i++)
            {
                Label label = new Label();
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.TopCenter;
                label.Text = (model as MonthModel).Days[i][6].WeekOfYear.ToString();
                label.Font = new Font("Arial", 10, FontStyle.Bold);
                label.ForeColor = daysAndHoursColor;
                panel.Controls.Add(label, 0, i + 1);
            }

            for (int i = 0; i < 7; i++)
            {
                Label label = new Label();
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Text = $"{(model as MonthModel).Days[0][i].DayOfWeek}";
                label.Font = new Font("Arial", 10, FontStyle.Bold);
                panel.Controls.Add(label, i + 1, 0);
            }

            if (bigPanel.Controls.Count >= 2)
                bigPanel.Controls.RemoveAt(1);
            bigPanel.Controls.Add(panel, 0, 1);

            for (int i = 0; i < (model as MonthModel).Days.Count; i++)
                for (int j = 0; j < (model as MonthModel).Days[i].Length; j++)
                {
                    if ((model as MonthModel).Days[i][j].Month != (model as MonthModel).Days[1][0].Month)
                        (model as MonthModel).Days[i][j].PictureBox.BackColor = daysFromDifferentMonthBackgroundColor;
                    else
                        (model as MonthModel).Days[i][j].PictureBox.BackColor = normalDaysBackgroundColor;
                    panel.Controls.Add((model as MonthModel).Days[i][j].PictureBox, j + 1, i + 1);
                    this.ShowDay((model as MonthModel).Days[i][j]);
                    if ((model as MonthModel).Days[i][j].DateEquals(_selectedDay))
                        this.SelectDay((model as MonthModel).Days[i][j]);
                }
        }

        public override void ShowDay(CalendarModel.Day day)
        {
            day.PictureBox.MouseClick += new MouseEventHandler(pictureBox_MouseClick);
            day.PictureBox.Paint += new PaintEventHandler(pictureBox_Paint);
            day.PictureBox.Image = new Bitmap(day.PictureBox.Width, day.PictureBox.Height);
            using (Graphics g = Graphics.FromImage(day.PictureBox.Image))
            {
                TextRenderer.DrawText(g, day.Number.ToString(), new Font("Arial", 12, FontStyle.Bold), new Point(0, 0), daysAndHoursColor, day.PictureBox.BackColor);
                int diff = 15;
                foreach (Event e in day.Events)
                {
                    TextRenderer.DrawText(g, e.ToString(), new Font("Arial", 8), new Rectangle(0, diff, day.PictureBox.Image.Width, diff), DataModel.IsEventAccepted(e.Id) ? e.Type.Color.Color : notAcceptedEventColor, day.PictureBox.BackColor, TextFormatFlags.WordEllipsis);
                    diff += 12;
                }
            }
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectedDay != (sender as PictureBox).Tag)
            {
                this.SelectDay((CalendarModel.Day)(sender as PictureBox).Tag);
            }
            dayManager.Show(selectedDay);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (((sender as PictureBox).Tag as CalendarModel.Day).DateEquals(selectedDay))
                ControlPaint.DrawBorder(e.Graphics, (sender as PictureBox).ClientRectangle, selectedDayColor, ButtonBorderStyle.Solid);
            else
                ControlPaint.DrawBorder(e.Graphics, (sender as PictureBox).ClientRectangle, normalDaysBackgroundColor, ButtonBorderStyle.None);
        }

        public override void SelectDay(CalendarModel.Day day)
        {
            if (selectedDay != null)
            {
                if (selectedDay.Month == (model as MonthModel).Days[1][0].Month)
                    selectedDay.PictureBox.BackColor = normalDaysBackgroundColor;
                else
                    selectedDay.PictureBox.BackColor = daysFromDifferentMonthBackgroundColor;
                selectedDay.PictureBox.Invalidate();
                this.ShowDay(selectedDay);
            }
            base.SelectDay(day);
            selectedDay.PictureBox.BackColor = selectedDayBackgroundColor;
            selectedDay.PictureBox.Invalidate();
            this.ShowDay(selectedDay);
        }
    }
}
