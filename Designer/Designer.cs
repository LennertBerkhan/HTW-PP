using System;
using System.Collections.Generic;

namespace Designer
{
    public class Planner
    {
        public void plan()
        {
            
            Machine m1 = new Machine(1, "Bohrer",15);
            Machine m2 = new Machine(2, "Fräser",10);

            Operation op0 = new Operation(0, 0);
            Operation op1 = new Operation();
            op1.setTask(1, 0, 5,op0,m1);    
            Operation op2 = new Operation();
            op2.setTask(2, 5, 10,op1,m1);
            Operation op3 = new Operation();
            op3.setTask(3, 14, -5,op2,m1);
        }
    }

    public class Operation
    {
        private int id { get; set; }
        private int startTime { get; set; } = 0;
        private int endTime { get; set; } = 0;
        private int duration { get; set; } = 0;
        private Operation predecessor;
        private Machine machId;

        public Operation() { }
        public Operation(int _id, int _startTime) { id = _id; startTime = _startTime; endTime = startTime; } //Für Initialoperation Operation 0

        public bool setTask(int _id, int _startTime, int _duration, Operation _predecessor, Machine _machId)
        {
            id = _id;
            startTime = _startTime;
            duration = _duration;
            endTime = startTime + duration;
            machId = _machId;
            predecessor = _predecessor;
            
            Console.WriteLine("setTask::\tid:{0};\tstartTime{1};\tduration:{2};\tendTime:{3}", id, startTime, duration, endTime);
            
            machId.setEntry(this);
            return true;
        }
    }

    public class Machine
    {
        private int id { get; set; }
        private string name { get; set; }
        private int capacity { get; set; }
        private List<Operation> workload { get; set; } = new List<Operation>();

        public Machine (int _id,string _name, int _capa) {
            id = _id;
            name = _name;
            capacity = _capa;
        }

        public void setEntry (Operation op)
        {
            workload.Add(op);
        }
    }
}
