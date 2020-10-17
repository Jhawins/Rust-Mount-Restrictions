using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Animal Mounting Restrictions", "Jhawins", "0.1.0")]
    [Description("A plugin that restricts mounting of Rideable Animals based on configured criteria")]
    class AnimalMountRestrictions : CovalencePlugin
    {
        #region Configuration

        private struct RestrictionSet
        {
            public List<string> restrictedItems { get; set; }
            public int? maximumAllowed { get; set; }
            public string errorMessage { get; set; }
        }

        private Configuration config;

        private class Configuration
        {
            [JsonProperty("RestrictionSets")]
            public List<RestrictionSet> RestrictionSets { get; set; }
        }

        private Configuration GetDefaultConfig()
        {
            return new Configuration
            {
                RestrictionSets = new List<RestrictionSet>() {
                    {
                        new RestrictionSet {
                            restrictedItems = new List<string> () { "heavy.plate.helmet", "heavy.plate.jacket", "heavy.plate.pants" },
                            maximumAllowed = 1,
                            errorMessage = "Wearing more than 1 heavy item while mounting this is not allowed!"
                        }
                    }
                }
            };
        }

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(GetDefaultConfig(), true);
        }

        #endregion

        #region Init

        private void Init()
        {
            config = Config.ReadObject<Configuration>();
            Puts("Initialized Mount Restrictions");
        }

        #endregion

        #region Hooks

        bool? CanMountEntity(BasePlayer player, BaseMountable entity)
        {
            var animal = entity.GetComponentInParent<BaseRidableAnimal>() ?? null;
            List<RestrictionSet> restrictionSets = config.RestrictionSets ?? null;

            if (animal != null && player != null)
            {
                if (restrictionSets != null && CheckAnyRestrictionsMatched(player.inventory.containerWear.itemList, player))
                {
                    return false;
                }
            }

            return null;
        }

        bool? CanWearItem(PlayerInventory inventory, Item item, int targetSlot)
        {
            BasePlayer player = inventory._baseEntity;
            BaseMountable mountedEntity = player.GetMounted() ?? null;
            if (mountedEntity != null && (mountedEntity.GetComponentInParent<BaseRidableAnimal>() ?? null) != null)
            {
                List<Item> newWearables = player.inventory.containerWear.itemList.ToList();
                newWearables.Add(item);
                return !CheckAnyRestrictionsMatched(newWearables, player);
            }

            return null;
        }

        #endregion

        #region Methods

        bool CheckAnyRestrictionsMatched(List<Item> items, BasePlayer player)
        {
            List<RestrictionSet> restrictionSets = config.RestrictionSets ?? null;
            if (restrictionSets != null && player != null)
            {
                List<RestrictionSet> matchedRestrictionSets = restrictionSets.Where(restrictionSet => restrictionSet.restrictedItems != null && restrictionSet.restrictedItems.Where(itemName => items.Any(item => item.info.shortname == itemName)).ToList().Count > restrictionSet.maximumAllowed).ToList();
                if (matchedRestrictionSets.Count > 0)
                {
                    matchedRestrictionSets.ForEach(restrictionSet => player.ChatMessage($"Animal Mount Restriction: {restrictionSet.errorMessage}"));
                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}