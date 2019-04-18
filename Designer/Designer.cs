using System;
using System.Collections.Generic;

namespace Designer
{
    public class Planner
    {
        public static void Plan()
        {
            var m1 = new Machine(1, "Bohrer", 15);
            var m2 = new Machine(2, "Fräser", 10);

            var op0 = new Operation(0, 0);
            var op1 = new Operation();
            op1.SetTask(1, 0, 5, op0, m1);
            var op2 = new Operation();
            op2.SetTask(2, 5, 10, op1, m1);
            var op3 = new Operation();
            op3.SetTask(3, 14, -5, op2, m1);
        }
    }

    public class Operation
    {
        private int Id { get; set; }
        private int StartTime { get; set; } = 0;
        private int EndTime { get; set; } = 0;
        private int Duration { get; set; } = 0;
        private Operation _predecessor;
        private Machine _machId;

        public Operation()
        {
        }

        // initial Operation 0
        public Operation(int id, int startTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = StartTime;
        } 

        public bool SetTask(int id, int startTime, int duration, Operation predecessor, Machine machId)
        {
            Id = id;
            StartTime = startTime;
            Duration = duration;
            EndTime = StartTime + Duration;
            this._machId = machId;
            this._predecessor = predecessor;

            Console.WriteLine("setTask::\tid:{0};\tstartTime{1};\tduration:{2};\tendTime:{3}", Id, StartTime, Duration,
                EndTime);

            this._machId.SetEntry(this);
            return true;
        }
    }

    public class Machine
    {
        private int Id { get; set; }
        private string Name { get; set; }
        private int Capacity { get; set; }
        private List<Operation> Workload { get; set; } = new List<Operation>();

        public Machine(int id, string name, int capa)
        {
            Id = id;
            Name = name;
            Capacity = capa;
        }

        public void SetEntry(Operation op)
        {
            Workload.Add(op);
        }
    }
}