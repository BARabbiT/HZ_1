using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Models.Entities.Specifications
{
    public struct Hungry
    {
        public int value;
        public int Id { get; }
        
        public Hungry(int value)
        {
            Id = 1;
            this.value = value;
        }
    }
}
