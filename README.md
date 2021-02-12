This plugin restricts mounting entities based on configurable "restriction sets".

Example: Restrict mounting horses when more than 1 heavy armor item is equipped.

## Limitations
**This plugin currently only restricts clothing/equipment. You will not be able to restrict weapon/hotbar items.**


## Configuration

### Default Configuration

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
      "langKey": "HeavyArmor",
      "entityNames": [
        "testridablehorse",
        "minicopterentity",
        "scraptransporthelicopter"
      ]
    }
  ]
}
```

## Localization

You MUST define a corresponding Lang entry for each rule created. The key for each entry should exactly match your `langKey` property from the restrictionSet. Messages may be re-used for similar rules. 

### Default Localization

 ```json
{
  "Prefix": "Mount Restriction: ",
  "HeavyArmor": "Wearing more than 1 heavy item while mounting this is not allowed!"
}
```
