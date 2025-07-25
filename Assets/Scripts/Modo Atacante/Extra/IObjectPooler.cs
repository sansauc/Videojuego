using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPooler
{
    GameObject GetInstanceFromPool();
    void ReturnToPool(GameObject instance);
    IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay);

    void ClearPool();

}
