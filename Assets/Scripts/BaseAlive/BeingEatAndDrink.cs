using Assets.Codebase.Models.Enums;
using UnityEngine;

public class BeingEatAndDrink : MonoBehaviour
{
    private BeingAI _beingAI;

    public void Awake()
    {
        _beingAI = GetComponentInParent<BeingAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Apple")
        {
            _beingAI.AddHungry(10);
            _beingAI.AddPointToMind(MindPointType.Eat, gameObject.transform.position);
        }
        if (other.gameObject.name.Contains("Water"))
        {
            _beingAI.AddThirsty(10);
            _beingAI.AddPointToMind(MindPointType.Drink, gameObject.transform.position);
        }
    }
}
