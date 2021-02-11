using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Oxide.Plugins {
    [Info("Mount Restrictions", "Jhawins", "1.0.2")]
    [Description("Restricts equipment when mounting entities based on configuration")]
    class MountRestrictions : CovalencePlugin {
        #region Localization

        protected override void LoadDefaultMessages() {
            lang.RegisterMessages(new Dictionary<string, string>() {
                ["Prefix"] = "Mount Restriction: ",
                ["CannotMount"] = "You cannot mount this while wearing that combination of gear!",
                ["CannotEquip"] = "You cannot equip that combination of gear while mounted to this!",
            }, this, "en");
        }

        private string GetMessage(string key, string id = null) => lang.GetMessage("Prefix", this, id) + lang.GetMessage(key, this, id);

        #endregion

        #region Configuration

        private struct RestrictionSet {
            public List<string> restrictedItems { get; set; }
            public int? maximumAllowed { get; set; }
            public List<string> entityNames { get; set; }
        }

        private Configuration config;

        private class Configuration {
            [JsonProperty(PropertyName = "RestrictionSets", ObjectCreationHandling = ObjectCreationHandling.Replace)]
            public List<RestrictionSet> RestrictionSets { get; set; }
        }

        private Configuration GetDefaultConfig() {
            return new Configuration {
                RestrictionSets = new List<RestrictionSet>() {
                    {
                        new RestrictionSet {
                            restrictedItems = new List<string> () { "heavy.plate.helmet", "heavy.plate.jacket", "heavy.plate.pants" },
                            maximumAllowed = 1,
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
                Config.WriteObject(config);
            } catch {
                // Likely to be a pretty insane exception. It is probably just invalid JSON
                Puts("Configuration is syntactically invalid! Validate your configuration. Loading defaults {0}");
                LoadDefaultConfig();
                return;
            }
        }

        #endregion

        #region Hooks

        bool? CanMountEntity(BasePlayer player, BaseMountable entity) {
            if (entity != null && player != null) {
                BaseVehicle vehicleEntity = entity.VehicleParent();
                if (vehicleEntity != null && CheckAnyRestrictionsMatched(player.inventory.containerWear.itemList, player, vehicleEntity, false)) {
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
                if (CheckAnyRestrictionsMatched(newWearables, player, mountedEntity, true)) {
                    return false;
                }
                return null;
            }

            return null;
        }

        #endregion

        #region Methods
        // track exceptions, hooks will be unsubscribed after 10 exceptions
        private int ConfigExceptions = 0;
        bool CheckAnyRestrictionsMatched(List<Item> items, BasePlayer player, BaseMountable mountedEntity, bool alreadyMounted) {
            try {
                if (mountedEntity.ShortPrefabName != null) {
                    string entityName = new Regex(@"\W").Replace(mountedEntity.ShortPrefabName, "");
                    List<RestrictionSet> restrictionSets = config.RestrictionSets;
                    List<RestrictionSet> matchedRestrictionSets = restrictionSets.FindAll(restrictionSet => restrictionSet.restrictedItems != null
                        && (restrictionSet.entityNames == null || restrictionSet.entityNames.Contains(entityName))
                        && restrictionSet.restrictedItems.FindAll(itemName => items.Exists(item => item.info.shortname == itemName)).Count > restrictionSet.maximumAllowed);
                    if (matchedRestrictionSets.Count > 0) {
                        string message = GetMessage(alreadyMounted ? "CannotEquip" : "CannotMount", player.UserIDString);
                        matchedRestrictionSets.ForEach(restrictionSet => player.ChatMessage(message));
                        return true;
                    }
                }
            } catch (Exception e) {
                ConfigExceptions++;
                Puts("Error - one or more restriction configurations is probably invalid {0}", e.ToString());
                if (ConfigExceptions >= 10) {
                    Puts("Fatal Error - Max exceptions of 10 has been reached. Unsubscribing hooks. Fix configuration and reload plugin");
                    Unsubscribe(nameof(CanMountEntity));
                    Unsubscribe(nameof(CanWearItem));
                }
            }
            return false;
        }

        #endregion

    }
}