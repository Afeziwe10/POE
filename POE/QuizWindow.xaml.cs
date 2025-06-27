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

namespace CyberSecurityChatbot
{
    /// <summary>
    /// Interaction logic for QuizWindow.xaml
    /// </summary>
    public partial class QuizWindow : Window
    {
        private List<Question> quizQuestions = new List<Question>(); //List of quiz question
        private int currentQuestionIndex = 0; //Tracks current question number
        private int score = 0; //Tracks user score
        private List<string> activityLog; //reference to main activity log
        public QuizWindow(List<string> log)
        {
            InitializeComponent();
            activityLog = log;
            LoadQuestions(); //Populate quiz qith question
            DisplayQuestion(); //Show first question
        }

        private void LoadQuestions()
        {
            //Loads all quiz questions with options and correct answers
            quizQuestions.Add(new Question("What should you do if you recieve an email asking for your \npassword?",
                new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                2)); //Correct answer is index 2

            quizQuestions.Add(new Question("True or False: A strong password should include numbers, \nsymbols, and uppercase letters.",
                new List<string> { "True", "False" },
                0));

            quizQuestions.Add(new Question("What is phishing?",
                new List<string> { "A way to catch fish", "An attempt to trick you into revealing personal information", "A type of antivirus software", "A secure website protocol"},
                1));

            quizQuestions.Add(new Question("Which of the following is a safe browsing practice?",
                new List<string> { "Click unknown links", "Disable your firewall", "Keep your browser updated", "Share password with friends"},
                2));

            quizQuestions.Add(new Question("True or False: You should use the same password for all your \naccounts.",
                new List<string> { "True","False"},
                1));

            quizQuestions.Add(new Question("How can you identify a secure website?",
                new List<string> { "The URl Start with http", "It has lots of aids","It uses https and a padlock icon appears", "It loads very slowly"},
                2));

            quizQuestions.Add(new Question("What is social engineering?",
                new List<string> { "Programming social media apps", "Tricking people into giving away information", "Fixing broken computers", "Encrypting your data" },
                1));

            quizQuestions.Add(new Question("Which is the safest option when using public Wi-Fi?",
                new List<string> { "Coonect without protection", "Use a VPN", "Turn Off your firewall", "Share files with strangers" },
                1));

            quizQuestions.Add(new Question("True or False: Antivirus software can protect you from all types \nof cyber threats,",
                new List<string> { "True", "False" },
                1));

            quizQuestions.Add(new Question("What should you do if you suspect a ransomware attack?",
                new List<string> { "Pay the ransom immediately", "Ignore it", "Disconnect from the network and notify IT", "Try to delete random files " },
                2));
        }

        public class Question
        {
            public string Text { get; set; }
            public List<string> Options { get; set; }
            public int CorrectOptionIndex { get; set; }

            //Constructor to initialize a question
            public Question(string text, List<string> options, int correctOptionIndex)
            {
                Text = text;
                Options = options;
                CorrectOptionIndex = correctOptionIndex;
            } 
        
        }

        //Display the current question and answer choices
        private void DisplayQuestion()
        {
            //Check if all question have been answered
            if (currentQuestionIndex >= quizQuestions.Count)
            {
                ShowFinalScore();
                return;
            }

            FeedbackText.Text = "";
            OptionsPanel.Children.Clear(); //Remove old answer option

            var question = quizQuestions[currentQuestionIndex];
            QuestionText.Text = question.Text;

            //Dynamically create radio buttons for each answer option
            for (int i = 0; i < question.Options.Count; i++)
            {
                RadioButton optionBtn = new RadioButton
                {
                    Content = question.Options[i],
                    GroupName = "Options",
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5)
                };
                OptionsPanel.Children.Add(optionBtn);
            }
        }

        //Handles logic when the next button is clicked
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = -1; //Tracks selected answer index

            //Loop to find which radio button is selected
            for (int i = 0; i < OptionsPanel.Children.Count; i++)
            {
                if (OptionsPanel.Children[i] is RadioButton btn && btn.IsChecked == true)
                {
                    selectedIndex = i;
                    break;
                }
            }

            //Ensures the user selects an answer
            if (selectedIndex ==  -1)
            {
                MessageBox.Show("Please select an answer before continuing.", "Quiz", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var question = quizQuestions[currentQuestionIndex];

            //Check if selected answer is corerect
            if (selectedIndex == question.CorrectOptionIndex)
            {
                FeedbackText.Text = "Correct";// + question.GetExplanation();
                score++;
            }
            else
            {
                FeedbackText.Text = "Incorrect";//+ question.GetExplanation();
            }

            currentQuestionIndex++; //move to the next question

            //Update button text for next or final result
            NextButton.Content = currentQuestionIndex < quizQuestions.Count ? "Next Question" : "See Result";

            //Switch button event tp proceed to next question
            NextButton.Click -= NextButton_Click;
            NextButton.Click += ProceedToNext;
        }

        //Procceds to next question when user clicks afyer feedback
        private void ProceedToNext(object sender, RoutedEventArgs e)
        {
            DisplayQuestion();
            NextButton.Content = "Next Question";
            NextButton.Click -= ProceedToNext;
            NextButton.Click += NextButton_Click;
        }

        //Shows the user's final score and closes the quiz
        private void ShowFinalScore()
        {
            MessageBox.Show($"You Scored {score} out of {quizQuestions.Count}.","Quiz Completed", MessageBoxButton.OK, MessageBoxImage.Information);
            activityLog.Add($"Quiz completed - score: {score}/10.");
            this.Close();
        }

        public string GetExplanation ()
        {
            return "Remember to follow cybersecurity best practices.";
        }
    }
}
