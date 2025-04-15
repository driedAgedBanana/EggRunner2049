using UnityEngine;

public class Teleportation : MonoBehaviour
{
    public Transform player;
    public BoxCollider2D groundZone;

    public Vector2 randomPos;

    public void TeleportPlayer()
    {
        Bounds groundBound = groundZone.bounds;

        float randomX = Random.Range(groundBound.min.x, groundBound.max.x);
        float randomY = Random.Range(groundBound.min.y, groundBound.max.y);

        randomPos = new Vector2(randomX, randomY);
        player.position = randomPos;

        Debug.Log($"Teleport player to: " + randomPos);
    }
}
