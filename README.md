# KeyboardMouseRecorder
Application WPF in C# to record mouse and keyboard actions of a user under Windows system

![screenshot app](images/screenshot.PNG)

This application allows you to record the actions performed by the user via mouse and keyboard.
At start-up, a name and description of the "Registration" phase are requested. Once started, a red window will appear at the bottom right containing the shortcut to execute to stop recording.

The recordings are saved on disk C, inside a folder called "CustomFolder". Recordings are saved in JSON format with this structure:
