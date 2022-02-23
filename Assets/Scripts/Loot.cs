using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Loot : MonoBehaviour
{
    private new Rigidbody rigidbody;

    public Rigidbody Rigidbody => rigidbody;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Inventory>(out var inventory))
        {
            inventory.LootAmount++;
            Destroy(gameObject);
        }
    }
}
