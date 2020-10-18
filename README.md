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
      "errorMessage": "Allowing you to wear that much armor would break the horse's back!",
      "entityNames": ["testridablehorse"]
    },
    {
      "maximumAllowed": 0,
      "restrictedItems": [
        "heavy.plate.helmet",
        "heavy.plate.jacket",
        "heavy.plate.pants"
      ],
      "errorMessage": "Heavy armor would sink the boat!",
      "entityNames": ["rowboat"]
    }
    {
      "maximumAllowed": 0,
      "restrictedItems": [
        "hazmatsuit",
        "hazmatsuit.spacesuit"
      ],
      "errorMessage": "You thought you were going to cheeze a monument?",
      "entityNames": ["minicopterentity", "scraptransporthelicopter"]
    }
  ]
}
```
