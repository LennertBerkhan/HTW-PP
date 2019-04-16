using Harmony;
using StandardCode;
using System;
using System.Reflection;
using Designer;

namespace HarmonyTest
{
    public class Aspect
    {
        //Erstmal ohne Code Generator
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
            var prefix = typeof(Aspect).GetMethod("BeforeCall");
            var postfix = typeof(Aspect).GetMethod("AfterCall");

            /// Patch the method (Apply the aspect)
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            /// Check if methods are applied and return result
            return harmony.HasAnyPatches("DurationNotNegativ");
        }

        public static bool Apply2()
        {
            var harmony = HarmonyInstance.Create("Machine");

            var original = typeof(Machine).GetMethod("setTimetable");

            var prefix = typeof(Aspect).GetMethod("BeforeCall2");
            //var postfix = typeof(Aspect).GetMethod("AfterCall2");

            harmony.Patch(original, new HarmonyMethod(prefix));//, new HarmonyMethod(postfix));


            /// Check if methods are applied and return result
            return harmony.HasAnyPatches("Machine");
        }


        public static void BeforeCall(int _id, int _startTime, int _duration, Operation _predecessor, Machine _machId)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(""); 
            //context: Operation inv: _duration > 0
            if (_duration < 0)
            {
                Console.WriteLine("Planning error: duration time from ID {0} is negativ.", _id);
            }

            //context: Operation inv: _startTime < predecessor.getEndtime()
            if (_startTime < _predecessor.getEndTime())
            {
                Console.WriteLine("Planning error: overalpping production times for ID {0} and ID {1}", _id, _predecessor.getId());
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void BeforeCall2(Machine __instance, int _st, int _et)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;

            int[,] tt = __instance.getTimetable();
            for (int i = 0; i < tt.GetLength(0); i++)
            {
                if(tt[i, 0] + tt[i, 1] != 0)
                {
                   
                    if(tt[i, 0] < _st && _st < tt[i, 1])
                    {
                        Console.WriteLine("Machine: Starttermin überschneidet sich mit anderen Jobs");
                    }
                    if (tt[i, 0] < _et && _et < tt[i, 1])
                    {
                        Console.WriteLine("Machine: Endtermin überschneidet sich mit anderen Jobs");
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Aspect after method call, catch the result
        /// </summary>
        /// <param name="__result">Return value of the Parameter</param>
        public static void AfterCall(Operation __instance)
        {
            Console.WriteLine(" -After Method- Endtime of the Operation is: {0}", __instance.getEndTime());
            // Console.WriteLine(" Result was " + endTime, "Aspect");
            
            // Even Private properties are accessable through reflection.
            PropertyInfo strProperty =
            __instance.GetType().GetProperty("endTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            MethodInfo strGetter = strProperty.GetGetMethod(nonPublic: true);
            int val = (int)strGetter.Invoke(__instance, null);
            Console.WriteLine("Private int -" + val);       
        }
    }
}
