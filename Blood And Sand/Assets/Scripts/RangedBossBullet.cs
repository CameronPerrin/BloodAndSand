using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBossBullet : MonoBehaviour
{
    public Rigidbody rb;
    public int projectileSpeed = 10;
    public float damage = 0;
    public int type = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.forward * projectileSpeed;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Health hp = collision.gameObject.GetComponent<Health>();
            if (hp)
            {
                hp.TakeDamage(damage, type);
            }
        }
        if (collision.gameObject.tag == "Untagged")
        {
            Destroy(this.gameObject);
        }
        //Destroy(this.gameObject);
    }
}
