using System;
using System.Windows;
using State_Design_Pattern.Logic;

namespace State_Design_Pattern.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EntryPage _entryPage = new EntryPage();
        private readonly StatusPage _statusPage = new StatusPage();

        private BookingContext _booking;

        public MainWindow()
        {
            InitializeComponent();
            Main.Content = _statusPage;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            _booking = new BookingContext(this);
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string attendee = _entryPage.txtAttendee.Text;

            int.TryParse(_entryPage.txtTicketCont.Text, out int ticketCount);

            _booking?.SubmitDetails(attendee, ticketCount);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _booking?.Cancel();
        }

        private void BtnDatePassed_Click(object sender, RoutedEventArgs e)
        {
            _booking?.DatePassed();
        }

        public void ShowStatusPage(string displayText)
        {
            _statusPage.txtStatus.Text = displayText;
            Main.Content = _statusPage;
        }

        public void ShowEntryPage()
        {
            _entryPage.txtAttendee.Text = "";
            _entryPage.txtTicketCont.Text = "";

            Main.Content = _entryPage;
        }

        public void ShowError(string errorText, string caption = "Error")
        {
            MessageBox.Show(this, errorText, caption);
        }

        public void ShowProcessingError()
        {
            ShowError("Processing failed. Enter a new booking", "Processing Error");
        }

    }
}
