using System;
using DG.Tweening;
using UnityEngine;

public class DoMovePuncher : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform punchTowards;
    
    [Space(10f)]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float intensity = 0.5f;
    [SerializeField] private int vibrato = 0;
    [SerializeField] private bool toggleVisible = true;

    private Vector3 originalPosition;
    private Tweener tween;
    private Vector3 punchDirection;

    public Transform PunchTowards
    {
        get => punchTowards;
        set => punchTowards = value;
    }

    private void Start()
    {
        punchDirection = punchTowards == null ? 
            Vector3.forward : 
            punchTowards.position - transform.position;
        
        if(toggleVisible)
            target.gameObject.SetActive(false);
        
        originalPosition = target.localPosition;
    }

    public void Punch()
    {
        if(toggleVisible)
            target.gameObject.SetActive(true);
        
        tween?.Kill();
        
        target.localPosition = originalPosition;
        
        tween = target.DOPunchPosition(punchDirection * intensity, duration, vibrato);
        
        if(toggleVisible)
            tween.OnComplete(() => target.gameObject.SetActive(false));
    }

    private void OnDestroy()
    {
        target.gameObject.SetActive(false);
    }
}
