using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class DoPuncher : MonoBehaviour
{
    [FormerlySerializedAs("punch")] 
    [SerializeField] private Transform target;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float intensity = 0.5f;
    [SerializeField] private int vibrato = 0;
    [SerializeField] private bool toggleVisible = true;

    private Vector3 originalPosition;
    private Tweener tween;
    
    public Vector3 PunchDirection { get; set; }

    private void Start()
    {
        PunchDirection = Vector3.forward;
        
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
        
        tween = target.DOPunchPosition(PunchDirection * intensity, duration, vibrato);
        
        if(toggleVisible)
            tween.OnComplete(() => target.gameObject.SetActive(false));
    }

    private void OnDestroy()
    {
        target.gameObject.SetActive(false);
    }
}
