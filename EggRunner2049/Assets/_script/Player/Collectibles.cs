using UnityEngine;

public enum EggType
{
    Heal,
    ExtraLife,
    ReduceCoolDown,
    // Future types like: SpeedBoost, Explode, Shield, etc.
}
public class Collectibles : MonoBehaviour
{
    public EggType eggType; // Set this in Inspector
    public int healAmount = 1;
    public int healthEggNeeded = 3;
    public int coolDownEggNeeded = 4;

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
                        player.CollectExtraLifeEgg(healthEggNeeded);
                        break;
                    case EggType.ReduceCoolDown:
                        player.ReduceCoolDownDashEgg(coolDownEggNeeded);
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
