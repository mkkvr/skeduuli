using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Skeduuli
{
    public partial class MainForm : Form
    {
        CalendarService service;
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        public MainForm()
        {
            InitializeComponent();
            IsMdiContainer = true;
            this.Text = "Skeduuli";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            IList<CalendarListEntry> list = service.CalendarList.List().Execute().Items;
            foreach (CalendarListEntry calendar in list)
            {
                CalendarForm newMDIChild = new CalendarForm(calendar.Id, credential, calendar.Summary);
                // Set the Parent Form of the Child window.

                newMDIChild.MdiParent = this;
                // Display the new form.
                newMDIChild.Show();
            }
           


        }

    }
}
