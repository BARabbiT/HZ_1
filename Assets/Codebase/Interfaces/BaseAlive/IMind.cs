﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Interfaces.BaseAlive
{
    interface IMind : IAbility
    {
        public int Calculate(int parametr);

        public void Execute();
    }
}
