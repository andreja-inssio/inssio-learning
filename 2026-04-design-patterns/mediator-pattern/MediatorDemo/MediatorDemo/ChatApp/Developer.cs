namespace MediatorDemo.ChatApp
{
    public class Developer : TeamMember
    {
        public Developer(string name) : base(name)
        {
            
        }

        public override void Receive(string from, string message)
        {
            Console.WriteLine($"{Name} ({nameof(Developer)}) has received: {message}");
            base.Receive(from, message);
        }
    }
}
