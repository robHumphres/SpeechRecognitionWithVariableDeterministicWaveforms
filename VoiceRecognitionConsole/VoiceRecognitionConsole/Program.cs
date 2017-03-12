using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Media;
using System.IO;
using System.Diagnostics;


namespace VoiceRecognitionConsole
{
    class Program
    {

        //Member variables
        static SpeechRecognitionEngine _recognizer = null;
        static ManualResetEvent manualResetEvent = null;

        private static WMPLib.WindowsMediaPlayer wplayer;
        static int passes = 0;
        static List<string> finalRecords = new List<string>();
        static string[] ScriptToRead = new string [] {
            @"C:\Users\for example john\Desktop\Project for Tappan\ScriptForSpeedDownHalf.txt",
            @"C:\Users\for example john\Desktop\Project for Tappan\ScriptForSpeedUp2x.txt",
            @"C:\Users\for example john\Desktop\Project for Tappan\ScriptForNeg12DBFiles.txt",
            @"C:\Users\for example john\Desktop\Project for Tappan\ScriptForNeg5DBFiles.txt"
        };


        static void Main(string[] args)
        {
            //Start up windows player
            wplayer = new WMPLib.WindowsMediaPlayer();

            //start thread for event class
            manualResetEvent = new ManualResetEvent(false);

            //Confirm that it's up and running.
            Console.WriteLine("It's up and running");

            //get dictionary setup and running the engine
            RecognizeSpeechAndWriteToConsole();

            //Creating a stop watch to see how long the tests took
            Stopwatch stock = new Stopwatch();
            stock.Start();

            //lambda thread for going through tests
            (new Thread(() =>
            {
                List<string> fileLocations = new List<string>();
                foreach (string fileToRead in ScriptToRead)//@"C:\Users\for example john\Desktop\Project for Tappan\ScriptForNeg5DBFiles.txt"
                    using (StreamReader reader = new StreamReader(fileToRead))
                    {
                        string temp = "";
                        while ((temp = reader.ReadLine()) != null)
                            fileLocations.Add(temp);//add to list to use in windows media player

                        Console.WriteLine("count of read in " + fileLocations.Count);
                        WaitNSeconds(5);
                        foreach (string s in fileLocations)
                        {
                            for (int n = 0; n < 20; n++)
                            {
                                wplayer.URL = s;//@"C:\Users\for example john\Desktop\Project for Tappan\Waveforms\Test\test_Pos12db.wav";
                                wplayer.controls.play();
                                WaitNSeconds(5);
                            }//end of for loop

                            finalRecords.Add(s + "Got " +passes+ " passes out of 20 ");
                            passes = 0;

                        }//end of foreach

                        //after each test need to clear the list before a new script starts up again
                        fileLocations.Clear();

                    }//end of using



                manualResetEvent.Set();
            })).Start();
            
            manualResetEvent.WaitOne();
            Console.WriteLine("Passed wait one");

            foreach (string s in finalRecords)
                Console.WriteLine(s);

            stock.Stop();
            Console.WriteLine("The Amount of time that this process took was... {0:hh\\:mm\\:ss}", stock.Elapsed);
            Console.ReadLine();

            

        }//end of main 



        /// <summary>
        /// This starts up the speech engine and loads the words into the custom dictionary
        /// </summary>
        static void RecognizeSpeechAndWriteToConsole()
        {
            _recognizer = new SpeechRecognitionEngine();

            //Loading up the dictionary
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("test"))); 
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("exit"))); 
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("time"))); 
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("simulation"))); 
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("golf"))); 
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("radio frequency"))); 
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("beer"))); 
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("hardware"))); 
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("software"))); 






            _recognizer.SpeechRecognized += _recognizeSpeechAndWriteToConsole_SpeechRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _recognizeSpeechAndWriteToConsole_SpeechRecognitionRejected; // if recognized speech is rejected, call the specified method
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous


        }//end of console method


        /// <summary>
        /// This method collects the event if the speech is recognized and will print to the console what word it thought it was and add to the number of pass attempts for that field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void _recognizeSpeechAndWriteToConsole_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "test":
                    Console.WriteLine("Test");
                    passes++;
                    break;
                case "exit":
                    Console.WriteLine("exit");
                    passes++;
                    break;
                case "time":
                    Console.WriteLine("time");
                    passes++;
                    break;
                case "simulation":
                    Console.WriteLine("simulation");
                    passes++;
                    break;
                case "golf":
                    Console.WriteLine("golf");
                    passes++;
                    break;
                case "radio frequency":
                    Console.WriteLine("Radio frequency");
                    passes++;
                    break;
                case "beer":
                    Console.WriteLine("beer");
                    passes++;
                    break;
                case "hardware":
                    Console.WriteLine("hardware");
                    passes++;
                    break;
                case "software":
                    Console.WriteLine("software");
                    passes++;
                    break;
            }

           
        }

        /// <summary>
        /// If there's a rejection in the speech and it's close to a word in the dictionary based on the ones added in _recognizeSpeechAndWriteToConsole_SpeechRecognized
        /// it'll print to console the word it thought you meant, it will not count as a pass since it's techinically confused.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void _recognizeSpeechAndWriteToConsole_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech rejected. Did you mean:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
        }//end of recognize speech


        /// <summary>
        /// A wait timer that doesn't stop the ui thread waiting for it to go through.
        /// </summary>
        /// <param name="timer"></param>
        private static void WaitNSeconds(int timer)
        {
            if (timer < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(timer);
            while (DateTime.Now < _desired)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }

    }
}
