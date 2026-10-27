using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Practice2Variant2
{
    // Enum позволяет выбирать породу из заранее заданного набора.
    public enum CatBreed
    {
        Siamese = 1,
        MaineCoon = 2,
        Sphynx = 3,
        ScottishFold = 4
    }

    // Struct хранит значение: при присваивании копируется сам вес.
    public readonly struct Weight
    {
        private readonly double _kilograms;

        public double Kilograms
        {
            get { return _kilograms; }
        }

        public Weight(double kilograms)
        {
            if (!double.IsFinite(kilograms) || kilograms < 0)
                throw new ArgumentOutOfRangeException(nameof(kilograms),
                    "Вес должен быть конечным неотрицательным числом.");

            _kilograms = kilograms;
        }

        public static Weight operator +(Weight left, Weight right)
        {
            // Конструктор также проверит, не стало ли число бесконечным.
            return new Weight(left.Kilograms + right.Kilograms);
        }

        public override string ToString()
        {
            return $"{Kilograms:G} кг";
        }
    }

    public class Animal
    {
        // Поля закрыты. Проверки выполняются через открытые свойства.
        private Weight _weight;
        private CatBreed _breed;

        public Weight Weight
        {
            get { return _weight; }
            set
            {
                if (value.Kilograms <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Вес животного должен быть больше нуля.");
                _weight = value;
            }
        }

        public CatBreed Breed
        {
            get { return _breed; }
            set
            {
                if (!Enum.IsDefined(typeof(CatBreed), value))
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Такой породы нет в перечислении CatBreed.");
                _breed = value;
            }
        }

        public Animal() : this(new Weight(4), CatBreed.ScottishFold)
        {
        }

        public Animal(Weight weight, CatBreed breed)
        {
            Weight = weight;
            Breed = breed;
        }

        // Деконструктор извлекает значения. Это не деструктор ~Animal().
        public void Deconstruct(out Weight weight, out CatBreed breed)
        {
            weight = Weight;
            breed = Breed;
        }

        public override string ToString()
        {
            return $"Животное: порода {Breed}, вес {Weight}";
        }
    }

    public class Cat : Animal
    {
        public const int MaxEnergy = 100;
        public const double MaxAge = 40; // Учебная граница проверки ввода.

        private string _name = "";
        private double _age;

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Кличка не должна быть пустой.", nameof(value));
                _name = value.Trim();
            }
        }

        public double Age
        {
            get { return _age; }
            set
            {
                if (!double.IsFinite(value) || value < 0 || value > MaxAge)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Возраст должен быть от 0 до {MaxAge} лет.");
                _age = value;
            }
        }

        // protected set позволяет котёнку менять энергию в своём Play().
        public int Energy { get; protected set; }

        public bool IsHungry
        {
            get { return Energy < 50; }
        }

        // У Cat четыре собственных свойства и два унаследованных.
        public Cat() : this("Мурка", new Weight(4), CatBreed.ScottishFold, 3)
        {
        }

        public Cat(string name, Weight weight, CatBreed breed, double age)
            : base(weight, breed)
        {
            Name = name;
            Age = age;
            Energy = MaxEnergy;
        }

        // Вместо невозможной в C# перегрузки = создаём отдельный объект.
        public Cat(Cat other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Name = other.Name;
            Weight = other.Weight;
            Breed = other.Breed;
            Age = other.Age;
            Energy = other.Energy;
        }

        public virtual string Play()
        {
            if (Energy < 20)
                return $"{Name} устала. Сначала покормите кошку.";

            Energy -= 20;
            return $"{Name} играет с мячиком. Энергия: {Energy}/{MaxEnergy}.";
        }

        public string Feed(double foodGrams)
        {
            if (!double.IsFinite(foodGrams) || foodGrams <= 0)
                throw new ArgumentOutOfRangeException(nameof(foodGrams),
                    "Количество корма должно быть конечным числом больше нуля.");

            // Учебная модель: масса корма прибавляется к весу, энергия растёт на 20.
            Weight = Weight + new Weight(foodGrams / 1000);
            Energy = Math.Min(MaxEnergy, Energy + 20);
            return $"{Name} съела {foodGrams:G} г корма. Вес: {Weight}. Энергия: {Energy}.";
        }

        public static string JudgingLook()
        {
            return "Кошка смотрит осуждающе: «Ты опять сидишь за компьютером?»";
        }

        public virtual Cat Copy()
        {
            return new Cat(this);
        }

        // Cat + Weight означает создание копии кошки с увеличенным весом.
        public static Cat operator +(Cat cat, Weight addedWeight)
        {
            if (cat == null)
                throw new ArgumentNullException(nameof(cat));

            Cat result = cat.Copy();
            result.Weight = cat.Weight + addedWeight;
            return result;
        }

        public override string ToString()
        {
            return $"Кошка {Name}: порода {Breed}, вес {Weight}, возраст {Age:G} лет, "
                + $"энергия {Energy}/{MaxEnergy}, голодна: {(IsHungry ? "да" : "нет")}";
        }
    }

    public class Kitten : Cat
    {
        public Kitten() : this("Пушок", new Weight(1.2), CatBreed.MaineCoon, 0.5)
        {
        }

        public Kitten(string name, Weight weight, CatBreed breed, double age)
            : base(name, weight, breed, age)
        {
        }

        public Kitten(Kitten other) : base(other)
        {
        }

        public override string Play()
        {
            if (Energy < 10)
                return $"Котёнок {Name} устал. Сначала покормите котёнка.";

            Energy -= 10;
            return $"Котёнок {Name} ловит бантик и прыгает! Энергия: {Energy}/{MaxEnergy}.";
        }

        // При копировании через переменную Cat сохраняется тип Kitten.
        public override Cat Copy()
        {
            return new Kitten(this);
        }

        public override string ToString()
        {
            return $"Котёнок {Name}: порода {Breed}, вес {Weight}, возраст {Age:G} лет, "
                + $"энергия {Energy}/{MaxEnergy}, голоден: {(IsHungry ? "да" : "нет")}";
        }
    }

    // Интерфейс задаёт, какие действия должен поддерживать домик.
    public interface IPetHouse
    {
        string Name { get; }
        int Count { get; }
        void Add(Animal animal);
        bool RemoveAt(int index);
        void ShowAnimals();
    }

    public class PetHouse : IPetHouse
    {
        // readonly запрещает заменить сам список, но не добавлять элементы.
        private readonly List<Animal> _animals = new List<Animal>();

        public string Name { get; }

        public int Count
        {
            get { return _animals.Count; }
        }

        public PetHouse() : this("Кошкин дом")
        {
        }

        public PetHouse(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название не должно быть пустым.", nameof(name));
            Name = name.Trim();
        }

        public void Add(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            _animals.Add(animal);
        }

        // Метод принимает индекс с нуля. Номер в меню начинается с единицы.
        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= _animals.Count)
                return false;

            _animals.RemoveAt(index);
            return true;
        }

        public void ShowAnimals()
        {
            Console.WriteLine($"{Name}. Животных: {Count}.");
            if (Count == 0)
                Console.WriteLine("В домике пока никого нет.");

            for (int i = 0; i < _animals.Count; i++)
            {
                // Для Cat и Kitten вызовется их собственный ToString().
                Console.WriteLine($"{i + 1}. {_animals[i]}");
            }
        }
    }

    // Все повторяющиеся проверки консольного ввода собраны в одном месте.
    internal static class Input
    {
        private static string ReadLine(string prompt)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine();
            if (text == null)
                throw new EndOfStreamException();
            return text.Trim();
        }

        public static string ReadName(string prompt)
        {
            while (true)
            {
                string text = ReadLine(prompt);
                if (!string.IsNullOrWhiteSpace(text))
                    return text;
                Console.WriteLine("Введите непустое название или кличку.");
            }
        }

        public static int ReadInt(string prompt, int minimum, int maximum)
        {
            while (true)
            {
                string text = ReadLine(prompt);
                if (int.TryParse(text, out int number)
                    && number >= minimum && number <= maximum)
                    return number;
                Console.WriteLine($"Введите целое число от {minimum} до {maximum}.");
            }
        }

        private static double ReadFiniteNumber(string prompt)
        {
            while (true)
            {
                // Разрешены и 1,5, и 1.5. Разделители тысяч не используются.
                string text = ReadLine(prompt).Replace(',', '.');
                if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out double number) && double.IsFinite(number))
                    return number;
                Console.WriteLine("Введите конечное число, например 1,5 или 1.5.");
            }
        }

        public static double ReadPositiveNumber(string prompt)
        {
            while (true)
            {
                double number = ReadFiniteNumber(prompt);
                if (number > 0)
                    return number;
                Console.WriteLine("Число должно быть больше нуля.");
            }
        }

        public static double ReadAge()
        {
            while (true)
            {
                double age = ReadFiniteNumber("Возраст в годах (например, 0,5): ");
                if (age >= 0 && age <= Cat.MaxAge)
                    return age;
                Console.WriteLine($"Возраст должен быть от 0 до {Cat.MaxAge} лет.");
            }
        }

        public static CatBreed ReadBreed()
        {
            Console.WriteLine("1 - Siamese (сиамская)");
            Console.WriteLine("2 - MaineCoon (мейн-кун)");
            Console.WriteLine("3 - Sphynx (сфинкс)");
            Console.WriteLine("4 - ScottishFold (шотландская вислоухая)");
            return (CatBreed)ReadInt("Порода: ", 1, 4);
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");

            Console.WriteLine("Практическая работа № 2. Вариант 2: кошки.");
            Console.WriteLine("Создана кошка по умолчанию. Её можно заменить в пункте 1.");

            try
            {
                RunMenu();
            }
            catch (EndOfStreamException)
            {
                // Завершение ввода не должно приводить к бесконечному циклу.
                Console.WriteLine("\nВвод завершён. Программа закрыта.");
            }
        }

        private static void RunMenu()
        {
            Cat currentCat = new Cat();
            IPetHouse house = new PetHouse();

            while (true)
            {
                Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                Console.WriteLine($"Текущий объект: {currentCat.Name} ({currentCat.GetType().Name})");
                Console.WriteLine("1 - Создать кошку или котёнка, задать параметры");
                Console.WriteLine("2 - Вывести все свойства и ToString()");
                Console.WriteLine("3 - Выполнить методы: играть, кормить, осуждающий взгляд");
                Console.WriteLine("4 - Домик: коллекция животных");
                Console.WriteLine("5 - Деконструкция: получить вес и породу");
                Console.WriteLine("6 - Присваивание ссылки и независимая копия");
                Console.WriteLine("7 - Проверить арифметический оператор +");
                Console.WriteLine("8 - Проверить Kitten и полиморфизм");
                Console.WriteLine("0 - Выход");

                int choice = Input.ReadInt("Ваш выбор: ", 0, 8);
                try
                {
                    switch (choice)
                    {
                        case 1:
                            currentCat = CreateCat();
                            Console.WriteLine($"Создан объект: {currentCat}");
                            break;
                        case 2:
                            ShowProperties(currentCat);
                            break;
                        case 3:
                            MethodsMenu(currentCat);
                            break;
                        case 4:
                            HouseMenu(house, currentCat);
                            break;
                        case 5:
                            ShowDeconstruction(currentCat);
                            break;
                        case 6:
                            ShowCopy(currentCat);
                            break;
                        case 7:
                            ShowAddition(currentCat);
                            break;
                        case 8:
                            ShowKitten();
                            break;
                        case 0:
                            Console.WriteLine("До свидания!");
                            return;
                    }
                }
                catch (ArgumentException error)
                {
                    Console.WriteLine($"Операция не выполнена: {error.Message}");
                }
            }
        }

        private static Cat CreateCat()
        {
            Console.WriteLine("1 - Cat с параметрами, 2 - Kitten с параметрами");
            Console.WriteLine("3 - Cat по умолчанию, 4 - Kitten по умолчанию");
            int type = Input.ReadInt("Тип объекта: ", 1, 4);
            if (type == 3)
                return new Cat();
            if (type == 4)
                return new Kitten();

            string name = Input.ReadName("Кличка: ");
            Weight weight = new Weight(Input.ReadPositiveNumber("Вес в килограммах: "));
            CatBreed breed = Input.ReadBreed();
            double age = Input.ReadAge();

            if (type == 2)
                return new Kitten(name, weight, breed, age);
            return new Cat(name, weight, breed, age);
        }

        private static void ShowProperties(Cat cat)
        {
            Console.WriteLine($"Тип объекта: {cat.GetType().Name}");
            Console.WriteLine($"Name (кличка): {cat.Name}");
            Console.WriteLine($"Weight (вес): {cat.Weight}");
            Console.WriteLine($"Breed (порода): {cat.Breed}");
            Console.WriteLine($"Age (возраст в годах): {cat.Age:G}");
            Console.WriteLine($"Energy (энергия): {cat.Energy}/{Cat.MaxEnergy}");
            Console.WriteLine($"IsHungry (голодна): {(cat.IsHungry ? "да" : "нет")}");
            Console.WriteLine($"ToString(): {cat}");
        }

        private static void MethodsMenu(Cat cat)
        {
            while (true)
            {
                Console.WriteLine("\n--- МЕТОДЫ ---");
                Console.WriteLine("1 - Play(): поиграть");
                Console.WriteLine("2 - Feed(): покормить");
                Console.WriteLine("3 - Cat.JudgingLook(): осуждающий взгляд");
                Console.WriteLine("0 - Назад");
                int choice = Input.ReadInt("Ваш выбор: ", 0, 3);
                switch (choice)
                {
                    case 1:
                        Console.WriteLine(cat.Play());
                        break;
                    case 2:
                        double grams = Input.ReadPositiveNumber("Количество корма в граммах: ");
                        Console.WriteLine(cat.Feed(grams));
                        break;
                    case 3:
                        // Статический метод вызывается через имя класса.
                        Console.WriteLine(Cat.JudgingLook());
                        break;
                    case 0:
                        return;
                }
            }
        }

        private static void HouseMenu(IPetHouse house, Cat currentCat)
        {
            while (true)
            {
                Console.WriteLine("\n--- ДОМИК ---");
                Console.WriteLine("1 - Добавить независимую копию текущего объекта");
                Console.WriteLine("2 - Показать животных");
                Console.WriteLine("3 - Удалить животное по номеру");
                Console.WriteLine("4 - Добавить котёнка по умолчанию");
                Console.WriteLine("0 - Назад");
                int choice = Input.ReadInt("Ваш выбор: ", 0, 4);
                switch (choice)
                {
                    case 1:
                        house.Add(currentCat.Copy());
                        Console.WriteLine("Копия добавлена в домик.");
                        break;
                    case 2:
                        house.ShowAnimals();
                        break;
                    case 3:
                        if (house.Count == 0)
                        {
                            Console.WriteLine("Домик пуст. Удалять некого.");
                            break;
                        }
                        house.ShowAnimals();
                        int number = Input.ReadInt("Номер для удаления: ", 1, house.Count);
                        house.RemoveAt(number - 1);
                        Console.WriteLine("Животное удалено из домика.");
                        break;
                    case 4:
                        house.Add(new Kitten());
                        Console.WriteLine("Котёнок добавлен в домик.");
                        break;
                    case 0:
                        return;
                }
            }
        }

        private static void ShowDeconstruction(Cat cat)
        {
            // Компилятор вызывает унаследованный Animal.Deconstruct().
            var (weight, breed) = cat;
            Console.WriteLine("var (weight, breed) = cat;");
            Console.WriteLine($"Из объекта получены: вес {weight}, порода {breed}.");
        }

        private static void ShowCopy(Cat cat)
        {
            Cat sameObject = cat; // Две ссылки на один объект.
            Cat copy = cat.Copy(); // Новый объект с теми же значениями.

            Console.WriteLine("Оператор = в C# перегрузить нельзя.");
            Console.WriteLine($"Cat sameObject = cat; общий объект: {ReferenceEquals(cat, sameObject)}");
            Console.WriteLine($"Cat copy = cat.Copy(); общий объект: {ReferenceEquals(cat, copy)}");
            Console.WriteLine($"Скопирован тип: {copy.GetType().Name}. Энергия копии: {copy.Energy}.");

            copy.Name = cat.Name + " (копия)";
            Console.WriteLine($"После изменения имени копии: оригинал = {cat.Name}, копия = {copy.Name}.");
            Console.WriteLine("Copy() использует копирующий конструктор. Оригинал не изменился.");
        }

        private static void ShowAddition(Cat cat)
        {
            Weight extra = new Weight(Input.ReadPositiveNumber("Сколько килограммов прибавить: "));
            Weight total = cat.Weight + extra;
            Cat heavierCat = cat + extra;

            Console.WriteLine($"Weight + Weight: {cat.Weight} + {extra} = {total}");
            Console.WriteLine($"Cat + Weight: {heavierCat}");
            Console.WriteLine($"Тип результата: {heavierCat.GetType().Name}.");
            Console.WriteLine($"Оригинал не изменился: {cat}");
        }

        private static void ShowKitten()
        {
            Kitten kitten = new Kitten();
            Cat catReference = kitten;
            Animal animalReference = kitten;

            Console.WriteLine("Создан Kitten. На него ссылаются переменные Cat и Animal.");
            Console.WriteLine(catReference.Play()); // Вызов Kitten.Play(), а не Cat.Play().
            Console.WriteLine($"Через Animal.ToString(): {animalReference}");

            Cat copy = catReference.Copy();
            Cat heavier = catReference + new Weight(0.1);
            Console.WriteLine($"Copy() сохранил тип: {copy.GetType().Name}.");
            Console.WriteLine($"После + тип: {heavier.GetType().Name}, вес: {heavier.Weight}.");
            Console.WriteLine($"Исходный котёнок: {kitten}");
        }
    }
}