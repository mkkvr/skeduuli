using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace Skeduuli
{
    public partial class CalendarForm : Form
    {

        // static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        CalendarService service;

        DateTime selectedDay;

        String calendarId;


        public CalendarForm(String calendarId, UserCredential credential, String title)
        {
            InitializeComponent();

            this.Text = title;
            this.calendarId = calendarId;

            for (int i = 0; i < 24; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                row.CreateCells(dataGridView1);
                row.Cells[0].Value = i.ToString();
                row.Cells[1].Value = "";
               
                dataGridView1.Rows.Add(row);

            }

            //UserCredential credential;

            //using (var stream =
            //    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            //{
            //    string credPath = System.Environment.GetFolderPath(
            //        System.Environment.SpecialFolder.Personal);
            //    credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart");

            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(credPath, true)).Result;
            //    Console.WriteLine("Credential file saved to: " + credPath);
            //}

            // Create Google Calendar API service.
            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(calendarId);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {

                    EventDateTime edt = eventItem.Start;
                    if (edt.DateTime != null)
                    {
                        DateTime bolded = (DateTime)eventItem.Start.DateTime;
                        monthCalendar1.AddBoldedDate(bolded);

                        string when = eventItem.Start.DateTime.ToString();
                        if (String.IsNullOrEmpty(when))
                        {
                            when = eventItem.Start.Date;
                        }
                    }
                }
            }
            monthCalendar1.MaxSelectionCount = 1;


       }

        public void addNewEvent(String text, DateTime startDate, DateTime endDate)
        {

             Event newEvent = new Event();
             newEvent.Summary = text;
             newEvent.Start = new EventDateTime();
             newEvent.Start.DateTime = startDate;
             newEvent.End = new EventDateTime();
             newEvent.End.DateTime = endDate;

             service.Events.Insert(newEvent, calendarId).Execute();

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            DateTime start = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day, e.RowIndex, 0,0);
            DateTime end = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day, e.RowIndex+1, 0, 0);
            AddForm subForm = new AddForm(this, start,end);
            subForm.Show();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

            dataGridView1.Rows.Clear();

            for (int i = 0; i < 24; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                row.CreateCells(dataGridView1);
                row.Cells[0].Value = i.ToString();
                row.Cells[1].Value = "";

                dataGridView1.Rows.Add(row);

            }

            EventsResource.ListRequest request = service.Events.List(calendarId);
            request.TimeMin = monthCalendar1.SelectionRange.Start;

            int year = ((DateTime)request.TimeMin).Year;
            int month = ((DateTime)request.TimeMin).Month;
            int day = ((DateTime)request.TimeMin).Day;

            selectedDay = new DateTime(year, month, day, 0, 0, 0);

            request.TimeMax = new DateTime(year, month, day, 23, 59, 59);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    Event thisEvent = service.Events.Get(calendarId, eventItem.Id).Execute();
                    int startHour = ((DateTime)thisEvent.Start.DateTime).Hour;
                    int endHour = ((DateTime)thisEvent.End.DateTime).Hour;
                    dataGridView1.Rows[((DateTime)thisEvent.Start.DateTime).Hour].Cells[1].Value = thisEvent.Summary;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int selectedRows = dataGridView1.SelectedRows.Count;
            int hours = dataGridView1.SelectedRows[0].Index;
            DateTime start = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day, hours, 0, 0);
            DateTime end = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day, hours + 1, 0, 0);
            AddForm subForm = new AddForm(this, start, end);
            subForm.Show();
        }
    }
}
