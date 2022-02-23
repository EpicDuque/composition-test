using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class HitBox<T> : MonoBehaviour where T : Component
{
    public enum HitBoxQueryFilterType
    {
        Include, Exclude, None
    }
    
    [SerializeField] protected Collider owner;
    [SerializeField] protected HitBoxQueryFilterType filterType;
    [SerializeField] protected List<TeamData> teamFilter;
    
    [Space(10f)]
    [SerializeField] protected UnityEvent<T> itemEnter;
    [SerializeField] protected UnityEvent<T> itemExit;

    public UnityEvent<T> ItemEnter => itemEnter;
    public UnityEvent<T> ItemExit => itemExit;

    private List<T> instances = new();
    
    public T[] Instances => instances.ToArray();

    public List<TeamData> TeamFilter => teamFilter;

    public HitBoxQueryFilterType FilterType => filterType;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other == owner) return;

        if (!other.TryGetComponent<T>(out var item)) return;

        if (other.TryGetComponent<TeamIdentity>(out var identity) && !IsValidTeam(identity)) return;
        
        ObjectEntered(item);
        itemEnter?.Invoke(item);
        instances.Add(item);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<T>(out var item) && instances.Contains(item))
        {
            ObjectExit(item);
            itemExit?.Invoke(item);
            instances.Remove(item);
        }
    }

    protected bool IsValidTeam(TeamIdentity identity)
    {
        return filterType switch
        {
            HitBoxQueryFilterType.Include => teamFilter.Contains(identity.TeamData),
            HitBoxQueryFilterType.Exclude => !teamFilter.Contains(identity.TeamData),
            HitBoxQueryFilterType.None => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public virtual T GetClosestInstance() => Instances
        .OrderBy(i => (transform.position - i.transform.position).sqrMagnitude)
        .First();
    
    protected abstract void ObjectEntered(T item);
    protected abstract void ObjectExit(T item);
}
