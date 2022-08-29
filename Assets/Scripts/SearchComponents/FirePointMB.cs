using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePointMB : MonoBehaviour
{
    [SerializeField]
    private Transform _firePointTransform;

    public Transform GetFirePoint()
    {
        if (_firePointTransform == null)
        {
            Debug.LogError($"{gameObject} dont have information about their «FirePoint». Check Inspector.");
            return null;
        }

        return _firePointTransform;
    }
}
