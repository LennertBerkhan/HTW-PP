using System.Diagnostics;
using Newtonsoft.Json;

namespace HarmonyBridge
{
    public class Aspect
    {
        public string ContextName;
        public string FunctionName;
        public string BeforeCode;
        public string AfterCode;

        public static Aspect OclToAspect(string ocl)
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "conv.exe",
                    Arguments = ocl
                }
            };
            p.Start();
            var json = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            var deserializedAspect = JsonConvert.DeserializeObject<Aspect>(json);
            return deserializedAspect;
        }
    }
}