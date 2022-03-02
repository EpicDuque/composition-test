using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterCombat : MonoBehaviour
{
    [SerializeField] private CombatStats combatStats;
    [SerializeField] private HitBoxDamageable[] combatHitBoxes;
    [SerializeField] private UnityEvent<CombatStats> eventAttack;

    [Space(10f)]
    [SerializeField] private IntRef powerReference;
    
    public UnityEvent<CombatStats> EventAttack => eventAttack;
    public CombatStats CombatStats => combatStats;

    public HitBoxDamageable[] CombatHitBoxes
    {
        get => combatHitBoxes;
        set => combatHitBoxes = value;
    }

    private bool pressed;
    private float cooldownTime;

    private void Start()
    {
        foreach (var cb in CombatHitBoxes)
        {
            cb.ItemEnter.AddListener(OnDamageableHit);
        }
    }

    private void Update()
    {
        if (cooldownTime > 0)
        {
            cooldownTime -= Time.deltaTime;
        }
    }

    private void OnDamageableHit(Damageable damageable)
    {
        damageable.Health -= CombatStats.Power;
    }

    private void OnAttack(InputValue value)
    {
        DoAttack();
    }

    public void DoAttack()
    {
        if (cooldownTime > 0) return;
        
        eventAttack?.Invoke(combatStats);
        
        cooldownTime = CombatStats.AttackPeriod;
    }
}
