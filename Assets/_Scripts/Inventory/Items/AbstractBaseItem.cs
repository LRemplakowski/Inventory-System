using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Inventory.Items
{
    public abstract class AbstractBaseItem : SerializedScriptableObject, IBaseItem
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [ShowInInspector, ReadOnly]
        public abstract ItemType ItemType { get; }
        [field: SerializeField]
        public bool Stackable { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                Name = this.name;
        }
    }
}
