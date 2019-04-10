using System;

namespace Designer
{
    public class Planner
    {
        public void plan()
        {
            Machine m1 = new Machine(1, "Bohrer");
            Machine m2 = new Machine(2, "Fräser");

            Operation op0 = new Operation(0, 0);

            Operation op1 = new Operation();
            op1.setTask(1, 0, 5,op0,m1);    
            Operation op2 = new Operation();
            op2.setTask(2, 5, 10,op1,m1);
            Operation op3 = new Operation();
            op3.setTask(3, 14, -5,op2,m2);

           // Console.WriteLine("Endzeit von Op1 is {0}", op2.getEndTimePredecessor(op1)); 
        }
    }

    public class Operation
    {
        int id;
        int startTime = 0;
        private int endTime = 0;
        int duration = 0;
        Operation predecessor;
        Machine machId;

        public Operation() { }
        public Operation(int _id, int _startTime) { id = _id; startTime = _startTime; endTime = startTime; } //Für Initialoperation Operation 0

        public bool setTask(int _id, int _startTime, int _duration, Operation _predecessor, Machine _machId)
        {
            id = _id;
            startTime = _startTime;
            duration = _duration;
            endTime = startTime + duration;
            Console.WriteLine("setTast::\tid:{0};\tstartTime{1};\tduration:{2};\tendTime:{3}", id, startTime, duration, endTime);
            return true;
        }

        public int getEndTime()
        {
            return endTime; 
        }
        public int getId()
        {
            return id;
        }

    }
    public class Machine
    {
        int id;
        string name;
        public Machine (int _id,string _name) {
            id = _id;
            name = _name;
        }

        
    }

}
