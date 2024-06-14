using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    private MeshRenderer mesh;
    private TrailRenderer trail;
    
    [Header("설정")]
    public List<ParticleSystem> meshParticles;
    public ParticleSystem impactParticlePrefab;
    public List<BuffEffect> buffEffects;

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


    #region BEZIER
    private float arrivalTime;
    private float shotTime;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    #endregion

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
        shotTime = 0.0f;
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

        //float distanceValue = Mathf.Clamp(10.0f * speed * Time.deltaTime / distance, 0.0f, 1.0f);
        float distanceValue = shotTime;
        shotTime += Time.deltaTime;

        float forwardX = Mathf.Lerp(transform.forward.x, direction.x, distanceValue);
        float forwardY = direction.y;
        float forwardZ = Mathf.Lerp(transform.forward.z, direction.z, distanceValue);
        transform.forward = new Vector3(forwardX, forwardY, forwardZ);

        //if (t == 0.0f)
        //{
        //    p1 = transform.position;
        //    p2 = p1 + transform.forward * 5.0f;
        //    arrivalTime = distance / speed;
        //}
        //p3 = destination;
        //t += Time.deltaTime / arrivalTime;  
        //transform.forward = (2 * t - 2) * p1 + (2 - 4 * t) * p2 + 2 * t * p3;
        //transform.position = (1 - t) * (1 - t) * p1 + 2 * (1 - t) * t * p2 + t * t * p3;

        float applySpeed = Mathf.Min(speed * Time.deltaTime, distance);
        Vector3 moveVector = applySpeed * transform.forward;

        transform.position = transform.position + moveVector;

        lifeTimeDelta -= Time.deltaTime;
    }

    private IEnumerator DespawnCoroutine()
    {
        if (owner != null)
        {
            if (impactParticlePrefab != null)
            {
                PoolManager.Instance.clientPool.Spawn(impactParticlePrefab.gameObject, transform.position, Quaternion.identity);
            }
            
            switch (attackType)
            {
                case AttackType.Single:
                    if (target != null
                        && target.gameObject.activeSelf
                        && target.poolCount == targetPoolCount)
                    {
                        if (owner.photonView.IsMine)
                        {
                            ApplyDamage();
                        }
                    }
                    break;
                case AttackType.Area:
                    if (owner.photonView.IsMine)
                    {
                        ApplyDamage();
                    }
                    break;
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
                foreach(BuffEffect buffEffect in buffEffects)
                {
                    buffEffect.ApplyBuff(owner.skill, target);
                }

                target.health.photonView.RPC("TakeHitRPC", RpcTarget.All, owner.photonView.ViewID, target.poolCount, normalDamage, trueDamage);
                break;
            case AttackType.Area:
                Collider[] contectedColliders = Physics.OverlapSphere(destination, attackArea, LayerMask.GetMask("Enemy"));
                foreach (Collider collider in contectedColliders)
                {
                    EnemyModel enemy = collider.GetComponent<EnemyModel>();
                    if (enemy == null)
                    {
                        continue;
                    }

                    foreach (BuffEffect buffEffect in buffEffects)
                    {
                        buffEffect.ApplyBuff(owner.skill, enemy);
                    }

                    enemy.health.photonView.RPC("TakeHitRPC", RpcTarget.All, owner.photonView.ViewID, enemy.poolCount, normalDamage, trueDamage);
                }
                break;
        }
    }
}
