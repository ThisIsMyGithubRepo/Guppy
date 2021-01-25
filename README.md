# Guppy
3D Printer Serial Communication Tool, mostly for Marlin. Why Guppy? Marlin is a fish, and Guppies are much smaller fish. Plus I'm not great at naming apps.

Guppy is a Windows .Net WPF Core 3.1 App.

<!-- ABOUT THE PROJECT -->
## About The Project

![Product Name Screen Shot][product-screenshot]

I like to connect to my 3D printer via serial to control it for tuning and adjusting. I'm happy to print via SD, and don't really need something like OctoPrint.

I really don't know what I'm doing, in particular around publishing an app. So, if you have a suggestion or see me doing something wrong - please let me know.

<!-- USAGE EXAMPLES -->
## Usage
### Basic Features
* Connect/Disconnect using...wait for it...the Connect and Disconnect buttons. The app will try to autoconnect if you send a command while disconnected.
* Type commands directly into the command bar. The last 5 commands are remembered and you can scroll through them with the up/down arrow keys to reuse them.
* When you've scrolled to a command, you can use the left arrow to get to the end of it for editing - I find I often go back to a command and want to tweak a parameter. This makes it easier.

### Macros
The app has 20 macro buttons, which can you assign any commands you want. Macros can have more than one command (multi-line). It comes with default commands including some UBL mesh management, but you can edit them to make them as you see fit.
* Single click a macro button to send the commands.
* Right click a macro button to edit it.
* You can drag/drop text from the output window onto a macro button. This will open a macro edit window with your dragged text inserted. The app will sanitize the text, stripping out any "echo" statements to make them runnable:
![Product Name Screen Shot][text-drag-drop-screenshot]

### Special Command Responses
The app will notice if certain responses are received from the printer, currently just M501/M503 config output and G29 T1 (deliminated mesh) output.
* M501/503 results can be dragged/dropped onto a macro. It will strip off all the non-command stuff leaving you with just the commands you would send to re-set your conifg. This should make it easier to bridge a firmware upgrade.
![config-to-marco-screenshot]
* G29 T1 Mesh Result will be processed, and let you double-click to see a mesh visualization.
![mesh-view-screenshot]

<!-- ROADMAP -->
## Roadmap
Literally while typing this, I realize I should make the app process mesh gcode results. You can highlight and drag/drop G29 S-1 output and it'll "commandify" it, but a nice processed result would be better.

<!-- Install -->
## Install
I have no idea how I should distribute this - maybe send a hint my way?
You can obviously download the source and build it, if you have VSStudio.

You can also download [Guppy.zip][exe-link] and inside you'll find a single-file-exe built for Win x64 .Net 3.1, that migh run for you? Runs for me. I have no idea how to make you feel even remotely safe about downloading and running a random exe from the internet.

<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [WPFSurfacePlot3D](https://github.com/brittanybelle/wpf-surfaceplot3d)
* [Best Readme Template](https://github.com/othneildrew/Best-README-Template) which I largely ignored :(


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[product-screenshot]: Images/M503ProcessedResponse.jpg
[text-drag-drop-screenshot]: Images/TextDragDropCommandified.jpg
[config-to-marco-screenshot]: Images/ConfigTurnedIntoMacro.jpg
[mesh-view-screenshot]: Images/MeshView.jpg
[exe-link]: /Guppy.zip
