using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterCombat : MonoBehaviour
{
    [SerializeField] private CombatStats combatStats;
    [SerializeField] private HitBoxDamageable[] combatHitBoxes;
    [SerializeField] private UnityEvent<CombatStats> eventAttack;

    public UnityEvent<CombatStats> EventAttack => eventAttack;
    public CombatStats CombatStats => combatStats;

    public HitBoxDamageable[] CombatHitBoxes
    {
        get => combatHitBoxes;
        set => combatHitBoxes = value;
    }

    private bool pressed;

    private void Start()
    {
        foreach (var cb in CombatHitBoxes)
        {
            cb.ItemEnter.AddListener(OnDamageableHit);
        }
    }

    private void OnDamageableHit(GenericDamageable damageable)
    {
        damageable.TakeDamage(combatStats.Power);
    }

    private void OnAttack(InputValue value)
    {
        DoAttack();
    }

    public void DoAttack()
    {
        eventAttack?.Invoke(combatStats);
    }
}
