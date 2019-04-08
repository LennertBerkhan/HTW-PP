using System;

namespace Designer
{
    public class Planner
    {
        public void plan()
        {
            Operation op1 = new Operation();
            op1.setTask(1, 0, 5);
            Operation op2 = new Operation();
            op2.setTask(2, 5, 10);
            Operation op3 = new Operation();
            op3.setTask(3, 15, -5);
        }
    }

    public class Operation
    {
        int id;
        int startTime;
        private int endTime;
        int duration;

        public bool setTask(int _id, int _startTime, int _duration)
        {
            id = _id;
            startTime = _startTime;
            duration = _duration;
            endTime = startTime + duration;
            Console.WriteLine("setTast::\tid:{0};\tstartTime{1};\tduration:{2};\tendTime:{3}", id, startTime, duration, endTime);
            return true;
        }


    }
    public class Machine
    {
        int id;
    }
}
