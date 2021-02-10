using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Oxide.Plugins {
    [Info("Mount Restrictions", "Jhawins", "1.0.0")]
    [Description("Restricts equipment when mounting entities based on configuration")]
    class MountRestrictions : CovalencePlugin {

        #region Localization

        protected override void LoadDefaultMessages() {
            lang.RegisterMessages(new Dictionary<string, string>() {
                ["HeavyArmor"] = "Wearing more than 1 heavy item while mounting this is not allowed!"
            }, this);
        }

        private string GetMessage(string key) => lang.GetMessage(key, this);

        #endregion

        #region Configuration

        private struct RestrictionSet {
            public List<string> restrictedItems { get; set; }
            public int? maximumAllowed { get; set; }
            public string messageKey { get; set; }
            public List<string> entityNames { get; set; }
        }

        private Configuration config;

        private class Configuration {
            [JsonProperty(PropertyName = "RestrictionSets")]
            public List<RestrictionSet> RestrictionSets { get; set; }
        }

        private Configuration GetDefaultConfig() {
            return new Configuration {
                RestrictionSets = new List<RestrictionSet>() {
                    {
                        new RestrictionSet {
                            restrictedItems = new List<string> () { "heavy.plate.helmet", "heavy.plate.jacket", "heavy.plate.pants" },
                            maximumAllowed = 1,
                            messageKey = "HeavyArmor",
                            entityNames = new List<string> { "testridablehorse", "minicopterentity", "scraptransporthelicopter" }
                        }
                    }
                }
            };
        }

        protected override void LoadDefaultConfig() {
            Config.WriteObject(GetDefaultConfig(), true);
        }

        protected override void LoadConfig() {
            base.LoadConfig();
            try {
                config = Config.ReadObject<Configuration>();
                if (config == null) {
                    throw new JsonException();
                }
                Puts("Non-default configuration found. Saving");
                SaveConfig();
            } catch {
                Puts($"Configuration file is syntactically invalid! Default configuration will be used");
                LoadDefaultConfig();
            }
        }

        #endregion

        #region Hooks

        bool? CanMountEntity(BasePlayer player, BaseMountable entity) {
            if (entity != null && player != null) {
                BaseVehicle vehicleEntity = entity.VehicleParent();
                if (vehicleEntity != null && CheckAnyRestrictionsMatched(player.inventory.containerWear.itemList, player, vehicleEntity)) {
                    return false;
                }
            }

            return null;
        }

        bool? CanWearItem(PlayerInventory inventory, Item item) {
            BasePlayer player = inventory.containerWear.playerOwner;
            BaseVehicle mountedEntity = player.GetMountedVehicle();
            if (mountedEntity != null) {
                List<Item> newWearables = new List<Item>(player.inventory.containerWear.itemList);
                newWearables.Add(item);
                return !CheckAnyRestrictionsMatched(newWearables, player, mountedEntity);
            }

            return null;
        }

        #endregion

        #region Methods

        bool CheckAnyRestrictionsMatched(List<Item> items, BasePlayer player, BaseMountable mountedEntity) {
            try {
                if (mountedEntity.ShortPrefabName != null) {
                    string entityName = new Regex(@"\W").Replace(mountedEntity.ShortPrefabName, "");
                    List<RestrictionSet> restrictionSets = config.RestrictionSets;
                    List<RestrictionSet> matchedRestrictionSets = restrictionSets.FindAll(restrictionSet => restrictionSet.restrictedItems != null
                        && (restrictionSet.entityNames == null || restrictionSet.entityNames.Contains(entityName))
                        && restrictionSet.restrictedItems.FindAll(itemName => items.Exists(item => item.info.shortname == itemName)).Count > restrictionSet.maximumAllowed);
                    if (matchedRestrictionSets.Count > 0) {
                        matchedRestrictionSets.ForEach(restrictionSet => player.ChatMessage($"Mount Restriction: {GetMessage(restrictionSet.messageKey)}"));
                        return true;
                    }
                }
            } catch {
                Puts($"Mount Restriction: Error - one or more restriction configurations may be invalid");
                return false;
            }

            return false;
        }

        #endregion

    }
}
