using System.Threading;

namespace State_Design_Pattern.Logic
{
    class PendingState : BookingState
    {
        CancellationTokenSource _cancellationTokenSource;

        public override void Cancel(BookingContext booking)
        {
            _cancellationTokenSource.Cancel();
        }

        public override void DatePassed(BookingContext booking)
        {

        }

        public override void EnterDetails(BookingContext booking, string atendee, int ticketCount)
        {

        }

        public override void EnterState(BookingContext booking)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            booking.ShowState("Pending");
            booking.View.ShowStatusPage("Processing Booking");

            StaticFunctions.ProcessBooking(booking, ProcessingComplete, _cancellationTokenSource);
        }

        public void ProcessingComplete(BookingContext booking, ProcessingResult result)
        {
            switch (result)
            {
                case ProcessingResult.Success:
                    booking.TransitionToState(new BookedState());
                    break;
                case ProcessingResult.Fail:
                    booking.View.ShowProcessingError();
                    booking.TransitionToState(new NewState());
                    break;
                case ProcessingResult.Cancel:
                    booking.TransitionToState(new ClosedState("Processing Cancelled"));
                    break;
                default:
                    break;
            }
        }
    }
}
