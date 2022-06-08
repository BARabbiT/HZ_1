using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codebase.Utils
{
    public static class VectorUtils
    {
        public static Vector3 GetRandomDir(float step, Vector3 cV)
        {
            Vector3 corrector = Vector3.zero;
            int dir = new FastRandom().Range(0,8);

            switch (dir)
            {
                case 0:
                    {
                        corrector = new Vector3(step, 0, 0); //up
                        break;
                    }
                case 1:
                    {
                        corrector = new Vector3(step, 0, step); //up_r
                        break;
                    }
                case 2:
                    {
                        corrector = new Vector3(0, 0, step); //right
                        break;
                    }
                case 3:
                    {
                        corrector = new Vector3(-step, 0, step); //d_right
                        break;
                    }
                case 4:
                    {
                        corrector = new Vector3(-step, 0, 0); //d
                        break;
                    }
                case 5:
                    {
                        corrector = new Vector3(-step, 0, -step); //d_l
                        break;
                    }
                case 6:
                    {
                        corrector = new Vector3(0, 0, -step); //l
                        break;
                    }
                case 7:
                    {
                        corrector = new Vector3(step, 0, -step); //up_l
                        break;
                    }
                case 8:
                    {
                        corrector = new Vector3(0, 0, 0); //center
                        break;
                    }

            }

            return cV + corrector;
        }

    }
}
