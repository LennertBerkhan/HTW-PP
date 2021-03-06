using System;

namespace StandardCode
{
    public class Worker
    {
        public void DoSomething()
        {
            IPlan n = new Plan(); // kann aber auch Plan sein 

            n.AddQuantity(1);
            n.AddQuantity(4);
            n.AddQuantity(10);
            Console.ReadKey();
        }

    }

    public class Plan : IPlan
    {
        private int _number = 0;
        private string PrivateStr { get; set; } = "I'm a private property";

        public bool AddQuantity(int number)
        {
            Console.WriteLine(@"Adding : " + number + " to " + _number + "!");
            _number = number + _number;
            return true;

        }
    }

    public interface IPlan
    {
        bool AddQuantity(int number);
    }
}
