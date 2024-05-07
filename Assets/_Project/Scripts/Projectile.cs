using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MeshRenderer mesh;
    private TrailRenderer trail;

    public float speed;
    public float lifeTime;
    private float lifeTimeDelta;
    
    public Transform target;
    public Vector3 destination; // 쏘기 전에 죽은 적이 있을수 있으니 임시로 담아두는 목표점

    private Coroutine despawnCoroutine;
    private float distance;
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

        mesh.enabled = true;
        lifeTimeDelta = lifeTime;
    }

    private void OnDisable()
    {
        if (trail != null)
        {
            trail.enabled = false;
        }
        target = null;
    }

    private void Update()
    {
        if (target != null && target.gameObject.activeSelf && despawnCoroutine == null)
        {
            destination = target.position + Vector3.up;   
        }

        if (target == null
            || target.gameObject.activeSelf == false
            || Vector3.Distance(destination, transform.position) < 0.25f
            || lifeTimeDelta <= 0.0f)
        {
            if (trail == null)
            {
                PoolManager.Instance.clientPool.Despawn(gameObject);
                return;
            }
            if (despawnCoroutine == null)
            {
                despawnCoroutine = StartCoroutine(DespawnCoroutine());
            }
        }

        distance = Vector3.Distance(destination, transform.position);
        
        if (distance < 0.25f)
        {
            mesh.enabled = false;
            return;
        }

        float distanceValue = Mathf.Clamp(1.0f / distance, 0.0f, 1.0f);

        Vector3 direction = destination - transform.position;
        direction.Normalize();

        transform.forward = Vector3.Lerp(transform.forward, direction, distanceValue);

        Vector3 moveVector = speed * transform.forward * Time.deltaTime;
        transform.position = transform.position + moveVector;

        lifeTimeDelta -= Time.deltaTime;
    }

    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitUntil(() => distance < 0.25f);
        yield return new WaitForSeconds(trail.time);
        PoolManager.Instance.clientPool.Despawn(gameObject);
        despawnCoroutine = null;
    }
}
