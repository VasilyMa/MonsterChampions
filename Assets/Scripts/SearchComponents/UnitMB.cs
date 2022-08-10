using UnityEngine;

public class UnitMB : MonoBehaviour
{
    [SerializeField] bool _isFriendly;

    public bool IsFriendly
    {
        get
        {
            return _isFriendly;
        } 
    }
}
