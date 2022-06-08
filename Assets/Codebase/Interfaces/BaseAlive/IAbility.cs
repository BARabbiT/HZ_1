using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IAbility
{
    public void Subscribe(BeingAI obj);

    public void UnSubscribe(BeingAI obj);
}
