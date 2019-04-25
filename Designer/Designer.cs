using System;
using System.Collections.Generic;
using System.Linq; 

namespace Designer
{
    public class Planner
    {
        private int ProductionTime { get; set; } = 16;  // Zeitspanne für Produktionsplan 

        private List<Operation> Operations { get; set; } = new List<Operation>();

        public void Plan()
        {
            
            var ma1 = new Machine(1, "Bohrer", 15);
            var ma2 = new Machine(2, "Fräser", 10);

            var mt1 = new Material(1, "Holz", 10); 
            var mt2 = new Material(2, "Kleber", 50);
            var mt3 = new Material(3, "Schrauben", 50);

            Operations.Add(new Operation(0, 0));
            Operations.Add(new Operation().SetTask(1, 0, 5, Operations[0] , ma1, mt1, 10));
            Operations.Add(new Operation().SetTask(2, 5, 10, Operations[1], ma1, mt1, 10));
            Operations.Add(new Operation().SetTask(3, 14, -5, Operations[2], ma1, mt3, 30));

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
        private Material _matId;
        private int Quant { get; set; } = 0;

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

        public Operation SetTask(int id, int startTime, int duration, Operation predecessor, Machine machId, Material mat, int quant)
        {
            Id = id;
            StartTime = startTime;
            Duration = duration;
            EndTime = StartTime + Duration;
            this._machId = machId;
            this._predecessor = predecessor;
            this._matId = mat;
            Quant = quant; 
                           
            Console.WriteLine("setTask::\tid:{0};\tstartTime{1};\tduration:{2};\tendTime:{3}", Id, StartTime, Duration,EndTime);

            this._machId.SetEntry(this);
            this._matId.SetReservation(this); 
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
        private List<Operation> Reservation { get; set; } = new List<Operation>();

        public Material(int id, string name, int quant)
        {
            Id = id;
            Name = name;
            Quantity = quant;
        }

        public void SetReservation(Operation op)
        {
            Reservation.Add(op); 
        }



    }
}