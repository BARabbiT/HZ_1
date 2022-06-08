using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Models.Entities.Specifications
{
    public struct Healthy
    {
        public int value;
        public int Id { get; }

        public Healthy(int value)
        {
            Id = 4;
            this.value = value;
        }
    }
}
