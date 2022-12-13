using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    [SerializeField] int currentHealth, maxHealth;
    [SerializeField] float damageMulitplier;

    [SerializeField] List<Collider> hitboxes = new List<Collider>();



    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        List<Collider> temphitboxes = new List<Collider>();

        temphitboxes.AddRange(GetComponentsInChildren<Collider>());
        temphitboxes.ForEach(hitbox => { if (hitbox.gameObject.tag == "Hitbox") hitboxes.Add(hitbox);});
    }

    /// <summary>
    /// The default for this method is subtracting health.
    /// Taking damage gives a positive damageIn, while healing gives a negative damageIn value.
    /// </summary>
    /// <param name="damageIn"></param>
    public void ChangeHealth(int damageIn)
    {
        currentHealth -= Mathf.FloorToInt(damageIn * damageMulitplier);
    }

}
