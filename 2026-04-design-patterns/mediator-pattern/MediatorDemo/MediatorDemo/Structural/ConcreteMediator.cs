namespace MediatorDemo.Structural
{
    internal class ConcreteMediator : Mediator
    {
        //public Colleague1 Colleague1 { get; set; }
        //public Colleague2 Colleague2 { get; set; }

        private List<Colleague> colleagues = [];

        public void Register(Colleague colleague)
        {
            colleague.SetMediator(this);
            colleagues.Add(colleague);
        }

        public T CreateColleague<T>() where T: Colleague, new()
        {
            var c = new T();
            c.SetMediator(this);
            return c;
        }

        public override void Send(string message, Colleague colleague)
        {
            //if (colleague == Colleague1)
            //{
            //    Colleague1.HandleNotification(message);
            //}
            //else
            //{
            //    Colleague2.HandleNotification(message);
            //}

            colleagues.Where(c => c != colleague).ToList().ForEach(c => c.HandleNotification(message));
        }
    }
}
