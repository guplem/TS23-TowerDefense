using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileMover : MonoBehaviour
{
    public float speed = 15f;
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public GameObject hit;
    public GameObject flash;
    public GameObject impactSoundPrefab;
    public AudioClip impactClip;
    private Rigidbody rb;
    public GameObject[] Detached;
    // public UnityEvent onCollisionEnter = new();
    private HealthController target;
    private int damage;
    private float offset = 0;
    private Vector3 latestPosTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
        Destroy(gameObject,5);
	}

    void FixedUpdate ()
    {
		if (speed != 0)
        {
            //rb.velocity = transform.forward * speed;
            if (target != null)
                latestPosTarget = target.transform.position;
            if (target == null && Vector3.Distance(latestPosTarget, transform.position) <= speed)
                Destroy(gameObject);
            rb.velocity = (latestPosTarget + (Vector3.up * (1 - offset)) + Vector3.right * offset - transform.position).normalized * speed;
        }
	}

    //https ://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    void OnCollisionEnter(Collision collision)
    {
        //Lock all axes movement and rotation
        rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            GameObject hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            ParticleSystem hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }

            if (impactSoundPrefab != null && impactClip != null)
            {
                GameObject soundInstance = Instantiate(impactSoundPrefab, pos, rot);
                AudioSource audioSource = soundInstance.GetComponent<AudioSource>();
                audioSource.clip = impactClip;
                audioSource.Play();
                Destroy(soundInstance, impactClip.length);   
            }
        }
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
                Destroy(detachedPrefab, 3.5f);
            }
        }
        // onCollisionEnter?.Invoke();
        collision.gameObject.GetComponentRequired<HealthController>().health -= damage;
        Destroy(gameObject);
    }

    public void SetTarget(HealthController target, int damage, float offset)
    {
        this.target = target;
        this.damage = damage;
        this.offset = offset;
    }
}
