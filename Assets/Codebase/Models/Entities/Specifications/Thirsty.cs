using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Models.Entities.Specifications
{
    public struct Thirsty
    {
        public int value;
        public int Id { get; }

        public Thirsty(int value)
        {
            Id = 2;
            this.value = value;
        }
    }
}
