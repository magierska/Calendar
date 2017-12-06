using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess;
using CalendarModel;

namespace Calendar
{
    public class RightMonthView : View 
    {
        TableLayoutPanel bigPanel;
        Label title, numberLabel;

        public RightMonthView(TableLayoutPanel _bigPanel, Label _title)
        {
            bigPanel = _bigPanel;
            title = _title;
            model = new MonthModel();
        }

        public override void ShowDays(CalendarModel.Day _selectedDay)
        {
            title.Text = $"{_selectedDay.MonthName} {_selectedDay.Year}";

            TableLayoutPanel panel = new TableLayoutPanel();
            View.SetDoubleBuffered(panel);
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 7;
            panel.RowCount = (model as MonthModel).Days.Count;
            for (int i = 0; i < panel.ColumnCount; i++)
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < panel.RowCount; i++)
                panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            int j = 0;
            while ((model as MonthModel).Days[0][j].Number != 1)
                j++;
            for (int i = 0; i < (model as MonthModel).Days.Count; i++, j = 0)
                for (; j < 7; j++)
                {
                    if ((model as MonthModel).Days[i][j].Month != _selectedDay.Month)
                        break;
                    this.ShowDay((model as MonthModel).Days[i][j]);
                    panel.Controls.Add(numberLabel, j, i);
                    if ((model as MonthModel).Days[i][j].DateEquals(_selectedDay))
                        base.SelectDay((model as MonthModel).Days[i][j]);
                }
            if (bigPanel.Controls.Count >= 2)
                bigPanel.Controls.RemoveAt(1);
            bigPanel.Controls.Add(panel, 0, 1);
        }

        public override void ShowDay(CalendarModel.Day day)
        {
            numberLabel = new Label();
            View.SetDoubleBuffered(numberLabel);
            numberLabel.Anchor = AnchorStyles.None;
            numberLabel.Text = day.Number.ToString();
        }
    }
}
