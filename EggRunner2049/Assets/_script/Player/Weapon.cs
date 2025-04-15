using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;

    public void Fire()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 fireDirection = (mouseWorldPos - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(fireDirection * fireForce, ForceMode2D.Impulse);
    }

    //public void FireSpread()
    //{
    //    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

    //    float spreadAngle = 15f;

    //    // Fire center
    //    FireInDirection(direction);

    //    // Fire left
    //    FireInDirection(Quaternion.Euler(0, 0, -spreadAngle) * direction);

    //    // Fire right
    //    FireInDirection(Quaternion.Euler(0, 0, spreadAngle) * direction);
    //}

    //private void FireInDirection(Vector2 dir)
    //{
    //    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    //    bullet.GetComponent<Rigidbody2D>().AddForce(dir.normalized * fireForce, ForceMode2D.Impulse);
    //}

}
