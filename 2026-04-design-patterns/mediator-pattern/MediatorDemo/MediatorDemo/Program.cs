using MediatorDemo.ChatApp;
using MediatorDemo.Structural;

namespace MediatorDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var teamChat = new TeamChatroom();

            var steve = new Developer("Steve");
            var justin = new Developer("Justin");
            var jenna = new Developer("Jenna");
            var kim = new Tester("Kim");
            var julia = new Tester("Julia");

            teamChat.RegisterMembers(steve, justin, jenna, kim, julia);

            steve.Send("Hey everyone, we're going to be deploying at 2pm today.");
            kim.Send("Ok, thanks for letting us know.");

            Console.WriteLine();

            steve.SendTo<Developer>("Make sure to execute your unit tests before checking in!");
        }

        static void StructuralExample(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var mediator = new ConcreteMediator();

            //var c1 = new Colleague1(mediator);
            //var c2 = new Colleague2(mediator);

            //mediator.Colleague1 = c1;
            //mediator.Colleague2 = c2;



            //var c1 = new Colleague1();
            //var c2 = new Colleague2();

            //mediator.Register(c1);
            //mediator.Register(c2);


            var c1 = mediator.CreateColleague<Colleague1>();
            var c2 = mediator.CreateColleague<Colleague2>();

            c1.Send($"Hello from {nameof(c1)}!");
            c2.Send($"Hello there from {nameof(c2)}!");
        }
    }
}
