using UnityEngine;
using CoolTools.Attributes;
namespace CoolTools.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Game Data/Item", order = 0)]
    public class ItemBase : ScriptableObject
    {
        [SerializeField] protected int itemID;
        [SerializeField] protected string itemName = "Item";
        [SerializeField, SpritePreviewSmall] protected Sprite spriteIcon;

        public Sprite SpriteIcon => spriteIcon;

        public string ItemName => itemName;

        public int ItemID => itemID;
    }
}