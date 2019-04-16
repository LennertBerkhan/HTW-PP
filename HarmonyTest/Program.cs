using System;
using Designer; 

namespace HarmonyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var planner = new Planner();
            planner.plan();
            Console.ReadKey();
            
            if (!Aspect.Apply()) throw new Exception("Applying aspect failed");
            if (!Aspect.Apply2()) throw new Exception("Applying aspect failed");

            planner.plan();
            Console.ReadKey();

        }
    }
}