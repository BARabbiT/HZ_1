using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouth : IAbility
{
    //0 - Herbivore, 1 - non Herbivore, 2 - both
    public int Herbivore { get; }

    public int Calculate(int parametr);

    public void Execute();
}
