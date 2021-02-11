# Rust-Animal-Mount-Restrictions

## Description

This plugin restricts mounting of Rideable Entities (future support for those rideable Bears) when certain wearable items are worn.
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
      "restrictedItems": [
        "heavy.plate.helmet",
        "heavy.plate.jacket",
        "heavy.plate.pants"
      ],
      "maximumAllowed": 1,
      "entityNames": [
        "testridablehorse",
        "minicopterentity",
        "scraptransporthelicopter"
      ]
    }
  ]
}
```
