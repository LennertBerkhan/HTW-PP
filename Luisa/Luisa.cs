using System;

namespace Luisa
{
    public class PlannerLuisa
    {
        public void plan ()
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
            _id = id;
            _startTime = startTime;
            _duration = duration;
            endTime = _startTime + _duration;
            Console.WriteLine("setTast::\tid:{0};startTime{1};duration:{2};endTime:{3}");
            return true;
        }


    }
    public class Machine
    {
        int id;
    }
}


