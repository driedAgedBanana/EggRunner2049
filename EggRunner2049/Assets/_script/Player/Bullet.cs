using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletTime = 5f;

    private void Update()
    {
        Destroy(gameObject, bulletTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);

        //Add damage here later on
    }
}
