using UnityEngine;


public class Ammo : MonoBehaviour
{
    [SerializeField] AmmoSlot[] ammoSlots;

    AmmoSlot GetAmmoSlot(AmmoType type)
    {
        foreach (AmmoSlot slot in ammoSlots)
        {
            if (slot.ammoType == type)
            {
                return slot;
            }
        }

        return null;
    }

    public int GetCurrentAmmo(AmmoType type)
    {
        return GetAmmoSlot(type).ammoAmount;
    }

    public void ReduceCurrentAmmo(AmmoType type, int amount)
    {
        GetAmmoSlot(type).ammoAmount -= amount;
    }

    public void IncreaseCurrentAmmo(AmmoType type, int amount)
    {
        GetAmmoSlot(type).ammoAmount += amount;
    }
}
