using UnityEngine;

public class UnitMB : MonoBehaviour
{
    public int Entity;
    [SerializeField] bool _isFriendly;

    public bool IsFriendly
    {
        get
        {
            return _isFriendly;
        } 
    }
}
