using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace KeyboardMouseRecorder
{
    public class Registration
    {
        private String name;
        public static Registration rec_istance;
        //coda con azioni eseguite
        private static Queue mouseQueue = new Queue();
        private static Queue keyboardQueue = new Queue();
        private long startTimestamp;
        private long endTimestamp;
        private string desc;
        public string SArray { get; private set; }

        /**
        * name - nome della registrazione
        **/
        public Registration(string name, string descrizione)
        {
            rec_istance = this;
            //creo cartella se non esiste e creo file rec
            this.createDirectory(name);
            //avvio listener tastiera
            KeyboardHook.StartHook();
            //avvio listener mouse
            MouseHook.StartHook();
            startTimestamp = (DateTimeOffset.Now.ToUnixTimeMilliseconds());
            desc = descrizione;
        }

        /**
         * rec_name - nome della registrazione
         **/
        private void createDirectory(string rec_name)
        {
            this.name = rec_name;
            //creo file json della registrazione
            System.IO.FileInfo file = new System.IO.FileInfo("C:\\CustomFolder\\Single\\" + rec_name + ".json");
            
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            System.IO.File.WriteAllText(file.FullName, "");
        }

        //aggiungo azione alla coda
        public Queue getMouseQueue()
        {
            return mouseQueue;
        }
        public Queue getKeyboardQueue()
        {
            return keyboardQueue;
        }
        
        //fermo registrazione
        public void stopRegistration(long endTS)
        {
            Debug.WriteLine("Fine rec");
            //salvo l'ora della fine
            endTimestamp = endTS;
            //chiudo hook
            KeyboardHook.StopHook();
            MouseHook.StopHook();
            //inizializzo le liste
            List<Action> keyboardList = new List<Action>();
            List<Action> mouseList = new List<Action>();

            //riempio la lista della tastiera,togliendo  i tasti della combinazione
            int temp = 0;
            int contatoreAzioni = 0;
            foreach (KeyboardAction i in keyboardQueue)
            {
                contatoreAzioni++;
                keyboardList.Add(i); 
                temp++;
            }    
            //inverto lista
            keyboardList.Reverse();
            int deleted = 0;
            //list element to remove
            List<KeyboardAction> toRemove = new List<KeyboardAction>();
            //clear list from shortcut actions
            foreach (KeyboardAction ka in keyboardList) 
            {    
                Debug.WriteLine(ka);
                //clear list from shortcut actions
                if ((ka.keyboardVK=="160" && ka.keyboardFlags == "0") ||
                   (ka.keyboardVK == "160" && ka.keyboardFlags == "128") ||
                   (ka.keyboardVK == "162" && ka.keyboardFlags == "0") ||
                   (ka.keyboardVK == "162" && ka.keyboardFlags == "128"))
                {
                    toRemove.Add(ka);
                    deleted++; 
                }   
                //se ho fatto tutte le eliminazioni
                if (deleted == 4)
                {  
                    //esco dal ciclo
                    break;
                }
                 
            }  
            //delete them
            keyboardList.RemoveAll(x => toRemove.Contains((KeyboardAction)x));
            //reverse list cleaned
            keyboardList.Reverse();
            //update counter actions
            contatoreAzioni -= 4;
            //fill mouse actions list
            foreach (MouseAction i in mouseQueue)
            {
                contatoreAzioni++;
                mouseList.Add(i);
                temp++;
            }
            
            //merge list
            
            List<Action> mergedList = new List<Action>();
            foreach(Action a in mouseList)
            {
                mergedList.Add(a);
            }
            foreach (Action a in keyboardList)
            {
                mergedList.Add(a);
            }

            mergedList.Sort((x, y) => x.actionUnixTime.CompareTo(y.actionUnixTime));


            //serializing data for json file
            FileJson file = new FileJson
            {  
                nomeFile = name,  
                descrizione = desc,
                startTimestamp = startTimestamp,
                endTimestamp = endTimestamp,
                totalTimestamp = ((long)DateTimeOffset.FromUnixTimeMilliseconds(endTimestamp).DateTime.Subtract(DateTimeOffset.FromUnixTimeMilliseconds(startTimestamp).DateTime).TotalMilliseconds),
                numeroAzioni = contatoreAzioni,
                actionsList = mergedList,
                //MouseList = mouseList,
                //KeyboardList = keyboardList,
                risX = SystemParameters.PrimaryScreenWidth,
                risY= SystemParameters.PrimaryScreenHeight 
            };
            file.type = "FileJson";
            string json = JsonConvert.SerializeObject(file, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"C:\\CustomFolder\\Single\\"+this.name+".json", json);

            
            //clear queue
            mouseQueue = new Queue();
            keyboardQueue = new Queue();

            //close registration window
            RecWindow.window_istance.hideWindow();
            //reopen main window
            MainWindow.window_istance.openWindow();
            Debug.WriteLine("Registrazione finita");
            MessageBox.Show("Registrazione effettuata con successo\nFile salvato in C:/Stresstester/Single");

        }

    }
}

