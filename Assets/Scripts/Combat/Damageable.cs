using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField] private CombatStats combatStats;
    [SerializeField] private bool destroyOnDeath = true;

    [Space(10f)]
    [SerializeField] private UnityEvent<int> eventDamage;
    [SerializeField] private UnityEvent eventDeath;

    [Space(10f)]
    [SerializeField] private IntRef healthReference;
    [SerializeField] private IntRef maxHealthReference;

    public UnityEvent<int> EventDamage => eventDamage;
    public UnityEvent EventDeath => eventDeath;
    
    public CombatStats CombatStats
    {
        get => combatStats;
        set => combatStats = value;
    }

    [SerializeField, InspectorDisabled]
    private int health;
    
    [SerializeField, InspectorDisabled]
    private int maxHealth;

    public int Health
    {
        get => health;
        set
        {
            var amt = health - value;
            health = value;
            health = Mathf.Clamp(health, 0, combatStats.MAXHealth);

            if (healthReference != null)
                healthReference.Value = health;
                
            EventDamage?.Invoke(amt);
        
            if (health <= 0)
            {
                health = 0;
                Death();
            }
        }
    }

    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;
            
            if(maxHealthReference != null)
                maxHealthReference.Value = value;
        }
    }

    private void Start()
    {
        ResetHealth();
    }

    public void Death()
    {
        eventDeath?.Invoke();
        if(destroyOnDeath) Destroy(gameObject);
    }

    public void ResetHealth()
    {
        MaxHealth = CombatStats.MAXHealth;
        Health = MaxHealth;
    }

}
