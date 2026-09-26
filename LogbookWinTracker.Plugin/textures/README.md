Use this folder for IMAGES referenced from your json files.

The tutorial to get you up and running is located [here](https://github.com/Monster-Train-2-Modding-Group/Trainworks-Reloaded/wiki)

I find its best to organize the file structure like so.

* banners - Store the clans 6 banner art images here.
  * _Disabled - Used when you are on the opposite track
  * _Enabled - Used when you on on the track with the node. And when its visited
  * _Frozen - Used when the ring this node is on is after the node you are on.
  * _MapIcon - The Map Icon for the node
  * _Visited_Disabled - Used when you have claimed the banner reward
  * ClanBannerMinimapIcon - Icon used in the mini map screen. Should always be this image provided.
* card_art - Store card images here. Card Images can be as small as you want. The examples are 256x256. (Best to keep as small as possible for filesize of mod).
* card_borders - Store the 3 card border images. The images provided can be used as a template.
  * Equipment and room cards share a border image the square image
  * Spell cards have a sideways hexagonal border
  * Unit cards have a longways hexagonal border with the top having the pointed end.
* character_art - Store character art images / skeletons here. These should be png images with transparency and can be as large or small as you want. The images provided are 256x256
* icons - Miscellaneous icons.
* relic_art - Artwork for artifacts here.

Special template images
 * icons/EmptyEquipmentIcon - Base icon for equipment. To make it look good don't draw in the bottom right quadrant outside the circle.
 * icons/EmptyRoomIcon - Base icon for rooms.
 * icons/EmptyPortrait - Empty champion portrait.
 
The example images provided conform to the sizes required for the images in game.


