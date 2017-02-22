using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        //init speech recognition engine with finnish language package
        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine(new CultureInfo("fi-FI"));

        public Form1()
        {
            InitializeComponent();
        }

        List<string> pikaKommand = new List<string>();

        // start listening withEnable button
        private void btnEnable_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
            btnDisable.Enabled = true;
            btnEnable.Enabled = false;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // list of words to be listened - for example from numbers 1 to 10
            Choices commands = new Choices();
            commands.Add(new string[] { "yksi", "kaksi", "kolme", "neljä", "viisi", "kuusi", "seitsemän", "kahdeksan", "yhdeksän", "kymmenen",
                                        "komento ok",
                                        "väärin" });

            // grammar for the recognizer
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);

            Grammar grammar = new Grammar(gBuilder);

            recEngine.LoadGrammarAsync(grammar);
            recEngine.SetInputToDefaultAudioDevice();
            recEngine.SpeechRecognized += RecEngine_SpeechRecognized;
            
        }

        

        void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //show all recognized words and confidence level -> show them in the box on right side of the window
            richTextBox2.Invoke(new MethodInvoker(delegate { richTextBox2.Text += e.Result.Text+ " Confidence score: "+ e.Result.Confidence +"\n"; }));

            // when confidence level is 0.9 we can determine that recognition is correct
            if (e.Result.Confidence > 0.90) {

                // start creating command list of correct words
                if (e.Result.Text == "komento ok")
                {
                    //with 'komento ok' -> create command phrase from the command list and show it to the user 
                    richTextBox1.Invoke(new MethodInvoker(delegate { richTextBox1.Text += " -> Oikein\n"; }));

                    var komento = "Pikakomento on: ";
                    if (pikaKommand.Count == 0) { komento = komento + " - "; }
                    else
                    {
                        for (int i = 0; i < pikaKommand.Count; i++)
                        {
                            komento = komento + " " + pikaKommand[i];
                        }
                    }

                    MessageBox.Show(komento);
                    pikaKommand.Clear();

                } else if (e.Result.Text == "väärin") {
                    // if there is an incorrect word in commmand list, it can be erased with comman 'väärin'
                    richTextBox1.Invoke(new MethodInvoker(delegate { richTextBox1.Text += " -> Väärä -> "; }));
                    if (pikaKommand.Count!=0)
                    {
                        pikaKommand.RemoveAt(pikaKommand.Count - 1);
                    }
                    
                }
                else {
                    // show all words in the textbox on left and collect them in command list 
                    richTextBox1.Invoke(new MethodInvoker(delegate { richTextBox1.Text += e.Result.Text + " "; }));
                    pikaKommand.Add(e.Result.Text);
                    //Console.WriteLine("--------------------> " + pikaKommand.Count );

                }
      
            }
            

            // DeBug -------------------------------------------------------
            // for debuging purposes write information about the recognition process to console
            Console.ReadLine();
            Console.WriteLine("Recognition result summary:-----------------------------------------------------------------------");
            Console.WriteLine(
              "  Recognized phrase: {0}\n" +
              "  Confidence score {1}\n" +
              "  Grammar used: {2}\n",
              e.Result.Text, e.Result.Confidence, e.Result.Grammar.Name);

            // Display information about the words in the recognition result.
            Console.WriteLine("  Word summary: ");
            foreach (RecognizedWordUnit word in e.Result.Words)
            {
                Console.WriteLine(
                  "    Lexical form ({1})" +
                  " Pronunciation ({0})" +
                  " Display form ({2})",
                  word.Pronunciation, word.LexicalForm, word.DisplayAttributes);
            }

            // Display information about the audio in the recognition result.
            Console.WriteLine("  Input audio summary:\n" +
                  "    Candidate Phrase at:       {0} mSec\n" +
                  "    Phrase Length:             {1} mSec\n" +
                  "    Input State Time:          {2}\n" +
                  "    Input Format:              {3}\n",
                  e.Result.Audio.AudioPosition,
                  e.Result.Audio.Duration,
                  e.Result.Audio.StartTime,
                  e.Result.Audio.Format.EncodingFormat);

            // Display information about the alternate recognitions in the recognition result.
            Console.WriteLine("  Alternate phrase collection:");
            foreach (RecognizedPhrase phrase in e.Result.Alternates)
            {
                Console.WriteLine("    Phrase: " + phrase.Text);
                Console.WriteLine("    Confidence score: " + phrase.Confidence);
            }

            // Display information about text that was replaced during normalization.
            if (e.Result.ReplacementWordUnits.Count != 0)
            {
                Console.WriteLine("  Replacement text:\n");
                foreach (ReplacementText rep in e.Result.ReplacementWordUnits)
                {
                    Console.WriteLine("      At index {0} for {1} words. Text: {2}\n",
                    rep.FirstWordIndex, rep.CountOfWords, rep.Text);
                }
                //label.Text += String.Format("\n\n");

            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("No text was replaced");
            }

            

        }
  
        //stop listening
        private void btnDisable_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsyncStop();
            btnDisable.Enabled = false;
            btnEnable.Enabled = true;
        }

    }
}
