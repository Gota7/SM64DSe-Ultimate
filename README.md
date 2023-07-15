## Installation

You can install it from the release section.


# my-improved-version-of-sm64dse
This is an edited version of sm64dse 2.3.5 by Fiachra

The .exe file is in bin/release

Requires at least Visual Studio 2015

<h3>Changelog:</h3><br/>
-invisible pole now has an indicator for its height
<br/>
-custom Model for the StarMarker
<br/>
-painting shows its tilt properly
<br/>
-exit objects show their whole trigger area, also tilted
<br/>
-path objects draw the node-connecting lines when highlighted
<br/>
-added buttons for copying and pasting the objects coordinates
<br/>

<h4>Changes v1.1:</h4><br/>
-new interface for editing object properties
<br/>
-paths now show up as closed if they are
<br/>
-double clicking an object in the objectlist snaps the camera to it (for example usefull to find the level entrances)
<br/>
-in the 3d model tab you can select single areas to show up (pretty usefull to see what is going to be visible when entering a door)
<br/>
-some BugFixes
<br/>
<h4>Changes v1.2:</h4><br/>
-views now have index numbers like entrances
<br/>
-nodes are now ordered by their path
<br/>
-adding new PathNodes is much easier
<br/>
-the parameters for the paths startindex and its length are removed as they aren't needed and would screw up everything
<br/>
-saving won't screw up your paths anymore (at least i hope so)
<br/>

<h4>Changes v1.3:</h4><br/>
-if Model 3D Tab is active and an area is selected, the editor will only show the models that will be rendered
<br/>
-updated and added some star / star marker models, so you know what object you are actually dealing with
<br/>
-filetype filtering for selecting a: BMD, KCL, BTP, BCA, NCG/ICG, NCL/ICL, ISC/NSC -file
<br/>
-replacing a texture in the Texture Editor should now be possible for every texture, but you have to make sure that the correct palette is selected, because the auto selected one is not always what you want
<br/>

<h4>Changes v1.4:</h4><br/>
-i finally got rid of the old property table interface
<br/>
-there is an extra window for editing raw parameters, it has some conversion features
<br/>
-i added object specific parameters for Paintings, Stars and Star Markers
<br/>
-i fixed the one missing line of the Pole Renderer (i doubt anybody recognized, but it looks more right now)
<br/>
-btw. figured out a 4th parameter for Painting, its kind of a mode
<br>

<h4>Changes v1.4.1:</h4><br/>
-i fixed some things regarding the new Raw Editor of 1.4
<br/>
-the exit area markers are a bit thinner now so selecting entrances will be easier in some cases
<br/>
-the bowser puzzle renderer wont crash anymore when setting the piece index too high
<br/>

<h4>Changes v1.5</h4><br>
-added parameters for goomba
<br/>
-added modelDisplaySettings where you can enable/disable textures, vertexcolors, wireframe and a DrawCallType indicator
<br/>
-swapped hex dec for ushort, just realized that they are wrong
<br>
-single areas in the 3d model tab now show their texture animation

<h4>Changes v1.6</h4><br>
-you can choose if you want to see the actual Level Names in the levellist, pretty usefull if you can't remember where you put your levels
<br/>
-hovering over an act in the in the actbar (at the top of the level editor) tells you the mission title if availible
<br/>
-there are some more functions for the level display settings (including: perspectives, view distance and FOV(EXPERIMENTAL))
<br/>
-there is a completely new texture animation editor, editing and creating textures has never been easier, it has a UV preview, a texture preview, a timeline and support for KEYFRAMES!

<h4>Changes v1.7</h4><br/>
-enabled lightning for proper shading
<br/>
-configured the model render to apply texture more accurate, especially for stars
<br/>
-better Goomboss renderer showning it's position and path
<br/>
-added a FlamethrowerRenderer showing the right position, rotation and (accurate) length
<br/>
-added preview scale widget for the Model Animation Editor
<br/>
-if you want to choose an animation for a model, you will automaticly start in the directory of the model
<br/>
-added an option to merge all files from the archieves to the fileSelect