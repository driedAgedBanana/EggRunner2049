using UnityEngine;

public enum EggType
{
    Heal,
    ExtraLife,
    // Future types like: SpeedBoost, Explode, Shield, etc.
}
public class Collectibles : MonoBehaviour
{
    public EggType eggType; // Set this in Inspector
    public int healAmount = 1;
    public int eggNeeded = 3;

    private int _currentEgg;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                switch (eggType)
                {
                    case EggType.Heal:
                        player.HealPlayer(healAmount);
                        break;
                    case EggType.ExtraLife:
                        player.CollectExtraLifeEgg(eggNeeded);
                        break;
                    // case EggType.SpeedBoost:
                    //     player.ApplySpeedBoost();
                    //     break;
                }

                Destroy(gameObject); // Egg disappears after use
            }
        }
    }
}
