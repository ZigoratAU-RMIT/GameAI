using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Physics Layer")]
public class PhysicsLayerFilter : ContextFilter
{
    public LayerMask mask;

    private List<Transform> filtered = new List<Transform>();

    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        if(filtered.Count == 0)
        {
            foreach (Transform item in original)
            {
                if (mask == (mask | (1 << item.gameObject.layer)))
                {
                    Debug.Log(item);
                    filtered.Add(item);
                }
            }
        }
        
        
        return filtered;
    }
}
