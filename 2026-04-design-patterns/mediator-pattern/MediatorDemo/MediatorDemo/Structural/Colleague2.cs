namespace MediatorDemo.Structural
{
    internal class Colleague2 : Colleague
    {
        //public Colleague2(Mediator mediator) : base(mediator)
        //{
        //}

        public override void HandleNotification(string message)
        {
            Console.WriteLine($"{nameof(Colleague2)} receives notification message: {message}");
        }
    }
}
