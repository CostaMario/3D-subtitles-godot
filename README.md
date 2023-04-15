# See sounds

These scripts allow you to add 3D worldized subtitles in your Godot 4 games.

# How to use

Simply download the scripts and add them to your project, then add "subtitle_manager.tscn" as a singleton using Godot's autoload system,
then set the "Subtitle 3d Instance Path" property on the Subtitle Manager to the path of "subtitle_3d_instance.tscn".

Now add the Subtitled Sound class to any AudioStreamPlayer3D, and a subtitle will be displayed every time that sound is triggered.
Use the visual volume multiplier to affect how far the subtitle will be displayed (this will be removed in favor of automatically detecting the loudness of a sound stream later on).
