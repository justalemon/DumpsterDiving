# Configuration

The configuration file is called `DumpsterDiving.json` and it can be found on your `scripts` folder.

## blips

* Type: [bool]
* Default Value: true

Specifies if the dumpsters should have a blip attached once you get near them.

## sound

* Type: [bool]
* Default Value: true

If the Chinatown Wars "looting sound" should be played during the diving process.

## fade

* Type: [int]
* Default Value: 250

The time in milliseconds for the fade in and fade out animations. Set to `0` for disabling the animations.

## markerdistance

* Type: [float]
* Default Value: 25

The maximum distance from the player to the dumpster to show a blip on the minimap/radar. The default value should be enough for ~500m.

## moneymin

* Type: [int]
* Default Value: 10

The minimum ammount of money that you can get from the dumpster. This value is inclusive.

## moneymax

* Type: [int]
* Default Value: 10

The Maximum ammount of money that you can get from the dumpster. This value is inclusive.

[bool]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/bool
[int]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/int
[float]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/float
