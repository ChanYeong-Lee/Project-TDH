using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MeshRenderer mesh;
    private TrailRenderer trail;

    public List<ParticleSystem> meshParticles;

    public float speed;
    public float lifeTime;
    private float lifeTimeDelta;

    public float alternativeTrailTime;

    public Transform target;
    public Vector3 destination; // 쏘기 전에 죽은 적이 있을수 있으니 임시로 담아두는 목표점

    private Coroutine despawnCoroutine;
    private float SqrDistance;
    private void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        if (trail != null)
        {
            trail.Clear();
            trail.enabled = true;
        }
        if (mesh != null)
        {
            mesh.enabled = true;
        }

        foreach (ParticleSystem meshParticle in meshParticles)
        {
            meshParticle.Play();
        }

        lifeTimeDelta = lifeTime;
    }

    private void OnDisable()
    {
        if (trail != null)
        {
            trail.enabled = false;
        }

        if (target != null)
        {
            EnemyModel targetModel = target.GetComponent<EnemyModel>();
            targetModel.onDisable -= ResetTarget;
        }
        target = null;
    }

    private void Update()
    {
        if (target != null && target.gameObject.activeSelf && despawnCoroutine == null)
        {
            destination = target.position + Vector3.up;   
        }

        SqrDistance = Vector3.SqrMagnitude(destination - transform.position);

        if (target == null
            || target.gameObject.activeSelf == false
            || SqrDistance < 0.1f
            || lifeTimeDelta <= 0.0f)
        {
            if (despawnCoroutine == null)
            {
                despawnCoroutine = StartCoroutine(DespawnCoroutine()); 
            }
        }

        if (SqrDistance < 0.1f)
        {
            if (mesh != null)
            {
                mesh.enabled = false;
            }

            foreach (ParticleSystem meshParticle in meshParticles)
            {
                meshParticle.Stop(false);
                meshParticle.Clear(false);
            }
            return;
        }

        float distanceValue = Mathf.Clamp(4.0f / SqrDistance, 0.0f, 1.0f);

        Vector3 direction = destination - transform.position;
        direction.Normalize();

        transform.forward = direction;

        Vector3 moveVector = speed * transform.forward * Time.deltaTime;
        transform.position = transform.position + moveVector;

        lifeTimeDelta -= Time.deltaTime;
    }

    public void SetTarget(EnemyModel target)
    {
        this.target = target.transform;
        destination = target.transform.position + Vector3.up;
    }

    public void ResetTarget()
    {
        target = null;
    }

    private IEnumerator DespawnCoroutine()
    {
        if (trail != null)
        {
            yield return new WaitForSeconds(trail.time);
        }
        else
        {
            yield return new WaitForSeconds(alternativeTrailTime);
        }

        PoolManager.Instance.clientPool.Despawn(gameObject);
        despawnCoroutine = null;
    }
}
