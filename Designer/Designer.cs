using System;
using System.Collections.Generic;

namespace Designer
{
    public class Planner
    {
        private int ProductionTime { get; set; } = 16;  // Zeitspanne für Produktionsplan 

        private List<Operation> Operations { get; set; } = new List<Operation>();
        private List<Material> lm = new List<Material>();
        private List<int> lq = new List<int>();

        public void Plan()
        {
            
            var ma1 = new Machine(1, "Bohrer", 15);
            var ma2 = new Machine(2, "Fräser", 10);

            var mt1 = new Material(1, "Holz", 10); 
            var mt2 = new Material(2, "Kleber", 50);
            var mt3 = new Material(3, "Schrauben", 50);

            

            Operations.Add(new Operation(0, 0));
            lm.Add(mt1); lm.Add(mt2);
            lq.Add(10); lq.Add(20);
            Operations.Add(new Operation().SetTask(1, 0, 5, Operations[0],ma1, lm, lq));

            lm.Clear(); lq.Clear();
            lm.Add(mt1); lm.Add(mt3);
            lq.Add(10); lq.Add(20);
            Operations.Add(new Operation().SetTask(2, 5, 10, Operations[1],ma1, lm, lq));
            
            lm.Clear(); lq.Clear();
            lm.Add(mt3); lm.Add(mt2);
            lq.Add(30); lq.Add(10);
            Operations.Add(new Operation().SetTask(3, 14, -5, Operations[2],ma1, lm, lq));

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
        private List<Material> _mats;
        private List<int> Quants { get; set; }

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

        public Operation SetTask(int id, int startTime, int duration, Operation predecessor, Machine machId, List<Material> mats, List<int> quants)
        {
            Id = id;
            StartTime = startTime;
            Duration = duration;
            EndTime = StartTime + Duration;
            this._machId = machId;
            this._predecessor = predecessor;

            this._mats = mats;
            Quants = quants; 
                           
            Console.WriteLine("setTask::\tOperation id:{0}\tStart Time:{1}\tDuration:{2}\tEnd Time:{3}", Id, StartTime, Duration,EndTime);

            this._machId.SetEntry(this);
            
            for (int i = 0; i < _mats.Count; i++){
                mats[i].SetReservation(this,Quants[i]);
                //Ist Quatsch weil die Material ID private ist
                //Console.WriteLine("\t\t{0}. material with quantity {1}.",i+1,Quants[i]);
            }
            
            return this;
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

    public class Material
    {
        private int Id { get; set;  }
        private string Name { get; set; }
        private int Quantity { get; set; }
        private List<Operation> ReservationOp { get; set; } = new List<Operation>();
        private List<int> ReservationQu {get; set; } = new List<int>();

        public Material(int id, string name, int quant)
        {
            Id = id;
            Name = name;
            Quantity = quant;
        }

        public void SetReservation(Operation op, int q)
        {
            ReservationOp.Add(op);
            ReservationQu.Add(q);
        }



    }
}