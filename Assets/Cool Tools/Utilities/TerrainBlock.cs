using System;
using System.Text;
using CoolTools.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CoolTools.Core
{
    public class TerrainBlock : MonoBehaviour
    {
        private const float TimeToUnlock = 1.5f; // Seconds
        [SerializeField] private string blockID;
        public string BlockID
        {
            get => blockID; private set => blockID = value;
        }
        [SerializeField, HideInInspector] private bool locked = true;
        
        [ColorSpacer("Block Settings", spaceHeight = 5f)]
        [SerializeField] private bool unlocked;
        [SerializeField] private int price = 100;
        
        [Header("Events")]
        [SerializeField] private UnityEvent unlockedEvent;

        [ColorSpacer("Editor Settings", 0.7f, 0.85f, 0.7f)] 
        [SerializeField] private GameObject groupModel;
        [SerializeField] private GameObject groupEditor;
        [SerializeField] private GameObject floor;
        // [SerializeField] private MMFeedbacks unlockFeedback;
        // [SerializeField] private MMFeedbacks lockFeedback;
        // [SerializeField] private NavMeshObstacle navMeshObstacle;

        [Space(10f)]
        [SerializeField] private Image unlockProgressImage;

        [Header("Walls")] 
        [SerializeField] private GameObject wallFront;
        [SerializeField] private GameObject wallRight;
        [SerializeField] private GameObject wallBack;
        [SerializeField] private GameObject wallLeft;
        
        [Header("Texts")]
        [SerializeField] private TMP_Text textPrice;
        
        [Header("Icons")]
        [SerializeField] private Image iconUnlocked;

        private IDisposable unlockCountDisposable;
        private float unlockProgress;
        private Color originalProgressColor;
        private IObservable<long> unlockTimer;
        private IObservable<long> updateUnlockTimer;
        // private TerrainBlockManager blockManager;

        // public TerrainBlockManager BlockManager
        // {
        //     get => blockManager;
        //     set => blockManager = value;
        // }

        public float BlockSize => transform.localScale.y;

        public bool Unlocked
        {
            get => unlocked;
            set
            {
                unlocked = value;
                UpdateLockedUnlockedVisual();
                UpdateBlockColliders();

                if (unlocked)
                {
                    UnlockedEvent?.Invoke();
                }
            }
        }

        public float UnlockProgress => unlockProgress;
        public UnityEvent UnlockedEvent => unlockedEvent;
        public GameObject GroupModel => groupModel;

        private void Start()
        {
            // From here below, only editor UI will be allowed.
            if (Application.isPlaying)
            {
                groupEditor.SetActive(false);
            }

            if (!unlocked)
            {
                // Unlocked = PrefsManager.GetUnlockedTerrainBlock(this);
            }
            else
            {
                UnlockedEvent?.Invoke();
            }

            unlockProgressImage.gameObject.SetActive(true);
            originalProgressColor = unlockProgressImage.color;
            unlockProgressImage.color = Color.clear;
        }

        public void UpdateBlockColliders()
        {
            wallFront.SetActive(!IsBlockInDirection(transform.forward));
            wallRight.SetActive(!IsBlockInDirection(transform.right));
            wallBack.SetActive(!IsBlockInDirection(-transform.forward));
            wallLeft.SetActive(!IsBlockInDirection(-transform.right));
            
            UpdatePriceText();
        }

        public bool IsBlockAdjacent()
        {
            return IsBlockInDirection(transform.forward) ||
                   IsBlockInDirection(transform.right) ||
                   IsBlockInDirection(-transform.forward) ||
                   IsBlockInDirection(-transform.right);
        }

        private bool IsBlockInDirection(Vector3 direction)
        {
            var results = new RaycastHit[30];
            
            var ray = new Ray(transform.position + Vector3.up * BlockSize * 0.5f, direction);
            UnityEngine.Physics.RaycastNonAlloc(ray, results, BlockSize * 4f);
            
            for (var i = 0; i < 10; i++)
            {
                var col = results[i].collider;
                if (col == null || !col.CompareTag("Solid")) continue;
                
                var block = col.GetComponentInParent<TerrainBlock>();
                if (block != this && block.Unlocked)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateLockedUnlockedVisual()
        {
            floor.SetActive(unlocked);
            
            switch (unlocked)
            { 
                case true when !groupModel.activeSelf:
                    // unlockFeedback.PlayFeedbacks();
                    
                    break;
                
                case false when groupModel.activeSelf:
                    groupModel.SetActive(false);

                    break;
            }
            
            UpdatePriceText();
        }

        private void OnValidate()
        {
            UpdatePriceText();
            
            if(iconUnlocked != null)
                iconUnlocked.gameObject.SetActive(unlocked);
        }

        private void UpdatePriceText()
        {
            if (textPrice == null) return;
            
            if (unlocked)
            {
                textPrice.text = string.Empty;
                return;
            }

            textPrice.text = IsBlockAdjacent() ? price.ToString() : string.Empty;
        }

        private void ToggleProgressImage(bool state)
        {
            var imageTransform = unlockProgressImage.transform;
            if (state)
            {
                imageTransform.parent.gameObject.SetActive(true);
                // imageTransform.DOComplete();
                // unlockProgressImage.DOComplete();
                
                unlockProgressImage.color = originalProgressColor;
                imageTransform.localScale = Vector3.one;
            }
            else
            {
                // imageTransform.DOComplete();
                // unlockProgressImage.DOComplete();
                
                imageTransform.parent.gameObject.SetActive(false);
            }
        }
        
        public void ToggleUnlockCount(bool state)
        {
            if (unlocked) return;
            
            unlockCountDisposable?.Dispose();

            ToggleProgressImage(state);

            if (!state) return;
            
            unlockProgress = 0f;
            unlockProgressImage.fillAmount = 0f;

            // unlockTimer = Observable.Timer(TimeSpan.FromSeconds(TimeToUnlock));
            // updateUnlockTimer = Observable.EveryUpdate().TakeUntil(unlockTimer);
            
            // unlockCountDisposable = updateUnlockTimer.Subscribe(frame =>
            // {
            //     unlockProgress += Time.deltaTime;
            //     unlockProgressImage.fillAmount = unlockProgress / TimeToUnlock;
            //
            //     if (PrefsManager.CoinAmount < price)
            //     {
            //         unlockCountDisposable.Dispose();
            //     }
            // });
            //
            // updateUnlockTimer.Last().Subscribe(_ =>
            // {
            //     if (unlockProgress < TimeToUnlock) return;
            //     
            //     ToggleProgressImage(false);
            //     unlockProgressImage.fillAmount = 1f;
            //     Unlocked = true;
            //             
            //     Wallet.AddCurrency(-price);
            //
            //     Broker.Publish(new TerrainBlockUnlocked{Block = this});
            // });
        }

        /// <summary>
        /// WARNING: Dangerous function.
        /// Do NOT execute this function if you are not sure what is this for.
        ///
        /// Generates a new random Block ID for persistance (Save / Load).
        /// </summary>
        // [ContextMenu("Generate New Block ID")]
        // public void GenerateBlockID()
        // {
        //     // Hard Coded safety flag to avoid misuse of this funcion.
        //     var forceIDGen = false;
        //
        //     if (!forceIDGen && BlockID != string.Empty) return;
        //     
        //     var length = 7;
        //
        //     // creating a StringBuilder object()
        //     var str_build = new StringBuilder();  
        //     var random = new Random();  
        //     random.InitState((uint)UnityEngine.Random.Range(0, int.MaxValue));
        //     
        //     char letter;  
        //
        //     for (int i = 0; i < length; i++)
        //     {
        //         var flt = random.NextDouble();
        //         var shift = Convert.ToInt32(Math.Floor(25 * flt));
        //         letter = Convert.ToChar(shift + 65);
        //         str_build.Append(letter);  
        //     }
        //
        //     blockID = str_build.ToString();
        // }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            var position = transform.position + Vector3.up * BlockSize * 0.5f;
            
            Gizmos.DrawLine(position, position + Vector3.forward * 4);
            Gizmos.DrawLine(position, position + Vector3.right * 4);
            Gizmos.DrawLine(position, position + Vector3.left * 4);
            Gizmos.DrawLine(position, position + Vector3.back * 4);
            
        }
    }
}