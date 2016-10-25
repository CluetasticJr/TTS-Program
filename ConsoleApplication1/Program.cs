using FormatUtils;
using SpeechLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Speech.Synthesis;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            SpVoice speech = new SpVoice();
            Dictionary<string, SpVoice> playerVoices = new Dictionary<string, SpVoice>();
            List<SpObjectToken> voices = new List<SpObjectToken>();

            List<string> approvedVoiceNames = new List<string>();
            foreach (InstalledVoice v in (new SpeechSynthesizer()).GetInstalledVoices())
            {
                if (v.VoiceInfo.Culture.TwoLetterISOLanguageName == "en")
                {
                    if (v.VoiceInfo.Name != "Microsoft David Desktop" || v.VoiceInfo.Name != "SampleTTSVoice" || v.VoiceInfo.Name != "VW Paul") approvedVoiceNames.Add(v.VoiceInfo.Name);
                }
            }
            Console.WriteLine("Minecraft Chat Speaker Clueless Edition V1.0.0.1");
            Console.WriteLine("Please type the path to the Minecraft log file.");
            Console.WriteLine("The file will be located inside the log folder of your Minecraft directory.");
            Console.WriteLine("The file will be called latest.log");
            string logpath;
            logpath = Console.ReadLine();

            foreach (SpObjectToken token in (new SpVoice()).GetVoices("", ""))
            {
                if (approvedVoiceNames.Contains(token.GetAttribute("Name"))) voices.Add(token);
            }

            foreach (InstalledVoice v in (new SpeechSynthesizer()).GetInstalledVoices())
            {

                foreach (InstalledVoice voice in (new SpeechSynthesizer()).GetInstalledVoices())
                {
                    VoiceInfo info = voice.VoiceInfo;
                    Console.WriteLine("Voice Name: " + info.Name);
                }
                // loops forever, and ever and ever and ever.
                // probably should do something better here for clean shutdown.
                while (1 == 1)
                {
                    FileStream theFile = null;
                    StreamReader readMe = null;
                    try
                    {
                        //theFile = new FileStream("C:\\Users\\CluetasticJr\\AppData\\Roaming\\.minecraft\\logs\\latest.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        theFile = new FileStream(logpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        readMe = new StreamReader(theFile);
                    }
                    catch (FileNotFoundException)
                    {
                        // don't care if the file isn't there yet, just keep looping until it is.
                        Thread.Sleep(1000);
                        continue;
                    }

                    // we know the file exists and we have it open, start reading from it.
                    // but we only care about new text that shows up, ignore anything that was
                    // already in the file to start with.
                    readMe.ReadToEnd(); // ignore what's there.

                    // loop and keep reading new stuff.
                    // again, should have a real loop exit for clean shutdown.
                    while (1 == 1)
                    {
                        string stuff = readMe.ReadLine();
                        if (stuff != null && stuff.Length > 0)
                        {
                            // this is where you would put the API calls to turn text to speech.
                            if (MinecraftFormatUtils.IsLineChat(stuff))
                            {
                                string stripped = MinecraftFormatUtils.RemoveColorCodes(MinecraftFormatUtils.GetChatContent(stuff));
                                string playerName = MinecraftFormatUtils.GetPlayerName(stripped);
                                if (!playerVoices.TryGetValue(playerName, out speech))
                                {
                                    speech = new SpVoice(); //initialize the text to speech
                                    speech.Voice = voices[(new Random()).Next(voices.Count)]; //randomly chooses the specific tts voice
                                    playerVoices[playerName] = speech; //set the tts voice to the previously chosen voice
                                }
                                try
                                {
                                    string trimmed = MinecraftFormatUtils.TrimPlayerName(stripped);
                                    if (trimmed.StartsWith("<")) trimmed = string.Format("less than sign {0}", trimmed.Substring(1));
                                    Console.WriteLine(stripped);
                                    speech.Speak(trimmed, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }
    }
}