
using StandardCode;
using System;
using PlannerClasses; 

namespace HarmonyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //// Without Applied Patches
            //var worker = new Worker();
            //worker.DoSomething();



            //Console.WriteLine(""); 
            //Console.WriteLine("--- Applying Aspects ---");
            //Console.WriteLine("");
            //Console.ReadKey();

            //if (!Aspect.Apply()) throw new Exception("Applying aspect failed");

            //worker.DoSomething();

            var planner = new Planner();
            planner.plan();
            Console.ReadKey();
            
            if (!Aspect.Apply2()) throw new Exception("Applying aspect failed");
            planner.plan();
            Console.ReadKey();

        }
    }
}
