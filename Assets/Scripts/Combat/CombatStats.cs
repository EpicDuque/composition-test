using UnityEngine;


[CreateAssetMenu(fileName = "New Combat Stats", menuName = "Combat/Combat Stats", order = 0)]
public class CombatStats : ScriptableObject
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int power;
    [SerializeField] private float attackPeriod;
    
    public int MAXHealth => maxHealth;
    public int Power => power;
    public float AttackPeriod => attackPeriod;
}
