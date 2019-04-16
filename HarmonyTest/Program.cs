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
            
            if (!DurationNotNegativ.Apply()) throw new Exception("Applying aspect failed");
            if (!OverlappingProdTime.Apply()) throw new Exception("Applying aspect failed");

            planner.plan();
            Console.ReadKey();

        }
    }
}