# Rust-Animal-Mount-Restrictions

## Description

This plugin restricts mounting of Rideable Eneities (future support for those rideable Bears) when certain wearable items are worn.
Also restricts equipping items once already mounted.

## Use
Oxide plugin, load like any other

## Configuration
Supports multiple sets of restrictions.

Default restriction is no more than 1 Heavy armor can be equipped at a time.
```json
{
  "RestrictionSets": [
    {
      "maximumAllowed": 1,
      "restrictedItems": [
        "heavy.plate.helmet",
        "heavy.plate.jacket",
        "heavy.plate.pants"
      ],
      "errorMessage": "Wearing more than 1 heavy item while mounting this is not allowed!",
      // leave this blank to apply to ALL mountables
      "entityNames": ["testridablehorse", "minicopterentity", "rowboat", "scraptransporthelicopter"]
    }
  ]
}
```
