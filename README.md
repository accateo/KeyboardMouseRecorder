# KeyboardMouseRecorder
Application WPF in C# to record mouse and keyboard actions of a user under Windows system

![screenshot app](images/screenshot.PNG)

This application allows you to record the actions performed by the user via mouse and keyboard.
At start-up, a name and description of the "Registration" phase are requested. Once started, a red window will appear at the bottom right containing the shortcut to execute to stop recording.

The recordings are saved on disk C, inside a folder called "CustomFolder". Recordings are saved in JSON format with this structure:

{
  "nomeFile": "test",
  "descrizione": "test",
  "startTimestamp": 1655836170939,
  "endTimestamp": 1655836174394,
  "totalTimestamp": 3455,
  "numeroAzioni": 2,
  "risX": 1600.0,
  "risY": 900.0,
  "actionsList": [
    {
      "wParam": 512,
      "mouseX": 623,
      "mouseY": 387,
      "mouseCode": 0,
      "type": "mouse",
      "actionUnixTime": 1655836171916
    },
    {
      "keyboardVK": "70",
      "keyboardScancode": "33",
      "keyboardFlags": "128",
      "type": "keyboard",
      "actionUnixTime": 1655836173045
    }
  ],
  "type": "FileJson"
}

where you can see:
- the name and the description of the file
- the start and end timestamp of the recording 
- the total duration in milliseconds of the recording
- the number of actions performed in total
- the pixel resolution of the screen from which we recorded
- the list of actions performed

For the mouse we have:
- the point in X and Y pixel coordinates
- the type of action (movement, click and double click)
- the timestamp of the action
For the keyboard we have:
- VK Code, ScanCode and Flag of the pressed key
- timestamp of the action 
For further information about code -> https://docs.microsoft.com/en-us/windows/win32/inputdev/user-input
