using System.Collections.Generic;

namespace KeyboardMouseRecorder
{
    public class FileAction
    {
        public string type;
    }
    internal class FileJson: FileAction
    {
        public string nomeFile;
        public string descrizione;
        public long startTimestamp;
        public long endTimestamp;
        public long totalTimestamp;
        public int numeroAzioni;
        public double risX;
        public double risY;
        public List<Action> actionsList;
    }
}