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
    public class DayView : View
    {
        TableLayoutPanel panel;
        Label title;
        int cellHeight;
        DayManager dayManager;
        PictureBox hoursPictureBox;

        public DayView(TableLayoutPanel _panel, Label _title, DayManager _dayManager)
        {
            panel = _panel;
            title = _title;
            dayManager = _dayManager;
            model = new DayModel();

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

            title.Text = $"{(model as DayModel).Day.Number} {(model as DayModel).Day.MonthName} {(model as DayModel).Day.Year}";
            title.MouseClick += new MouseEventHandler(day_MouseClick);

            View.SetDoubleBuffered(panel);

            while (panel.Controls.Count > 0)
                panel.Controls.RemoveAt(0);
            panel.Controls.Add(hoursPictureBox, 0, 0);
            panel.Controls.Add((model as DayModel).Day.PictureBox, 1, 0);
            this.ShowDay((model as DayModel).Day);

            this.SelectDay(_selectedDay);
        }

        public override void ShowDay(CalendarModel.Day _day)
        {
            Bitmap bitmap = new Bitmap(_day.PictureBox.Width, _day.PictureBox.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int i = 1; i < 24; i++)
                {
                    g.DrawLine(Pens.DarkGray, new Point(0, i * cellHeight), new Point(bitmap.Width, i * cellHeight));
                }
            }
            _day.PictureBox.BackgroundImage = bitmap;
            _day.PictureBox.BackColor = Color.Transparent;
            _day.PictureBox.MouseClick += new MouseEventHandler(day_MouseClick);
            _day.PictureBox.Image = new Bitmap(_day.PictureBox.Width, _day.PictureBox.Height);
            using (Graphics g = Graphics.FromImage(_day.PictureBox.Image))
            {
                int cornerX = 0;
                for (int i = 0; i < _day.Events.Count; i++)
                {
                    Event e = _day.Events[i];

                    int j = i + 1;
                    while (j < _day.Events.Count && (_day.Events[j].Start.Hour < e.End.Hour || (_day.Events[j].Start.Hour == e.End.Hour && _day.Events[j].Start.Minute < e.End.Minute)))
                        j++;
                    int width = (_day.PictureBox.Width - cornerX - 1) / (j - i);

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
            this.SelectDay((model as DayModel).Day);
            dayManager.Show(selectedDay);
        }
    }
}
