namespace MediatorDemo.Structural
{
    internal class Colleague1 : Colleague
    {
        //public Colleague1(Mediator mediator) : base(mediator)
        //{
        //}

        public override void HandleNotification(string message)
        {
            Console.WriteLine($"{nameof(Colleague1)} receives notification message: {message}");
        }
    }
}
