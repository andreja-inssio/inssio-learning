namespace MediatorDemo.ChatApp
{
    public abstract class TeamMember
    {
        public string Name { get; }
        private Chatroom _chatroom;
        protected TeamMember(string name)
        {
            Name = name;
        }

        internal void SetChatroom(Chatroom chatroom)
        {
            _chatroom = chatroom;
        }

        public void Send(string message)
        {
            _chatroom.Send(Name, message);
        }

        public virtual void Receive(string from, string message)
        {
            Console.WriteLine($"from {from}: '{message}'");
        }

        public void SendTo<T>(string message) where T: TeamMember
        {
            _chatroom.SendTo<T>(Name, message);
        }
    }
}
