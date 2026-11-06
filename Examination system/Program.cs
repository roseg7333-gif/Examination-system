using System;
using System.Collections.Generic;

namespace ExaminationSystem
{
    enum Level
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    
    class Question
    {
        public string Header { get; set; }
        public int Marks { get; set; }
        public Level Level { get; set; }

        public virtual void Display()
        {
            Console.WriteLine(Header + "  (" + Marks + " marks)");
        }

        public virtual bool CheckAnswer(string answer)
        {
            return false;
        }
    }

    class TrueFalseQuestion : Question
    {
        public bool CorrectAnswer { get; set; }

        public override void Display()
        {
            base.Display();
            Console.WriteLine("Type T for True or F for False");
        }

        public override bool CheckAnswer(string answer)
        {
            answer = answer.Trim().ToLower();
            if (answer == "t" || answer == "true")
                return CorrectAnswer == true;
            if (answer == "f" || answer == "false")
                return CorrectAnswer == false;
            return false;
        }
    }

    class ChooseOneQuestion : Question
    {
        public string[] Choices = new string[4];
        public int CorrectChoice { get; set; }

        public override void Display()
        {
            base.Display();
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine((i + 1) + ") " + Choices[i]);
            }
            Console.WriteLine("Enter the number of the correct choice");
        }

        public override bool CheckAnswer(string answer)
        {
            int num;
            if (!int.TryParse(answer, out num))
                return false;
            return num == CorrectChoice;
        }
    }

    class MultipleChoiceQuestion : Question
    {
        public string[] Choices = new string[4];
        public List<int> CorrectAnswers = new List<int>();

        public override void Display()
        {
            base.Display();
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine((i + 1) + ") " + Choices[i]);
            }
            Console.WriteLine("Enter the correct numbers separated by comma (example: 1,3)");
        }

        public override bool CheckAnswer(string answer)
        {
            List<int> studentAnswers = new List<int>();
            string[] parts = answer.Split(',');

            foreach (string p in parts)
            {
                int num;
                if (!int.TryParse(p.Trim(), out num))
                    return false;
                if (!studentAnswers.Contains(num))
                    studentAnswers.Add(num);
            }

          
            if (studentAnswers.Count != CorrectAnswers.Count)
                return false;

            foreach (int a in studentAnswers)
            {
                if (!CorrectAnswers.Contains(a))
                    return false;
            }
            return true;
        }
    }

    class Program
    {
        
        static List<Question> questions = new List<Question>();


        static int ReadNumber(string message, int min, int max)
        {
            while (true)
            {
                Console.Write(message);
                int n;
                if (int.TryParse(Console.ReadLine(), out n) && n >= min && n <= max)
                    return n;
                Console.WriteLine("Invalid input, try again.");
            }
        }

        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("===== Main Menu =====");
                Console.WriteLine("1. Doctor Mode");
                Console.WriteLine("2. Student Mode");
                Console.WriteLine("3. Exit");

                int choice = ReadNumber("Choose: ", 1, 3);

                if (choice == 1)
                    DoctorMode();
                else if (choice == 2)
                    StudentMode();
                else
                    running = false;
            }

            Console.WriteLine("Bye!");
        }

        static void DoctorMode()
        {
            Console.WriteLine();
            Console.WriteLine("===== Doctor Mode =====");

            int count = ReadNumber("How many questions do you want to add? ", 1, 100);

            for (int i = 1; i <= count; i++)
            {
                Console.WriteLine();
                Console.WriteLine("--- Question " + i + " ---");
                Console.WriteLine("Type: 1. True/False  2. Choose One  3. Multiple Choice");
                int type = ReadNumber("Select type: ", 1, 3);

                Console.WriteLine("Level: 1. Easy  2. Medium  3. Hard");
                int levelNum = ReadNumber("Select level: ", 1, 3);
                Level level = (Level)levelNum;

                Console.Write("Question header: ");
                string header = Console.ReadLine();

                int marks = ReadNumber("Marks: ", 1, 1000);

                if (type == 1)
                {
                    TrueFalseQuestion q = new TrueFalseQuestion();
                    q.Header = header;
                    q.Marks = marks;
                    q.Level = level;

                    int ans = ReadNumber("Correct answer (1 = True, 2 = False): ", 1, 2);
                    q.CorrectAnswer = (ans == 1);

                    questions.Add(q);
                }
                else if (type == 2)
                {
                    ChooseOneQuestion q = new ChooseOneQuestion();
                    q.Header = header;
                    q.Marks = marks;
                    q.Level = level;

                    for (int c = 0; c < 4; c++)
                    {
                        Console.Write("Choice " + (c + 1) + ": ");
                        q.Choices[c] = Console.ReadLine();
                    }
                    q.CorrectChoice = ReadNumber("Correct choice number (1-4): ", 1, 4);

                    questions.Add(q);
                }
                else
                {
                    MultipleChoiceQuestion q = new MultipleChoiceQuestion();
                    q.Header = header;
                    q.Marks = marks;
                    q.Level = level;

                    for (int c = 0; c < 4; c++)
                    {
                        Console.Write("Choice " + (c + 1) + ": ");
                        q.Choices[c] = Console.ReadLine();
                    }

               
                    while (true)
                    {
                        Console.Write("Correct answers (comma separated, example 1,3): ");
                        string line = Console.ReadLine();
                        string[] parts = line.Split(',');
                        List<int> temp = new List<int>();
                        bool ok = true;

                        foreach (string p in parts)
                        {
                            int n;
                            if (int.TryParse(p.Trim(), out n) && n >= 1 && n <= 4)
                            {
                                if (!temp.Contains(n))
                                    temp.Add(n);
                            }
                            else
                            {
                                ok = false;
                            }
                        }

                        if (ok && temp.Count > 0)
                        {
                            q.CorrectAnswers = temp;
                            break;
                        }
                        Console.WriteLine("Invalid input, try again.");
                    }

                    questions.Add(q);
                }

                Console.WriteLine("Question added.");
            }

            Console.WriteLine();
            Console.WriteLine("Done. Total questions in the bank: " + questions.Count);
        }

        static void StudentMode()
        {
            Console.WriteLine();
            Console.WriteLine("===== Student Mode =====");

            if (questions.Count == 0)
            {
                Console.WriteLine("The question bank is empty. Ask the doctor to add questions first.");
                return;
            }

            Console.WriteLine("Exam type: 1. Practical (half of the questions)  2. Final (all questions)");
            int examType = ReadNumber("Choose exam type: ", 1, 2);

            Console.WriteLine("Level: 1. Easy  2. Medium  3. Hard");
            int levelNum = ReadNumber("Choose level: ", 1, 3);
            Level level = (Level)levelNum;

            
            List<Question> levelQuestions = new List<Question>();
            foreach (Question q in questions)
            {
                if (q.Level == level)
                    levelQuestions.Add(q);
            }

            if (levelQuestions.Count == 0)
            {
                Console.WriteLine("There are no " + level + " questions yet.");
                return;
            }

      
            int examCount = levelQuestions.Count;
            if (examType == 1)
            {
                examCount = levelQuestions.Count / 2;
                if (examCount == 0)
                    examCount = 1; 
            }

            int totalMarks = 0;
            int studentMarks = 0;

            Console.WriteLine();
            Console.WriteLine("Exam started, number of questions: " + examCount);

            for (int i = 0; i < examCount; i++)
            {
                Question q = levelQuestions[i];

                Console.WriteLine();
                Console.Write("Q" + (i + 1) + ": ");
                q.Display();          
                Console.Write("Your answer: ");
                string answer = Console.ReadLine();

                totalMarks += q.Marks;

                if (q.CheckAnswer(answer))                                                      // polymorphism
                {
                    studentMarks += q.Marks;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Your Result: " + studentMarks + " / " + totalMarks);
        }
    }
}
