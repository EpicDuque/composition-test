using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class GenericDamageable : MonoBehaviour
{
    [SerializeField] private CombatStats combatStats;
    [SerializeField] private bool destroyOnDeath = true;

    [Space(10f)]
    [SerializeField] private UnityEvent<int> eventDamage;
    [SerializeField] private UnityEvent eventDeath;

    public UnityEvent<int> EventDamage => eventDamage;
    public UnityEvent EventDeath => eventDeath;
    
    public CombatStats CombatStats
    {
        get => combatStats;
        set => combatStats = value;
    }

    [SerializeField, InspectorDisabled]
    private int health;

    public int Health => health;

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
        health = combatStats.MAXHealth;
    }
    
    public void TakeDamage(int amt)
    {
        health -= amt;

        EventDamage?.Invoke(amt);
        
        if (health <= 0)
        {
            health = 0;
            Death();
        }
    }
}
