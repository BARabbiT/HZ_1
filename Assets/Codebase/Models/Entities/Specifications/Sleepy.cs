using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Models.Entities.Specifications
{
    public struct Sleepy
    {
        public int value;
        public int Id { get; }

        public Sleepy(int value)
        {
            Id = 3;
            this.value = value;
        }
    }
}
