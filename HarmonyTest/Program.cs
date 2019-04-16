using System;
using Designer; 

namespace HarmonyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var planner = new Planner();
            Console.WriteLine("Output without Harmony/Aspects");
            planner.plan();
            Console.ReadKey();

            //Implement aspects
            if (!DurationNotNegativ.Apply()) throw new Exception("Applying aspect failed");
            if (!OverlappingProdTime.Apply()) throw new Exception("Applying aspect failed");
            if (!MachineNotFree.Apply()) throw new Exception("Applying aspect failed");
            if (!CapacityCheck.Apply()) throw new Exception("Applying aspect failed");

            Console.WriteLine("\nOutput with Harmony/Aspects");
            planner.plan();
            Console.ReadKey();

        }
    }
}