using UnityEngine;

public class UnitMB : MonoBehaviour
{
    public int Entity;
    public int unitID;
    public GameObject[] prefabs;
    [SerializeField] bool _isFriendly;

    public bool IsFriendly
    {
        get
        {
            return _isFriendly;
        } 
    }
}
