using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float speed = 5f;
    private bool hasCloned = false;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= 0 && !hasCloned)
        {
            Instantiate(gameObject, new Vector3(120, transform.position.y, transform.position.z), Quaternion.identity);
            hasCloned = true;
        }

        if (transform.position.x <= -24)
        {
            Destroy(gameObject);
        }
    }
}
