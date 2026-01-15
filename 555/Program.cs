#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZooManagementApp
{
    abstract class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }

        public Animal(int id, string name, double weight)
        {
            Id = id;
            Name = name;
            Weight = weight;
        }

        public abstract double Calc();
        public abstract string Voice();

        public virtual string ToStr()
        {
            return $"{Id},{GetType().Name},{Name},{Weight}";
        }

        public virtual string Info()
        {
            return $"ID: {Id} | {Name}";
        }
    }

    class Lion : Animal
    {
        public Lion(int id, string name, double weight) : base(id, name, weight) { }

        public override double Calc() => Weight * 0.05;
        public override string Voice() => "Рррр-арр!";

        public override string ToStr() => base.ToStr() + ",";
        public override string Info() => base.Info() + " (Лев)";
    }

    class Elephant : Animal
    {
        public double Prop { get; set; }

        public Elephant(int id, string name, double weight, double prop) : base(id, name, weight)
        {
            Prop = prop;
        }

        public override double Calc() => Weight * 0.10;
        public override string Voice() => "Ту-ру-ру!";

        public override string ToStr() => base.ToStr() + $",{Prop}";
        public override string Info() => base.Info() + $" (Слон) Хобот: {Prop}м";
    }

    class Parrot : Animal
    {
        public string Prop { get; set; }

        public Parrot(int id, string name, double weight, string prop) : base(id, name, weight)
        {
            Prop = prop;
        }

        public override double Calc() => 0.1;
        public override string Voice() => "Привіт!";

        public override string ToStr() => base.ToStr() + $",{Prop}";
        public override string Info() => base.Info() + $" (Папуга) Колір: {Prop}";
    }

    class Giraffe : Animal
    {
        public double Prop { get; set; }

        public Giraffe(int id, string name, double weight, double prop) : base(id, name, weight)
        {
            Prop = prop;
        }

        public override double Calc() => Weight * 0.08;
        public override string Voice() => "Хрум-хрум";

        public override string ToStr() => base.ToStr() + $",{Prop}";
        public override string Info() => base.Info() + $" (Жираф) Шия: {Prop}м";
    }

    class Penguin : Animal
    {
        public string Prop { get; set; }

        public Penguin(int id, string name, double weight, string prop) : base(id, name, weight)
        {
            Prop = prop;
        }

        public override double Calc() => Weight * 0.15;
        public override string Voice() => "Махаємо!";

        public override string ToStr() => base.ToStr() + $",{Prop}";
        public override string Info() => base.Info() + $" (Пінгвін) Звання: {Prop}";
    }

    static class Data
    {
        private static string f1 = "animals.csv";
        private static string f2 = "users.csv";

        public static void Init()
        {
            if (!File.Exists(f1))
            {
                File.WriteAllText(f1, "Id,Type,Name,Weight,Extra\n", Encoding.UTF8);
            }

            if (!File.Exists(f2))
            {
                File.WriteAllText(f2, "Id,Email,Password\n", Encoding.UTF8);
                File.AppendAllText(f2, "1,admin,admin\n", Encoding.UTF8);
            }
        }

        public static int GetId(string path)
        {
            if (!File.Exists(path)) return 1;

            string[] rows = File.ReadAllLines(path);
            if (rows.Length < 2) return 1;

            int max = 0;
            for (int i = 1; i < rows.Length; i++)
            {
                string r = rows[i];
                if (string.IsNullOrWhiteSpace(r)) continue;

                string[] s = r.Split(',');
                if (int.TryParse(s[0], out int curr))
                {
                    if (curr > max) max = curr;
                }
            }
            return max + 1;
        }

        public static List<Animal> Read()
        {
            List<Animal> list = new List<Animal>();
            if (!File.Exists(f1)) return list;

            string[] rows = File.ReadAllLines(f1, Encoding.UTF8);

            for (int i = 1; i < rows.Length; i++)
            {
                string r = rows[i];
                if (string.IsNullOrWhiteSpace(r)) continue;

                string[] s = r.Split(',');
                if (s.Length < 4) continue;

                try
                {
                    int id = int.Parse(s[0]);
                    string type = s[1];
                    string name = s[2];
                    double w = double.Parse(s[3]);
                    string ext = s.Length > 4 ? s[4] : "";

                    Animal obj = null;

                    switch (type)
                    {
                        case "Lion":
                            obj = new Lion(id, name, w);
                            break;
                        case "Elephant":
                            double t = 0;
                            double.TryParse(ext, out t);
                            obj = new Elephant(id, name, w, t);
                            break;
                        case "Parrot":
                            obj = new Parrot(id, name, w, ext);
                            break;
                        case "Giraffe":
                            double n = 0;
                            double.TryParse(ext, out n);
                            obj = new Giraffe(id, name, w, n);
                            break;
                        case "Penguin":
                            obj = new Penguin(id, name, w, ext);
                            break;
                    }

                    if (obj != null) list.Add(obj);
                }
                catch { continue; }
            }
            return list;
        }

        public static void Add(Animal obj)
        {
            obj.Id = GetId(f1);
            File.AppendAllText(f1, obj.ToStr() + "\n", Encoding.UTF8);
        }

        public static void Del(int id)
        {
            List<Animal> list = Read();
            Animal found = null;

            foreach (var item in list)
            {
                if (item.Id == id)
                {
                    found = item;
                    break;
                }
            }

            if (found != null)
            {
                list.Remove(found);

                List<string> text = new List<string>();
                text.Add("Id,Type,Name,Weight,Extra");

                foreach (var item in list)
                {
                    text.Add(item.ToStr());
                }

                File.WriteAllLines(f1, text, Encoding.UTF8);
                Console.WriteLine("Видалено.");
            }
            else
            {
                Console.WriteLine("Не знайдено.");
            }
        }

        public static bool CheckUser(string l, string p)
        {
            if (!File.Exists(f2)) return false;
            string[] rows = File.ReadAllLines(f2);

            for (int i = 1; i < rows.Length; i++)
            {
                string[] s = rows[i].Split(',');
                if (s.Length >= 3)
                {
                    if (s[1] == l && s[2] == p) return true;
                }
            }
            return false;
        }

        public static bool AddUser(string l, string p)
        {
            if (!File.Exists(f2)) return false;
            string[] rows = File.ReadAllLines(f2);

            for (int i = 1; i < rows.Length; i++)
            {
                string[] s = rows[i].Split(',');
                if (s.Length > 1 && s[1] == l) return false;
            }

            int id = GetId(f2);
            string line = $"{id},{l},{p}\n";
            File.AppendAllText(f2, line, Encoding.UTF8);
            return true;
        }

        public static void StartData()
        {
            if (GetId("animals.csv") == 1)
            {
                Add(new Lion(0, "Алекс", 220));
                Add(new Elephant(0, "Дамбо", 200, 0.5));
                Add(new Parrot(0, "Яго", 0.6, "Червоний"));
                Add(new Giraffe(0, "Мелман", 1200, 2.8));
                Add(new Penguin(0, "Шкіпер", 15, "Капітан"));
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Lab 5";

            Data.Init();
            Data.StartData();

            if (!LoginMenu()) return;

            bool run = true;
            while (run)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== MENU ===");
                Console.ResetColor();
                Console.WriteLine("1. Таблиця");
                Console.WriteLine("2. Додати");
                Console.WriteLine("3. Видалити");
                Console.WriteLine("4. Статистика");
                Console.WriteLine("5. Звуки");
                Console.WriteLine("0. Вихід");
                Console.Write("> ");

                string k = Console.ReadLine();

                switch (k)
                {
                    case "1": Table(); break;
                    case "2": Add(); break;
                    case "3": Del(); break;
                    case "4": Stat(); break;
                    case "5": Sound(); break;
                    case "0": run = false; break;
                    default: Console.WriteLine("Error"); break;
                }

                if (run)
                {
                    Console.WriteLine("\nEnter...");
                    Console.ReadLine();
                }
            }
        }

        static bool LoginMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Вхід");
                Console.WriteLine("2. Реєстрація");
                Console.WriteLine("0. Вихід");
                Console.Write("> ");

                string k = Console.ReadLine();
                if (k == "0") return false;

                Console.Write("Email: ");
                string l = Console.ReadLine();
                Console.Write("Pass: ");
                string p = Console.ReadLine();

                if (k == "1")
                {
                    if (Data.CheckUser(l, p))
                    {
                        Console.WriteLine("OK");
                        return true;
                    }
                    else Console.WriteLine("No");
                }
                else if (k == "2")
                {
                    if (Data.AddUser(l, p)) Console.WriteLine("OK");
                    else Console.WriteLine("Error");
                }
                Console.ReadLine();
            }
        }

        static void Table()
        {
            List<Animal> list = Data.Read();
            Console.WriteLine("--- LIST ---");
            foreach (var item in list)
            {
                Console.WriteLine("| {0,-3} | {1,-35} | {2,8} | {3,10:F2} |",
                    item.Id, item.Info(), item.Weight, item.Calc());
            }
        }

        static void Add()
        {
            Console.WriteLine("1-Lion, 2-Elephant, 3-Parrot, 4-Giraffe, 5-Penguin");
            string t = Console.ReadLine();

            Console.Write("Name: ");
            string n = Console.ReadLine();

            Console.Write("Weight: ");
            double w = 0;
            double.TryParse(Console.ReadLine(), out w);

            Animal obj = null;

            if (t == "1") obj = new Lion(0, n, w);
            else if (t == "2")
            {
                Console.Write("Prop: ");
                double p = double.Parse(Console.ReadLine());
                obj = new Elephant(0, n, w, p);
            }
            else if (t == "3")
            {
                Console.Write("Prop: ");
                obj = new Parrot(0, n, w, Console.ReadLine());
            }
            else if (t == "4")
            {
                Console.Write("Prop: ");
                double p = double.Parse(Console.ReadLine());
                obj = new Giraffe(0, n, w, p);
            }
            else if (t == "5")
            {
                Console.Write("Prop: ");
                obj = new Penguin(0, n, w, Console.ReadLine());
            }

            if (obj != null)
            {
                Data.Add(obj);
                Console.WriteLine("OK");
            }
        }

        static void Del()
        {
            Table();
            Console.Write("ID: ");
            int id = 0;
            if (int.TryParse(Console.ReadLine(), out id))
            {
                Data.Del(id);
            }
        }

        static void Stat()
        {
            List<Animal> list = Data.Read();
            if (list.Count == 0) return;

            double s1 = 0;
            double s2 = 0;

            foreach (var item in list)
            {
                s1 += item.Calc();
                s2 += item.Weight;
            }

            double avg = s1 / list.Count;

            Console.WriteLine($"Count: {list.Count}");
            Console.WriteLine($"Total W: {s2}");
            Console.WriteLine($"Total F: {s1:F2}");
            Console.WriteLine($"Avg F: {avg:F2}");
        }

        static void Sound()
        {
            List<Animal> list = Data.Read();
            foreach (var item in list)
            {
                Console.WriteLine($"{item.Name}: {item.Voice()}");
            }
        }
    }
}