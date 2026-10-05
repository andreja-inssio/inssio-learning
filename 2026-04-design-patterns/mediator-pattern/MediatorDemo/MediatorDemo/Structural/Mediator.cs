namespace MediatorDemo.Structural
{
    internal abstract class Mediator
    {
        public abstract void Send(string message, Colleague colleague);
    }
}
