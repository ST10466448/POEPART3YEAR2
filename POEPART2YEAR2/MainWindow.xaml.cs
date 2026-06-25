using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Media;
using System.Windows;

namespace POEPART2YEAR2
{
    public partial class MainWindow : Window
    {
        // MEMORY VARIABLES
        private string userName = "";
        private string favouriteTopic = "";
        private List<string> activityLog =
        new List<string>();


        private string connectionString = @"Server=LabVM2049939\SQLEXPRESS;Database=CyberSecurityBotDB;Trusted_Connection=True;TrustServerCertificate=True;";
        private int currentQuestion = 0;
        private int score = 0;

        private List<QuizQuestion> quizQuestions =
            new List<QuizQuestion>()
        {
        new QuizQuestion
    {
        Question = "What is phishing?",
        CorrectAnswer = "A scam designed to steal information"
    },

         new QuizQuestion
    {
        Question = "Should you use the same password for every account?",
        CorrectAnswer = "No"
    },

        new QuizQuestion
    {
        Question = "What does VPN stand for?",
        CorrectAnswer = "Virtual Private Network"
    }
        };



        // RANDOM OBJECT
        Random random = new Random();

        // DELEGATE
        delegate string BotResponse(string input);

        BotResponse responseDelegate;

        // COLLECTION WITH MORE TOPICS + DETAILED RESPONSES
        Dictionary<string, List<string>> responses =
            new Dictionary<string, List<string>>()
        {
            {
                "password",
                new List<string>()
                {
                    "Strong passwords should contain uppercase letters, lowercase letters, numbers, and symbols. Avoid using easy information like birthdays or names because hackers can guess them easily.",

                    "Using the same password for multiple accounts is risky. If one account is hacked, attackers can gain access to all your other accounts as well.",

                    "A password manager can help you generate and store secure passwords safely so that you do not need to remember every password yourself."
                }
            },

            {
                "phishing",
                new List<string>()
                {
                    "Phishing is a cyberattack where criminals pretend to be trusted organisations to trick people into giving away sensitive information such as passwords or banking details.",

                    "Always check email addresses carefully before clicking links. Many phishing emails look real but contain small spelling mistakes or suspicious domains.",

                    "If an email creates urgency such as 'Your account will be locked immediately', it could be a phishing attempt designed to pressure you into acting quickly."
                }
            },

            {
                "malware",
                new List<string>()
                {
                    "Malware is malicious software designed to damage systems, steal data, or spy on users. Examples include viruses, worms, ransomware, and spyware.",

                    "Avoid downloading files from unknown websites because malware is often hidden inside fake software, pirated programs, or suspicious attachments.",

                    "Keeping your operating system and antivirus software updated helps protect your device against newly discovered malware threats."
                }
            },

            {
                "vpn",
                new List<string>()
                {
                    "A VPN, or Virtual Private Network, encrypts your internet traffic and protects your privacy when browsing online.",

                    "Using public Wi-Fi without a VPN can expose your personal information to hackers who may intercept your internet traffic.",

                    "VPNs can also help prevent websites and advertisers from tracking your online activities and collecting your browsing data."
                }
            },

            {
                "privacy",
                new List<string>()
                {
                    "Protecting your privacy online means controlling who can access your personal information and how it is shared.",

                    "Avoid posting sensitive information such as your address, passwords, banking details, or personal documents online.",

                    "Always review privacy settings on social media platforms to control who can view your content and personal details."
                }
            },

            {
                "firewall",
                new List<string>()
                {
                    "A firewall acts as a security barrier between your computer and the internet by blocking suspicious or unauthorised traffic.",

                    "Firewalls help prevent hackers from gaining unauthorised access to your device or network.",

                    "Both hardware and software firewalls are important for protecting systems against cyber threats."
                }
            },

            {
                "antivirus",
                new List<string>()
                {
                    "Antivirus software scans your computer for malicious programs and helps remove threats before they can cause damage.",

                    "Regular antivirus scans are important because cyber threats are constantly evolving and new malware appears every day.",

                    "Keeping your antivirus updated ensures that it can detect the latest cyber threats effectively."
                }
            },

            {
                "social engineering",
                new List<string>()
                {
                    "Social engineering is when attackers manipulate people into revealing confidential information rather than hacking systems directly.",

                    "Cybercriminals often pretend to be trusted individuals such as IT staff, banks, or company employees to gain your trust.",

                    "Always verify a person's identity before sharing passwords, OTPs, or sensitive company information."
                }
            },

            {
                "ransomware",
                new List<string>()
                {
                    "Ransomware is a type of malware that locks or encrypts files and demands payment to restore access.",

                    "Backing up important files regularly can help you recover data without paying cybercriminals.",

                    "Never open suspicious email attachments because ransomware is commonly spread through phishing emails."
                }
            },

            {
                "2fa",
                new List<string>()
                {
                    "Two-factor authentication adds an extra layer of security by requiring a second verification step in addition to your password.",

                    "Even if hackers steal your password, two-factor authentication can still help prevent unauthorised access to your account.",

                    "Authentication apps are usually safer than SMS codes because text messages can sometimes be intercepted."
                }
            },

            {
                "scam",
                new List<string>()
                {
                    "Online scams are designed to trick users into sending money or revealing personal information.",

                    "Be cautious of messages promising prizes, quick money, or urgent requests because these are common scam tactics.",

                    "If something online seems too good to be true, it is usually a scam."
                }
            },

            {
                "cyberbullying",
                new List<string>()
                {
                    "Cyberbullying involves using technology to harass, threaten, or embarrass another person online.",

                    "If you experience cyberbullying, avoid responding to harmful messages and report the behaviour to the platform.",

                    "Saving screenshots of harmful messages can help provide evidence when reporting cyberbullying incidents."
                }
            },

            {
                "data breach",
                new List<string>()
                {
                    "A data breach occurs when sensitive information is accessed, stolen, or exposed without authorisation.",

                    "Companies should encrypt sensitive data and implement strong access controls to reduce the risk of data breaches.",

                    "If you are notified about a data breach, change your passwords immediately and monitor your accounts for suspicious activity."
                }
            },

            {
                "safe browsing",
                new List<string>()
                {
                    "Safe browsing involves visiting trusted websites and avoiding suspicious links or downloads.",

                    "Always look for HTTPS in the website address because it indicates that the connection is encrypted.",

                    "Avoid clicking pop-up advertisements or downloading files from untrusted websites because they may contain malware."
                }
            }
        };

        public MainWindow()
        {
            InitializeComponent();

            LoadTasks();

            QuestionText.Text =
                quizQuestions[currentQuestion].Question;

            DisplayBotMessage("Hello! I am your Cybersecurity Awareness Bot.");
            DisplayBotMessage("What is your name?");

            PlayGreeting();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();

            // ERROR HANDLING
            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayBotMessage("Please enter a message.");
                return;
            }

            DisplayUserMessage(input);

            HandleConversation(input.ToLower());

            UserInput.Clear();
        }

        private void HandleConversation(string input)
        {
            // STORE USER NAME
            if (string.IsNullOrEmpty(userName))
            {
                userName = input;

                DisplayBotMessage($"Nice to meet you, {userName}!");

                return;
            }

            // MEMORY FEATURE
            if (input.Contains("interested in"))
            {
                favouriteTopic = input.Replace("interested in", "").Trim();

                DisplayBotMessage($"I will remember that you are interested in {favouriteTopic}.");

                return;
            }

            // SENTIMENT DETECTION
            if (input.Contains("worried") || input.Contains("scared"))
            {
                DisplayBotMessage("It is understandable to feel worried about cybersecurity threats.");

                DisplayBotMessage("Remember to use strong passwords, avoid suspicious links, and keep your software updated.");

                return;
            }

            if (input.Contains("frustrated") || input.Contains("angry"))
            {
                DisplayBotMessage("I understand your frustration. Cybersecurity can sometimes feel overwhelming, but learning safe practices helps reduce risks.");

                return;
            }

            if (input.Contains("curious"))
            {
                DisplayBotMessage("Curiosity is great for learning cybersecurity and staying informed about online safety.");

                return;
            }

            // CONVERSATION FLOW
            if (input.Contains("tell me more") || input.Contains("another tip"))
            {
                if (!string.IsNullOrEmpty(favouriteTopic))
                {
                    DisplayBotMessage($"Since you are interested in {favouriteTopic}, remember to stay informed and use secure online practices.");
                }
                else
                {
                    DisplayBotMessage("Always keep your software updated and avoid clicking suspicious links or downloading unknown files.");
                }

                return;
            }

            // KEYWORD RECOGNITION
            foreach (var keyword in responses.Keys)
            {
                if (input.Contains(keyword))
                {
                    List<string> possibleResponses = responses[keyword];

                    int index = random.Next(possibleResponses.Count);

                    string selectedResponse = possibleResponses[index];

                    DisplayBotMessage(selectedResponse);

                    return;
                }
            }

            // DELEGATE USAGE
            responseDelegate = GetHelpResponse;

            DisplayBotMessage(responseDelegate(input));
        }

        private string GetHelpResponse(string input)
        {
            return "I did not understand that. You can ask me about passwords, phishing, malware, VPNs, privacy, ransomware, antivirus, social engineering, scams, firewalls, safe browsing, or two-factor authentication.";
        }

        private void DisplayUserMessage(string message)
        {
            ChatDisplay.AppendText($"YOU: {message}\n\n");
        }

        private void DisplayBotMessage(string message)
        {
            ChatDisplay.AppendText($"BOT: {message}\n\n");

            ChatDisplay.ScrollToEnd();
        }

        private void PlayGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greeting.wav");

                player.Play();
            }
            catch
            {
                DisplayBotMessage("Voice greeting unavailable.");
            }
        }
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn =
                       new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query =
                        "INSERT INTO Tasks(TaskName, ReminderDate) VALUES (@TaskName,@ReminderDate)";

                    SqlCommand cmd =
                        new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue(
                        "@TaskName",
                        TaskNameBox.Text);

                    cmd.Parameters.AddWithValue(
                        "@ReminderDate",
                        ReminderDatePicker.SelectedDate);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Task added successfully!");

                LogActivity($"Task Added: {TaskNameBox.Text}");

                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadTasks()
        {
            try
            {
                using (SqlConnection conn =
                       new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query =
                        "SELECT * FROM Tasks";

                    SqlDataAdapter adapter =
                        new SqlDataAdapter(query, conn);

                    DataTable dt =
                        new DataTable();

                    adapter.Fill(dt);

                    TaskGrid.ItemsSource =
                        dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TaskGrid.SelectedItem == null)
                {
                    MessageBox.Show("Please select a task first.");
                    return;
                }

                DataRowView row = (DataRowView)TaskGrid.SelectedItem;

                int taskID = Convert.ToInt32(row["TaskID"]);

                using (SqlConnection conn =
                       new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query =
                        "DELETE FROM Tasks WHERE TaskID = @TaskID";

                    SqlCommand cmd =
                        new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@TaskID", taskID);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Task deleted successfully!");

                LogActivity($"Task Deleted: {taskID}");

                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SubmitQuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            string userAnswer = AnswerBox.Text.Trim();

            string correctAnswer =
                quizQuestions[currentQuestion].CorrectAnswer;

            if (userAnswer.Equals(correctAnswer,
                StringComparison.OrdinalIgnoreCase))
            {
                score++;

                MessageBox.Show("Correct!");
            }
            else
            {
                MessageBox.Show(
                    $"Incorrect.\nCorrect answer: {correctAnswer}");
            }

            currentQuestion++;

            if (currentQuestion < quizQuestions.Count)
            {
                QuestionText.Text =
                    quizQuestions[currentQuestion].Question;

                AnswerBox.Clear();
            }
            else
            {
                QuestionText.Text = "Quiz Complete!";

                ScoreText.Text =
                    $"Final Score: {score}/{quizQuestions.Count}";
            }

            LogActivity("Quiz question answered.");
        }
    
    private void LogActivity(string action)
        {
            string entry =
                $"{DateTime.Now:HH:mm:ss} - {action}";

            activityLog.Add(entry);

            // Keep only the last 5 actions
            if (activityLog.Count > 5)
            {
                activityLog.RemoveAt(0);
            }

            ActivityLogDisplay.Text =
                string.Join(Environment.NewLine, activityLog);
        }
    }
}
