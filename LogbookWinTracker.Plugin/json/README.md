Use this folder to create json files specifying your mod.

The tutorial to get you up and running is located [here](https://github.com/Monster-Train-2-Modding-Group/Trainworks-Reloaded/wiki)

I find its best to organize the file structure like so.

json/

* plugin.json - Only present if you did not generate a minimal clan. Has configuration to change the Firestarter starter card of Pyreborne.
* global.json - Useful to include definitions used across all files for readability/searchability. Currently contains a Consume trait that is used in various places if minimal clan is generated.
* artifacts/ - Artifact definitions defined here.
* champions/ - Define the two champion cards data here, along with the champion upgrade paths and various other UI sprites champions have.
* clan/ - Define the clan definition along with the banner, subtypes used, and the clan's card style.
* enhancers/ - Define the shop upgrades this clan gets
* equipment/ - Equipment cards
* rooms/ - Room cards
* spells/ - Spell cards included in the clan
* status_efffects/ - Custom status effect definitions if any
* units/ - Unit cards included in the clan

Be sure when you add a json file to add it to your `Plugin.cs` `AddMergedJsonFile` line otherwise it won't be loaded!

When you build your mod in the github codespace (or Visual Studio if running locally) simply copy the built files (you'll need to download from the codespace, its helpful to build a .zip file) into the BepinEx/plugins directory.
