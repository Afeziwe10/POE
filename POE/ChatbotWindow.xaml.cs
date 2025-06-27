using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Speech.Recognition;
using static CyberSecurityChatbot.RemindersWindow;
using CyberSecurityChatbot;
using System.Text.RegularExpressions;

namespace CyberSecurityChatbot
{
    /// <summary>
    /// Interaction logic for ChatbotWindow.xaml
    /// </summary>
    public partial class ChatbotWindow : Window
    {
        private string logFile = $"ChatLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";  //log file per session

        private SpeechRecognitionEngine recognizer ; //Voice recognition engine
        private bool isListening = false ;  //Mic state tracker

        private RemindersWindow remindersWindow ; //Reminder manager window

        private int userQuestionCount = 0 ; //Question counter for quiz prompt
        private bool quizOffered = false ; //Flag to ensure quiz offered once

        private List<string> activityLog; //Session activity log
        private string currentUser; //Current logged in user

       // MyProfileWindow profileWindow = new MyProfileWindow (currentUser, activityLog);
        
        //Cybersecurity definitions
        private Dictionary<string, string> cyberDefinitions = new Dictionary<string, string>
        {

            {"password", "A strong password should have atleast 12 long, and also include symbols, numbers, upper case, and lower case. You may use a password manager to store them securely." },
            {"phishing", "phishing is a scam where attackers  trick you into providing sensitive information. Always verify links and sender details." },
            {"firewall", "A firewall  protect your network by blocking unauthorized access and filtering traffic." },
            {"vpn", "A VPN encrypts your internet traffic, keeping your data  safe and private from hackers." },
            {"ransomware", "Ransomware locks your files and demands payment. Always bach up your data and avoid suspicious emails." },
            {"social engineering", "Its when attackers manipulate people into revealing confiidential information, often using psychological tricks." },
            {"data breach", "It usually happens when sensitive information is exposed. Use strong password and enale two-factor authentication, know as (2FA)." },
            {"cybersecurity", "Cybersecurity is a practice of protecting systems, networks, and data from cyber threats and attacks." },
            {"dns", "(Domain Name System) translates website domain names into IP address, allowig users to access the website smoothly." },
            {"malware", "Malware spread through email attachments, malicious websites, USB drives, Software downloads, and phishing attacks." },
            {"antivirus","Antivirus software scans and removes malicious programs." }

        };

        private string currentTopic = ""; //Last Discussed topic tracker
        //List of phishing tips
        List<string> phishingTips = new List<string>
        {
            "Be cautious of emails asking for personal information. Scammers often disguise themselves as trustrated organisations.",
            "Hover over links to preview their destination before before clicking.",
            "Check the sender's email address carefully - fake ones often look very similar to real ones.",
            "Avoid downloading attachements from unknown sources.",
            "When in doubt, contact the organisation directly using official channels."
        };

        //List for password tips
        List<string> passwordTips = new List<string>
        {
            "Use at least 12 characters with a mix of letters, numbers, and symbols.",
            "Avoid using easily guessable info like",
            "Enable two-factor authentication for extra protection.",
            "Don't reuse password across multiple sites.",
            "Use a reputable password manager to store your credentials."
        };

        //List for random tips for vpn
        List<string> vpnTips = new List<string>
        {
            "Always use VPN when connected to public wi-fi.",
            "Choose a VPN provider that doesn't keep logs.",
            "VPNS encrypt your internet traffic to protect it from hackers.",
            "Turn on your VPN before accessing sensitive websites.",
            "A VPN hepls mask your ID address for online privacy."
        };

        //List on random ransomware tips
        List<string> ransomwareTips = new List<string>
        {
            "Regularly back up your data to prevent data loss.",
            "Never open email attachments from unknown sources.",
            "Keep your system and antivirus software updated.",
            "Disable macros in documents recived via email.",
            "Don't click suspecious links or download pirated software."
        };

        //List on social engineering tips
        List<string> socialEngineeringTips = new List<string>
        {
            "Be cautious when someone pressures you to act quickly.",
            "Verify identities before sharing personal information.",
            "Don't click links in unexpected messages, even from known contacts.",
            "Train staff to recognize social engineering tactics.",
            "Attackers often pretend to be in positions of authority-always double-check."
        };

        //List for follow up questions
        List<string> followUps = new List<string>
        {
            "more", "tell me more", "explain","explain", "i don't understand", "give me more", "can you explain"
        };

        //List of default responses for unrecognized questiona
        List<string> defaultResponse = new List<string>
        {
            "Hmm.... I don't have an answer for that. Try asking something related to cybersecurity",
            "Good question! But I may need more details.",
            "That's a tricky one! Maybe try rephrasing it?",
            "Cybersecurity is a braod topic! Could you be more specific please"
        };

        //Constructor with user and activity log
        public ChatbotWindow(string userename, List<string> log)
        {
            InitializeComponent();
            currentUser = userename;
            activityLog = log;
            AppendMessage("bot", $"Welcome {currentUser} Ask me anything about cybersecurity.");
            
            remindersWindow = new RemindersWindow();

            //Load conversation history 
            if (File.Exists(logFile))
            {
                var historyLines = File.ReadAllLines(logFile);
                foreach(string line in historyLines)
                {
                    string lower = line.ToLower();
                    string role = lower.Contains("you") ? "user" : "bot";
                    string content = line.Substring(line.IndexOf(":") + 1).Trim();
                    AppendMessage(role, content);
                }
            }
            //Send message on enter key press
             UserInput.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Enter) SendButton_Click(null, null); };

            recognizer = new SpeechRecognitionEngine();
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.LoadGrammar(new DictationGrammar());

            recognizer.SpeechRecognized += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    UserInput.Text = e.Result.Text;
                });
            };

            recognizer.RecognizeCompleted += (s, e) =>
            {
                isListening = false;
            };
        }
        //Track state for reminder flow
        private bool awaitingReminderConfirmation = false;
        //private List<Reminder> reminders = new List<Reminder>();
        private Reminder pendingReminder = null;  
        
        //Handle send button click or Enter key
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) 
                return;

            AppendMessage("user",  input);
            LogMessage("You", input);   
            UserInput.Clear();

            userQuestionCount++;
            //Offer quiz after 10 question
            if(userQuestionCount >= 10 && !quizOffered)
            {
                AppendMessage("bot", "Don't you want to try a cybersecurity quiz and check your knowledge? (yes/no)");
                quizOffered = true;
                return;
            }

            string question = input.ToLower();
            string response = "I'm not sure. Can you rephrase that?";

            Random random = new Random();

            bool isFollowUp = followUps.Any (f => question.Contains (f));
            bool foundTopic = false;

            //Handle reminder confirmation
            if (awaitingReminderConfirmation && pendingReminder != null)
            {
                if (question.Contains("yes") && question.Contains("remind me"))
                {
                    int days = ExtractDays(question);
                    if (days > 0)
                    {
                        pendingReminder.DueDate = DateTime.Now.AddDays(days).ToShortDateString();
                        pendingReminder.IsCompleted = false;

                        if(remindersWindow == null || !remindersWindow.IsVisible)
                        {
                            remindersWindow = new RemindersWindow();
                            remindersWindow.Show();
                        }
                        
                        remindersWindow.AddReminder(pendingReminder);
                        
                        AppendMessage("bot", $"Got it! I'll remind you in {days} days."); 
                        LogMessage("Bot", $"Reminder set for task '{pendingReminder.Title}' in {days} days.");

                        pendingReminder = null;
                        awaitingReminderConfirmation = false ;
                        return;
                    }
                }

                AppendMessage("bot", "Okay, no reminder will be set");
                pendingReminder = null;
                awaitingReminderConfirmation = false ;
                return;
            }
            //Handle quiz prompt response
            if (quizOffered)
            {
                if (question.Contains("yes"))
                {
                    QuizWindow quizWindow = new QuizWindow(activityLog);
                    quizWindow.Show();
                    this.Close();
                    return;
                }
                else if (question.Contains("no"))
                {
                    AppendMessage("bot", "No problem, lets continue chatting.");
                    quizOffered = false ;
                    userQuestionCount = 0;
                    return; 
                }
            }
            //Task adding with optional reminder 
            if (question.StartsWith("add task -"))
            {
                string taskTitle = input.Substring(10).Trim();
                pendingReminder = new Reminder
                {
                    Title = taskTitle,
                    Description = taskTitle,
                    IsCompleted = false
                };

                AppendMessage("bot", $"Task added with the description\"{taskTitle}\". Would you like a reminder?");
                awaitingReminderConfirmation = true ;

                activityLog.Add($"Task addeed: {taskTitle}");
                return;  
            }
            //If it's a follow-up, and a topic was previously discussed
            if (isFollowUp && !string.IsNullOrEmpty(currentTopic))
            {
                switch (currentTopic)
                {
                    case "phishing":
                        response = phishingTips[random.Next(phishingTips.Count)];
                        foundTopic = true;
                        break;
                    case "password":
                        response = passwordTips[random.Next(passwordTips.Count)];
                        foundTopic = true;
                        break;
                    case "vpn":
                        response = vpnTips[random.Next(vpnTips.Count)];
                        foundTopic = true;
                        break;
                    case "ransomware":
                        response = ransomwareTips[random.Next(ransomwareTips.Count)];
                        foundTopic = true;
                        break;
                    case "social engineering":
                        response = socialEngineeringTips[random.Next(socialEngineeringTips.Count)];
                        foundTopic = true;
                        break;
                    default:
                        response = "Sure, but could you clarify which topic you'd like more information on ?";
                        foundTopic = true;
                        break;
                }
            }
            //Checking for tips question using keywords
            else if (question.Contains("phishing") && question.Contains("tip"))
            {
                response = phishingTips[random.Next(phishingTips.Count)];
                currentTopic = "phishing";
                foundTopic = true;
            }

            else if (question.Contains("password") && question.Contains("tip"))
            {
                response = passwordTips[random.Next(passwordTips.Count)];
                currentTopic = "password";
                foundTopic = true;
            }

            else if (question.Contains("vpn") && question.Contains("tip"))
            {
                response = vpnTips[random.Next(vpnTips.Count)];
                currentTopic = "vpn";
                foundTopic = true;
            }

            else if (question.Contains("ransomware") && question.Contains("tip"))
            {
                response = ransomwareTips[random.Next(ransomwareTips.Count)];
                currentTopic = "ransomware";
                foundTopic = true;
            }

            else if (question.Contains("socia engineering") && question.Contains("tip"))
            {
                response = socialEngineeringTips[random.Next(socialEngineeringTips.Count)];
                currentTopic = "social engineering";
                foundTopic = true;
            }

            else
            {

                //Loop through predefined responses and find a match
                foreach (var key in cyberDefinitions.Keys)
                {
                    if (question.Contains(key))
                    {
                        response = cyberDefinitions[key];
                        currentTopic = key;
                        foundTopic = true;
                        break;
                    }
                }
            }

            if(!foundTopic)
            {
                response = defaultResponse[random.Next(defaultResponse.Count)];
            }
           
            //Selecting user mood
            string mood = ((ComboBoxItem)MoodSelector.SelectedItem).Content.ToString();

            if (mood == "Playful")
                response = "So, here's the scoop: " + response;
            else if (mood == "Friendly")
                response = "Sure!: " + response;
            else if (mood == "Serious")
                response = "Definition: " + response;

            AppendMessage("bot", response);
            LogMessage("Bot", response );
        }

        //Method to add a chat bubble message to the interface
        private void AppendMessage(string sender, string message)
        {
            //Create border element to serve as the message bubble
            Border bubble = new Border
            {
                //Set different background colors for user and bot messages
                Background = sender == "user" 
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(215, 130, 95)) //user color
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(226, 184, 137)), // bot color
                CornerRadius = new CornerRadius(12), //Rounded bubble corners
                Margin = new Thickness(5),// Margin around the bubble
                Padding = new Thickness(10),//Padding inside the bubble
                HorizontalAlignment = sender == "user" ? HorizontalAlignment.Right : HorizontalAlignment.Left //Align user to right, bot to left
            };

            //Create a container for the message text and potential emojis
            StackPanel content = new StackPanel();
            //Add the message text inside the bubble
            content.Children.Add( new TextBlock { Text = message, //The message content
                Foreground = System.Windows.Media.Brushes.Black, //Text color
                TextWrapping = TextWrapping.Wrap, //Allow for wrapping for long text
                MaxWidth = 300} ); //Limit bubble width

            //if message is from bot, add emoji reaction buttons
            if (sender == "bot")
            {
                StackPanel emojiPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal //Arrange emoji buttons horizontally
                };

                //Loop to create emoji buttons with different emoji options
                foreach (string emoji in new[] { "👍", "🤩", "😲", "❌" })
                {
                    Button emojiBtn = new Button 
                    { Content = emoji, //Emoji symbol on the button
                        Width = 30,
                        Height = 30, 
                        Margin = new Thickness(2), //Space between emoji
                        Tag = message //Store the message text for reaction reference
                    };
                    emojiBtn.Click += EmojiReaction_Click; //Event handle for emoji click
                    emojiPanel.Children.Add(emojiBtn); //Add emoji button to the panel
                }

                content.Children.Add( emojiPanel ); //add emoji panel to the bubble content
            }

            bubble.Child = content; //Assign the message content to the bubble
            bubble.Opacity = 0; //Start with bubble hidden for fade-in effect
            ChatStack.Children.Add( bubble ); //Add the bubble to the chat window

            //Create and apply fade-in animation for smooth message appearance
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
            bubble.BeginAnimation(Border.OpacityProperty, fade);
            ChatScrollViewer.ScrollToEnd(); //Automatically scroll to latest message
        }

        private int ExtractDays(string input)
        {
            //Use regex to find a number followed by the word day 'day'
            Match match = Regex.Match(input, @"(\d+)\s*day");
            //if a valid match is found, parse and return the number
            if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
            {
                return days;
            }

            return 0;
        }

        //Handles user emoji reactions whrm clicked
        private void EmojiReaction_Click(object sender, RoutedEventArgs e)
        {
            //Verify sender is a button and retrieve the tagged message
            if (sender is Button btn && btn.Tag is string msg)
            {
                string emoji = btn.Content.ToString();
                LogMessage("Reaction", $"Reacted to \"{msg}\" with {emoji}");
                MessageBox.Show($"You reacted with {emoji}", "Emoji Reaction", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        //logs messages to the text file with a timestamp
        private void LogMessage(string sender, string message)
        {
            File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] {sender}: {message}\n");
        }

        //Saves the current chat log by copying the log file with a timestamp
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(logFile, $"Saved_{DateTime.Now:HHmmss}.txt");
            MessageBox.Show("Conversation saved.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);    
        }

        private void MicButton_Click(object sender, RoutedEventArgs e)
        {
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.LoadGrammar(new DictationGrammar());

            AppendMessage("bot", "🎙️ Listening.....");

            recognizer.RecognizeCompleted += (s, args) =>
            {
                if (args.Result != null)
                {
                    string recognizedText = args.Result.Text;
                    Dispatcher.Invoke(() =>
                    {
                        UserInput.Text = recognizedText;
                        AppendMessage("bot", $"🗣️ You said: {recognizedText}");
                        SendButton_Click(null, null);
                    });
                }

                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        AppendMessage("bot", "⚠️ Couldn't understand your voice input.");
                    });
                }

                recognizer.Dispose();
            };

            recognizer.RecognizeAsync(RecognizeMode.Single);
        }
        //Opens the user profile window with current user details
        private void ProfileButton_Click(Object sender, RoutedEventArgs e)
        {
            MyProfileWindow profile = new MyProfileWindow(currentUser, activityLog);
            profile.Show();
        }
    }
}
