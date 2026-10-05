namespace State_Design_Pattern.Logic
{
    class ClosedState : BookingState
    {
        private readonly string _reasonClosed;

        public ClosedState(string reason)
        {
            _reasonClosed = reason;
        }

        public override void Cancel(BookingContext booking)
        {
            booking.View.ShowError("Invalid action for this state", "Closed Booking Error");
        }

        public override void DatePassed(BookingContext booking)
        {
            booking.View.ShowError("Invalid action for this state", "Closed Booking Error");
        }

        public override void EnterDetails(BookingContext booking, string atendee, int ticketCount)
        {
            booking.View.ShowError("Invalid action for this state", "Closed Booking Error");
        }

        public override void EnterState(BookingContext booking)
        {
            booking.ShowState("Closed");
            booking.View.ShowStatusPage(_reasonClosed);
        }
    }
}
