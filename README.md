# CustomJSONData
CustomJSONData is a library that allows the loading of arbitrary data from specific locations in beatmap JSON files. Custom data can be attached to levels (songs), individual difficulties of songs, and individual notes, obstacles, and lighting events within difficulties. In addition, entirely new event types can be added. 

NOTE: CustomJSONData does NOT work for OST maps and will not convert them to their `CustomBeatmap` counterparts. To add on to that, CustomJSONData completely overwrites how custom maps are deserialized from JSON.

# Custom data in info.dat
Custom data can also be placed on entire levels (songs) and on individual difficulties. For example:
```json
// ...
"_songFilename": "song.ogg",
"_coverImageFilename": "cover.png",
"_environmentName": "DefaultEnvironment",
"_customData": {
  "_contributors": [{
      "_role": "Furry",
      "_name": "Reaxt",
      "_iconPath": "furry.png"
    },
    {
      "_role": "Lighter",
      "_name": "Kyle 1413 The Second",
      "_iconPath": "test.png"
    }
  ],
  "_customEnvironment": "CoolCustomEnv",
  "_customEnvironmentHash": ""
},
"_difficultyBeatmapSets": [{
    "_beatmapCharacteristicName": "Standard",
    "_difficultyBeatmaps": [{
        "_difficulty": "Easy",
        "_difficultyRank": 1,
        "_beatmapFilename": "Easy.dat",
        "_noteJumpMovementSpeed": 0.0,
        "_noteJumpStartBeatOffset": 0,
        "_customData": {
          "_difficultyLabel": "Nightmare"
        }
// ...
```
This data can be accessed by casting a `BeatmapData` to `CustomJSONData.CustomBeatmap.CustomBeatmapData`. The first "_customData" section (the one for the whole level) is provided as `levelCustomData`, and the second (the one for the Easy difficulty) is provided as `beatmapCustomData`. 

Example (For more about reading custom data, see [Reading custom data](#Reading-custom-data)):
```csharp
if (beatmapData is CustomBeatmapData customBeatmapData) 
{
    string customEnvironment = customBeatmapData.levelCustomData.Get<string>("_customEnvironment"); // "CoolCustomEnv"
    string label = customBeatmapData.beatmapCustomData.Get<string>("_difficultyLabel"); // "Nightmare"
}
```

The custom data section is also available from a `StandardLevelInfoSaveData` by casting to `CustomJSONData.CustomLevelInfo.CustomLevelInfoSaveData` and accessing the customData property. `beatmapCustomData` is a little more complicated. In order to do so, cast a `StandardLevelInfoSaveData.DifficultyBeatmap` to a `CustomLevelInfoSaveData.DifficultyBeatmap`.

Example:
```csharp
if (standardLevelInfoSaveData is CustomLevelInfoSaveData customLevelInfoSaveData) 
{
    string customEnvironment = customLevelInfoSaveData.customData.Get<string>("_customEnvironment"); // "CoolCustomEnv"

    CustomLevelInfoSaveData.DifficultyBeatmap difficultyBeatmap = (CustomLevelInfoSaveData.DifficultyBeatmap)customBeatmapData.difficultyBeatmapSets.First().difficultyBeatmaps.First();
    string label = difficultyBeatmap.Get<string>("_difficultyLabel"); // "Nightmare"
}
```

# Custom data on notes, obstacles, waypoints, and lighting events
Custom data can be attached to notes (including bombs), obstacles, waypoints, and lighting events simply by adding a `_customData` property to the event/note/obstacle/waypoint object in the difficulty JSON file. For example, adding some custom fields to a note:
```json
"_notes": [{
    "_time": 8.0,
    "_lineIndex": 2,
    "_lineLayer": 0,
    "_type": 1,
    "_cutDirection": 1,
    "_customData": {
      "foo": 3,
      "bar": "Hello, BSMG!"
    }
  }
]
```
To get this data from a `NoteData`, `ObstacleData`, `WaypointData`, or `BeatmapEventData` object in your plugin code, cast it to the appropriate type from the `CustomJSONData.CustomBeatmap` namespace (`CustomNoteData`, `CustomObstacleData`, `CustomWaypointData`, or `CustomBeatmapEventData`) and access the resulting object's customData property.

Example (For more about reading custom data, see [Reading custom data](#Reading-custom-data)):
```csharp
if (noteData is CustomNoteData customNoteData)
{
    int foo = customNoteData.customData.Get<int>("foo"); // 3
}
```

*Note: The recommended way to create custom events to trigger plugin functionality is with CustomJSONData's [custom events](#Custom-events) feature. Custom data on lighting events should be used when your plugin does something related to the Beat Saber lighting event the data is placed on (e.g. changing the color of a group of lights or the direction of a ring spin), not to create new event types.*

# Reading custom data
In CustomJSONData, all JSON objects are converted to a `Dictionary<string, object>` and all JSON arrays are converted to a `List<object>`.

After getting your customData (see above), accessing your data is as simple as accessing from a dictionary.
```csharp
if (beatmapData is CustomBeatmapData customBeatmapData) 
{
    string label = customBeatmapData.beatmapCustomData["_difficultyLabel"]; // possible null reference, dont actually do this!
}
```

To help with this, CustomJSONData provides the extension method `Get<T>(this Dictionary<string, object> dictionary, string key)`. This will return the value as `T` if it exists, else it will return `default(T)`.

Full example of getting a color from an array: 
```csharp

/* json:
  "_notes": [{
      "_time": 10,
      "_lineIndex": 1,
      "_lineLayer": 0,
      "_type": 0,
      "_cutDirection": 1,
      "_customData": {
        "_color": [0.5, 1, 0]
      }
    }
  ]
*/

if (noteData is CustomNoteData customNoteData)
{
    // get the customData
    Dictionary<string, object> dictionary = customNoteData.customData;

    // get the _color array
    // remember that arrays are always a List<object> and must be casted
    List<object> colorArray = dictionary.Get<List<object>>("_color");

    // Convert to floats and save color
    IEnumerable<float> colorFloats = colorArray.Select(n => Convert.ToSingle(n));
    Color color = new Color(colorFloats.ElementAt(0), colorFloats.ElementAt(1), colorFloats.ElementAt(2)); // (0.5, 1, 0)
}
```

As a reminder, `customData` IS mutable, i.e. you can modify it at any time. Want to store a variable inside a note?
```csharp
customNoteData.customData["colorz"] = new Color(0, 0, 1);
// insert code here
Color noteColor = customNotedata.Get<Color>("colorz"); // (0, 0, 1);
```

# Custom events
In addition to providing access to the custom data found in info.dat, `CustomJSONData.CustomBeatmap.CustomBeatmapData` provides a new list of `CustomEventData` objects. Not to be confused with `CustomBeatmapEventDatas`, which are Beat Saber lighting events with custom data added, this is a place for entirely new events added to the game by plugins. These events are stored in difficulty `.dat` files. Here's an example of what a custom event might look like inside a difficulty:
```json
"_version": "2.0.0",
"_customEvents": [{
      "_time": 0.0,
      "_type": "HelloWorld",
      "_data": {
        "foo": 1.0,
        "message": "Hello from a custom event!"
    }
  }
],
"_events":[
// ...
```
A CustomEventData object has three fields.
* `time` functions identically to the `time` field on notes/obstacles/lighting events.
* `type`, unlike in lighting events, is a string; use it to specify what sort of event this is. Event types are de-facto defined/standardized by the first plugin to make use of them.
* `data` is custom data. To see how to access it, see [Reading custom data](#Reading-custom-data))

To subscribe to these events, you must register a callback through the `CustomEventCallbackController`. When a `CustomEventCallbackController` is instantiated, it will invoke the `didInitEvent` event.

From there you can invoke `AddCustomEventCallback(CustomEventCallback callback, float aheadTime = 0, bool callIfBeforeStartTime = true)`. `aheadTime` is how long in seconds before the event you want the callback to trigger. `callIfBeforeStartTime` is whether or not to trigger callbacks for events that happen before the song start, for example if the player starts in the middle using practice mode.

`AddCustomEventCallback` will return a `CustomEventCallbackData` object which you can use to deregister your callback if you wish using `RemoveBeatmapEventCallback(CustomEventCallbackData callbackData)`

Example:
```csharp
internal void OnEnable() 
{
    CustomEventCallbackController.didInitEvent += OnCustomEventCallbackControllerInit;
}

internal static void OnCustomEventCallbackControllerInit(CustomEventCallbackController customEventCallbackController)
{
    CustomEventCallbackData callback = customEventCallbackController.AddCustomEventCallback(Callback);
    ////customEventCallbackController.RemoveBeatmapEventCallback(callback);
}

internal static void Callback(ustomEventData customEventData) 
{
    if (customEventData.type == "HelloWorld")
    {
        string message = customEventData.data.Get<string>("message"); // "Hello from a custom event!"
    }
}
```

`CustomEventCallbackController` also exposes some useful properties. 
* `BeatmapData` (This is a `CustomBeatmapData`)
* `BeatmapObjectCallbackController`
* `AudioTimeSource`
* `SpawningStartTime`