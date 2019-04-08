
using StandardCode;
using System;
using Luisa; 

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

            var plannerLuisa = new PlannerLuisa();
            plannerLuisa.plan();
            Console.ReadKey(); 
        }
    }
}
