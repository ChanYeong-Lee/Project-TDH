using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCallbackAdapter : MonoBehaviour
{
    public void OnParticleSystemStopped()
    {
        PoolManager.Instance.clientPool.Despawn(gameObject);
    }
}
