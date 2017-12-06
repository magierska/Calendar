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
    class WeekView : View
    {
        TableLayoutPanel panel, headerPanel;
        Label title;
        int cellHeight;
        DayManager dayManager;
        PictureBox hoursPictureBox;

        public WeekView(TableLayoutPanel _panel, Label _title, TableLayoutPanel _headerPanel, DayManager _dayManager)
        {
            panel = _panel;
            headerPanel = _headerPanel;
            title = _title;
            dayManager = _dayManager;
            model = new WeekModel();

            hoursPictureBox = new PictureBox();
            hoursPictureBox.Dock = DockStyle.Fill;
            panel.Controls.Add(hoursPictureBox, 0, 0);
            cellHeight = hoursPictureBox.Height / 24;
            hoursPictureBox.Paint += new PaintEventHandler(hoursPictureBox_OnPaint);
        }

        private void hoursPictureBox_OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            TextRenderer.DrawText(g, "0", new Font("Arial", 12), new Point(0, 0), daysAndHoursColor, Color.White);
            for (int i = 1; i < 24; i++)
            {
                TextRenderer.DrawText(g, i.ToString(), new Font("Arial", 12), new Point(0, i * cellHeight), daysAndHoursColor, Color.White);
            }
        }

        public override void ShowDays(CalendarModel.Day _selectedDay)
        {
            if (_selectedDay == null)
                _selectedDay = selectedDay;

            title.Text = $"{(model as WeekModel).Days[6].WeekOfYear.ToString()}. week of {(model as WeekModel).Days[6].Year}";

            View.SetDoubleBuffered(panel);

            if (headerPanel.Controls.Count >= 7)
                for (int i = 0; i < 7; i++)
                    headerPanel.Controls.RemoveAt(0);

            for (int i = 0; i < (model as WeekModel).Days.Length; i++)
            {
                Label label = new Label();
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Text = $"{(model as WeekModel).Days[i].DayOfWeek}\n{(model as WeekModel).Days[i].Number}.{(model as WeekModel).Days[i].Month}";
                label.Font = new Font("Arial", 10, FontStyle.Bold);
                label.Tag = (model as WeekModel).Days[i];
                (model as WeekModel).Days[i].LabelTitle = label;
                label.MouseClick += new MouseEventHandler(day_MouseClick);
                headerPanel.Controls.Add(label, i + 1, 0);
                if ((model as WeekModel).Days[i].DateEquals(_selectedDay))
                    this.SelectDay((model as WeekModel).Days[i]);
            }

            while (panel.Controls.Count > 0)
                panel.Controls.RemoveAt(0);
            panel.Controls.Add(hoursPictureBox, 0, 0);
            for (int i = 0; i < (model as WeekModel).Days.Length; i++)
            {
                panel.Controls.Add((model as WeekModel).Days[i].PictureBox, i + 1, 0);
                this.ShowDay((model as WeekModel).Days[i]);
            }

            panel.AutoScrollPosition = new Point(0, 8 * cellHeight);
        }

        public override void ShowDay(CalendarModel.Day day)
        {
            Bitmap bitmap = new Bitmap(day.PictureBox.Width, day.PictureBox.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int i = 1; i < 24; i++)
                {
                    g.DrawLine(Pens.DarkGray, new Point(0, i * cellHeight), new Point(bitmap.Width, i * cellHeight));
                }
            }
            day.PictureBox.BackgroundImage = bitmap;
            day.PictureBox.BackColor = Color.Transparent;
            day.PictureBox.MouseClick += new MouseEventHandler(day_MouseClick);
            day.PictureBox.Image = new Bitmap(day.PictureBox.Width, day.PictureBox.Height);
            using (Graphics g = Graphics.FromImage(day.PictureBox.Image))
            {
                int cornerX = 0;
                for (int i = 0; i < day.Events.Count; i++)
                {
                    Event e = day.Events[i];

                    int j = i + 1;
                    while (j < day.Events.Count && (day.Events[j].Start.Hour < e.End.Hour || (day.Events[j].Start.Hour == e.End.Hour && day.Events[j].Start.Minute < e.End.Minute)))
                        j++;
                    int width = (day.PictureBox.Width - cornerX - 1) / (j - i);

                    Color eventColor = DataModel.IsEventAccepted(e.Id) ? e.Type.Color.Color : notAcceptedEventColor;
                    Color lighterEventColor = ControlPaint.Light(ControlPaint.LightLight(eventColor));
                    Point corner = new Point(cornerX, cellHeight * e.Start.Hour + (e.Start.Minute / 60) * cellHeight);
                    int height = (int)(((e.End.Hour - e.Start.Hour) * 60 + e.End.Minute - e.Start.Minute) * cellHeight / 60);
                    g.FillRectangle(new SolidBrush(lighterEventColor), new Rectangle(new Point(corner.X, corner.Y), new Size(width, height)));
                    TextRenderer.DrawText(g, e.Name, new Font("Arial", 10, FontStyle.Bold), new Rectangle(new Point(corner.X + 3, corner.Y + 3), new Size(width, height)), Color.Gray, lighterEventColor, TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.Top | TextFormatFlags.Left);
                    g.DrawRectangle(new Pen(eventColor, 1), new Rectangle(corner, new Size(width, height)));

                    if (j - i == 1)
                        cornerX = 0;
                    else
                        cornerX += width + 1;
                }
            }
        }

        private void day_MouseClick(object sender, MouseEventArgs e)
        {
            this.SelectDay((CalendarModel.Day)(sender as Control).Tag);
            dayManager.Show(selectedDay);
        }

        public override void SelectDay(CalendarModel.Day day)
        {
            if (selectedDay != null)
                selectedDay.LabelTitle.ForeColor = Color.Black;
            base.SelectDay(day);
            selectedDay.LabelTitle.ForeColor = selectedDayColor;
        }
    }
}
