using System;
using System.Collections.Generic;
using System.Text;

namespace KeyboardMouseRecorder
{
    public class Action
    {
        public long actionUnixTime;
    }
    public class KeyboardAction : Action
    {
        public string keyboardVK;
        public string keyboardScancode;
        public string keyboardFlags;
        public string type = "keyboard";
        
        
    }
    public class MouseAction : Action
    {

        public int wParam;
        public int mouseX;
        public int mouseY;
        public int mouseCode;
        public string type = "mouse";
    }
}
