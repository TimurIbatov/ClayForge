using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClayForge.Systems
{
    public enum ResourceKind { Coins, SoftClay, Firewood, Pigment, Glaze }

    [Serializable]
    public sealed class ResourceAmount
    {
        public ResourceKind kind;
        public int amount;
    }

    [Serializable]
    public sealed class ShopItem
    {
        public string id;
        public string title;
        public string description;
        public int price;
        public ResourceKind currency = ResourceKind.Coins;
        public ResourceKind unlocks;
        public bool purchased;
    }

    public sealed class WorkshopEconomy : MonoBehaviour
    {
        [SerializeField] private List<ResourceAmount> startingResources = new()
        {
            new ResourceAmount { kind = ResourceKind.Coins, amount = 120 },
            new ResourceAmount { kind = ResourceKind.SoftClay, amount = 8 },
            new ResourceAmount { kind = ResourceKind.Firewood, amount = 4 },
            new ResourceAmount { kind = ResourceKind.Pigment, amount = 2 },
            new ResourceAmount { kind = ResourceKind.Glaze, amount = 1 }
        };
        [SerializeField] private List<ShopItem> catalog = new()
        {
            new ShopItem { id = "wheel_pro", title = "Точный круг", description = "Больше контроля и скорости вращения.", price = 90, unlocks = ResourceKind.Coins },
            new ShopItem { id = "kiln", title = "Новая печь", description = "Открывает обжиг редких изделий.", price = 150, unlocks = ResourceKind.Firewood },
            new ShopItem { id = "blue_clay", title = "Синяя глина", description = "Пластичная глина с холодным оттенком.", price = 180, unlocks = ResourceKind.SoftClay },
            new ShopItem { id = "glaze_set", title = "Набор глазурей", description = "Три выразительных покрытия для коллекции.", price = 110, unlocks = ResourceKind.Glaze }
        };

        private readonly Dictionary<ResourceKind, int> _resources = new();
        public IReadOnlyList<ShopItem> Catalog => catalog;
        public event Action<ResourceKind, int> ResourceChanged;
        public event Action<ShopItem> ItemPurchased;

        private void Awake()
        {
            foreach (var entry in startingResources) _resources[entry.kind] = entry.amount;
        }

        public int Get(ResourceKind kind) => _resources.TryGetValue(kind, out var value) ? value : 0;

        public void Restore(ResourceKind kind, int amount)
        {
            _resources[kind] = Mathf.Max(0, amount);
            ResourceChanged?.Invoke(kind, _resources[kind]);
        }

        public bool Spend(ResourceKind kind, int amount)
        {
            if (amount <= 0 || Get(kind) < amount) return false;
            _resources[kind] -= amount;
            ResourceChanged?.Invoke(kind, _resources[kind]);
            return true;
        }

        public void Add(ResourceKind kind, int amount)
        {
            if (amount <= 0) return;
            _resources[kind] = Get(kind) + amount;
            ResourceChanged?.Invoke(kind, _resources[kind]);
        }

        public bool Purchase(string id)
        {
            var item = catalog.Find(candidate => candidate.id == id);
            if (item == null || item.purchased || !Spend(ResourceKind.Coins, item.price)) return false;
            item.purchased = true;
            ItemPurchased?.Invoke(item);
            return true;
        }

        public int SellFinishedPiece(int quality, string clayId)
        {
            var value = Mathf.Clamp(25 + quality * 4, 25, 240);
            Add(ResourceKind.Coins, value);
            Add(ResourceKind.Pigment, clayId == "porcelain" ? 1 : 0);
            return value;
        }
    }
}
