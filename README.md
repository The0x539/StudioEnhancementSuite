Assorted improvements for Bricklink's "Studio 2.0" digital LEGO model editor.

# Features
 - Adjust hardcoded 60 FPS limit
 - High-contrast outlines in the editor for dark-but-not-black colors
   - This currently only works in the editor. I haven't figured out how to do it for instructions
 - Shorthand search (*e.g.*, searching `tr22` produces results for "Tile, Round 2 x 2"
 - Redirect renders to run in Blender instead of Eyesight (Experimental, disabled by default, requires additional setup)
 - Abbreviate part names in the palette to improve readability

# Installation
1. Install [BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html) (**5.x, not 6.x!**) normally.
1. Place the DLL in `Studio 2.0/BepInEx/plugins/`. (If the directory does not exist, launching Studio should create it.)

# Configuration
Edit `Studio 2.0/BepInEx/config/StudioEnhancementSuite.cfg` in a text editor.
Each modification can be individually toggled, and some have additional options.
