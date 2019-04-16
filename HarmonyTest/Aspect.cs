using Harmony;
using System;
using System.Reflection;
using Designer;
using System.Collections.Generic;

namespace HarmonyTest
{
    public class DurationNotNegativ
    {
        //private  static  object PrefixRuntime = CodeGenerator.Gernerate();
        
        /// <summary>
        /// Applying Aspects to a Methods
        /// </summary>
        /// <returns>return true if correct applied</returns>
        public static bool Apply()
        {
            /// Create a Harmony Instance that has an unique name
            var harmony = HarmonyInstance.Create("DurationNotNegativ");

            //harmony.PatchAll(Assembly.GetExecutingAssembly());

            /// get the method to override
            var original = typeof(Operation).GetMethod("setTask");

            /// gather the methodInfos for patching
            var prefix = typeof(DurationNotNegativ).GetMethod("BeforeCall");
            var postfix = typeof(DurationNotNegativ).GetMethod("AfterCall");

            /// Patch the method (Apply the aspect)
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            /// Check if methods are applied and return result
            return harmony.HasAnyPatches("DurationNotNegativ");
        }

        public static void BeforeCall(int _id, int _startTime, int _duration, Operation _predecessor, Machine _machId)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //CONSTRAINT =>  context: Operation inv: _duration > 0
            if (_duration < 0)
            {
                Console.WriteLine("PLANNING ERROR: Duration time from ID {0} is negativ.", _id);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Aspect after method call, catch the result
        /// </summary>
        /// <param name="__result">Return value of the Parameter</param>
        public static void AfterCall(Operation __instance)
        {
            // Even Private properties are accessable through reflection.
            PropertyInfo strProperty =
            __instance.GetType().GetProperty("endTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            MethodInfo strGetter = strProperty.GetGetMethod(nonPublic: true);
            int val = (int)strGetter.Invoke(__instance, null);
        }
    }

    public class OverlappingProdTime
    {
        public static bool Apply()
        {
            var harmony = HarmonyInstance.Create("OverlappingProdTime");
            var original = typeof(Operation).GetMethod("setTask");
            var prefix = typeof(OverlappingProdTime).GetMethod("BeforeCall");
            var postfix = typeof(OverlappingProdTime).GetMethod("AfterCall");
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            return harmony.HasAnyPatches("OverlappingProdTime");
        }

        public static void BeforeCall(int _id, int _startTime, int _duration, Operation _predecessor, Machine _machId)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //CONSTRAINT =>  context: Operation inv: _startTime < predecessor.getEndtime()
            if (_startTime < _predecessor.getEndTime())
            {
                Console.WriteLine("PLANNING ERROR: Overalpping production times for ID {0} and ID {1}", _id, _predecessor.getId());
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AfterCall(Operation __instance)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //SPACE FOR CODE
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class MachineNotFree
    {
        public static bool Apply()
        {
            var harmony = HarmonyInstance.Create("MachineNotFree");
            var original = typeof(Machine).GetMethod("setEntry");
            var prefix = typeof(MachineNotFree).GetMethod("BeforeCall");
            var postfix = typeof(MachineNotFree).GetMethod("AfterCall");
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            return harmony.HasAnyPatches("MachineNotFree");
        }

        public static void BeforeCall(Machine __instance, Operation op)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Machine Not Free");

            PropertyInfo prop = __instance.GetType().GetProperty("workload", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            MethodInfo methInf = prop.GetGetMethod(nonPublic: true);
            List<Operation> workload = new List<Operation>();
            workload = (List<Operation>)methInf.Invoke(__instance, null);

            prop = op.GetType().GetProperty("startTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            methInf = prop.GetGetMethod(nonPublic: true);
            int startTimeToAdd = (int)methInf.Invoke(op, null);

            prop = op.GetType().GetProperty("endTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            methInf = prop.GetGetMethod(nonPublic: true);
            int endTimeToAdd = (int)methInf.Invoke(op, null);

            int startTime, endTime;

            foreach (Operation o in workload)
            {
                prop = o.GetType().GetProperty("startTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
                methInf = prop.GetGetMethod(nonPublic: true);
                startTime = (int)methInf.Invoke(o, null);

                prop = o.GetType().GetProperty("endTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
                methInf = prop.GetGetMethod(nonPublic: true);
                endTime = (int)methInf.Invoke(o, null);

                if(startTimeToAdd > startTime && startTimeToAdd < endTime)
                {
                    Console.WriteLine("PLANNING ERROR: Start time from Operation-ID {0} is within other production time.", op.getId());
                }   
                
                if(endTimeToAdd > startTime && endTimeToAdd < endTime)
                {
                    Console.WriteLine("PLANNING ERROR: End time from Operation-ID {0} is within other production time.", op.getId());
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AfterCall(Machine __instance)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //SPACE FOR CODE
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class TEMPLATE
    {
        public static bool Apply()
        {
            var harmony = HarmonyInstance.Create("TEMPLATE");
            var original = typeof(Machine).GetMethod("<METHODENAME>");
            var prefix = typeof(MachineNotFree).GetMethod("BeforeCall");
            var postfix = typeof(MachineNotFree).GetMethod("AfterCall");
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            return harmony.HasAnyPatches("TEMPLATE");
        }

        public static void BeforeCall()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //SPACE FOR CODE
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AfterCall(Operation __instance)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //SPACE FOR CODE
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
