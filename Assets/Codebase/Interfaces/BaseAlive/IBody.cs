using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Interfaces.BaseAlive
{
    interface IBody : IAbility
    {
        public int CalculateHealth(int parametr);
        public int CalculateMetaboly(int parametr);
        public int CalculateAge(int parametr);


        public void Execute();
    }
}
