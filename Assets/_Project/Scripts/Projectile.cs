using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    private MeshRenderer mesh;
    private TrailRenderer trail;
    
    [Header("설정")]
    public List<ParticleSystem> meshParticles;
    public ParticleSystem impactParticlePrefab;

    public float speed;
    public float lifeTime;
    public float alternativeTrailTime;
    private float lifeTimeDelta;

    [Header("상태")]
    public EnemyModel target;
    public CharacterAttack owner;
    public int targetPoolCount;
    public AttackType attackType;
    public float normalDamage;
    public float trueDamage;
    public float attackArea;

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

        target = null;

        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
        }
        despawnCoroutine = null;
    }

    private void Update()
    {
        if (target != null
            && target.gameObject.activeSelf
            && target.poolCount == targetPoolCount
            && despawnCoroutine == null)
        {
            destination = target.transform.position + Vector3.up;
        }

        distance = Vector3.Distance(destination, transform.position);

        if (distance < 0.25f
            || lifeTimeDelta <= 0.0f)
        {
            if (despawnCoroutine == null)
            {
                despawnCoroutine = StartCoroutine(DespawnCoroutine());
            }
        }

       if (distance < 0.25f)
       {
           //transform.position = destination;

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

        distance = Vector3.Distance(destination, transform.position);

        Vector3 direction = destination - transform.position;
        direction.Normalize();

        float distanceValue = Mathf.Clamp(10.0f * speed * Time.deltaTime / distance, 0.0f, 1.0f);

        float forwardX = Mathf.Lerp(transform.forward.x, direction.x, distanceValue);
        float forwardY = direction.y;
        float forwardZ = Mathf.Lerp(transform.forward.z, direction.z, distanceValue);
        transform.forward = new Vector3(forwardX, forwardY, forwardZ);

        float applySpeed = Mathf.Min(speed * Time.deltaTime, distance); 
        Vector3 moveVector = applySpeed * transform.forward;
        transform.position = transform.position + moveVector;

        lifeTimeDelta -= Time.deltaTime;
    }

    private IEnumerator DespawnCoroutine()
    {
        if (owner != null 
            && target != null
            && target.gameObject.activeSelf
            && target.poolCount == targetPoolCount)
        {
            if (owner.photonView.IsMine)
            {
                ApplyDamage();
            }
            
            if (impactParticlePrefab != null) 
            {
                PoolManager.Instance.clientPool.Spawn(impactParticlePrefab.gameObject, transform.position,Quaternion.identity);
            }
        }

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

    private void ApplyDamage()
    {
        switch (attackType)
        {
            case AttackType.Single:
                target.health.photonView.RPC("TakeHitRPC", RpcTarget.All, target.poolCount, normalDamage, trueDamage);
                break;
            case AttackType.Area:
                Collider[] contectedColliders = Physics.OverlapSphere(target.transform.position, attackArea, LayerMask.GetMask("Enemy"));
                foreach (Collider collider in contectedColliders)
                {
                    EnemyModel enemy = collider.GetComponent<EnemyModel>();
                    enemy.health.photonView.RPC("TakeHitRPC", RpcTarget.All, enemy.poolCount, normalDamage, trueDamage);
                }
                break;
        }
    }
}
