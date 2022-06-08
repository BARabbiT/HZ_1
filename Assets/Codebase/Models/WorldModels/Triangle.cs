using System.Collections.Generic;

namespace Assets.Codebase.Models.WorldModels
{
    public struct Triangle
    {
        public int I1 { get; set; }
        public int I2 { get; set; }
        public int I3 { get; set; }

        public List<int> ToList()
        {
            return new List<int>() { this.I1, this.I2, this.I3 };
        }

        public List<int> ConcatToListInt(Triangle triNew)
        {
            return new List<int>() { this.I1, this.I2, this.I3, triNew.I1, triNew.I2, triNew.I3 };
        }

 
    }
}
