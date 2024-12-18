using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Obstacle" || other.transform.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
    }
}
