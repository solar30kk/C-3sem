using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

class FileItem
{
    public string Name;
    public long Size;
    public DateTime Modified;
    public bool IsDirectory;

    public FileItem(string name, long size, DateTime modified, bool isDirectory = false)
    {
        Name = name;
        Size = size;
        Modified = modified;
        IsDirectory = isDirectory;
    }
}

class Program
{
    const string Blue = "0000A0";
    const string Cyan = "6EE3FF";
    const string Yellow = "FFFF00";
    const string Menu = "0DA599";
    const string Selected = "4FF7FC";
    const string SelectedText = "002383";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        EnableColors();
        Console.Write("\u001b[?7l");
        Console.Clear();
        Console.CursorVisible = false;
        SetColors(Cyan, Blue);
        for (int y = 0; y < 24; y++)
            WriteAt(0, y, new string(' ', 80));

        DrawBox(0, 1, 38, 20);
        DrawBox(40, 1, 38, 20);
        DrawSeparator(13);
        DrawSeparator(26);
        DrawSeparator(53);
        DrawSeparator(63);
        DrawSeparator(72);
        WriteAt(0, 20, "╟────────────┴────────────┴────────────╢");
        WriteAt(40, 20, "╟────────────┴─────────┴────────┴──────╢");
        WriteAt(16, 1, @" C:\NC ");
        SetColors(SelectedText, Selected);
        WriteAt(56, 1, @" C:\NC ");

        SetColors(Yellow, Blue);
        WriteAt(1, 2, "C:↓ Имя");
        WriteAt(18, 2, "Имя");
        WriteAt(31, 2, "Имя");
        WriteAt(41, 2, "C:↓ Имя");
        WriteAt(56, 2, "Размер");
        WriteAt(66, 2, "Дата");
        WriteAt(73, 2, "Время");

        List<FileItem> leftFiles = CreateLeftFiles();
        SetColors(Cyan, Blue);
        for (int i = 0; i < leftFiles.Count && i < 51; i++)
            WriteAt(1 + i / 17 * 13, 3 + i % 17, DisplayName(leftFiles[i].Name));

        List<FileItem> rightFiles = CreateRightFiles();
        for (int i = 0; i < rightFiles.Count && i < 17; i++)
        {
            FileItem file = rightFiles[i];
            if (i == 0) SetColors(SelectedText, Selected);
            else SetColors(Cyan, Blue);
            string size = file.IsDirectory ? "►КАТАЛОГ◄" : file.Size.ToString().PadLeft(9);
            string row = DisplayName(file.Name) + "│" + size + "│"
                + file.Modified.ToString("dd.MM.yy") + "│"
                + file.Modified.ToString("H:mm").PadLeft(6);
            WriteAt(41, 3 + i, row);
        }

        SetColors(Cyan, Blue);
        WriteAt(1, 21, "..           ►КАТАЛОГ◄ 11.10.02  19:48");
        WriteAt(41, 21, "..           ►КАТАЛОГ◄ 11.10.02  19:48");
        SetColors("000000", Menu);
        WriteAt(0, 0, new string(' ', 80));
        DrawMenuWord(4, "Левая");
        DrawMenuWord(13, "Файл");
        DrawMenuWord(21, "Диск");
        DrawMenuWord(29, "Команды");
        DrawMenuWord(40, "Правая");
        SetColors(SelectedText, Selected);
        WriteAt(75, 0, " 8 30");

        SetColors("FFFFFF", "000000");
        WriteAt(0, 23, @"C:\NC>".PadRight(80));
        WriteAt(0, 24, new string(' ', 79));
        string[] keys = { "Помощь", "Вызов", "Чтение", "Правка", "Копия",
                          "НовИмя", "НовКат", "Удал-е", "Меню", "Выход" };
        for (int i = 0; i < keys.Length; i++)
        {
            string number = (i + 1).ToString();
            SetColors("FFFFFF", "000000");
            WriteAt(i * 8, 24, number);
            SetColors("000000", Menu);
            WriteAt(i * 8 + number.Length, 24, keys[i].PadRight(6));
        }

        Console.Write("\u001b[?7h");
        SetColors("FFFFFF", "000000");
        Console.SetCursorPosition(6, 23);
        Console.Write("\u001b[3 q");
        Console.CursorVisible = true;
        Console.ReadKey(true);
        Console.Write("\u001b[0 q"); 
        Console.Write("\u001b[0m");
        Console.Clear();
        Console.CursorVisible = true;
    }

    static List<FileItem> CreateLeftFiles()
    {
        string[] names =
        {
            "..", "Ajaccgdo", "nc.cfg", "nc_exit.com", "telemax.dat", "nc_exit.doc",
            "123view.exe", "arcview.exe", "bitmap.exe", "clp2dib.exe", "dbview.exe",
            "draw2wmf.exe", "drw2wmf.exe", "ico2dib.exe", "msp2dib.exe", "nc.exe", "ncclean.exe",
            "ncdd.exe", "ncedit.exe", "ncff.exe", "nclabel.exe", "ncmain.exe", "ncnet.exe",
            "ncsf.exe", "ncsi.exe", "nczip.exe", "packer.exe", "paraview.exe", "pct2dib.exe",
            "playwave.exe", "q&aview.exe", "rbview.exe", "refview.exe", "saver.exe",
            "telemax.exe", "tif2dib.exe", "vector.exe", "wpb2dib.exe", "wpv2wmf.exe",
            "wpview.exe", "nc.ext", "nc.fil", "ncpscrip.hdr", "nc.hlp", "ncff.hlp",
            "telemax.hlp", "nc.ico", "nc.ini", "ncclean.ini", "norton.ini", "telemax.ini",
            "MyFileWithLongName.txt"
        };
        List<FileItem> files = new List<FileItem>();
        foreach (string name in names)
        {
            files.Add(new FileItem(name, 0, new DateTime(1995, 5, 25, 5, 0, 0), name == ".."));
        }
        files.Sort(CompareFiles);
        return files;
    }

    static int CompareFiles(FileItem first, FileItem second)
    {
        return string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase);
    }

    static List<FileItem> CreateRightFiles()
    {
        DateTime date = new DateTime(1995, 5, 25, 5, 0, 0);
        return new List<FileItem>
        {
            new FileItem("..", 0, new DateTime(2002, 10, 11, 19, 48, 0), true),
            new FileItem("123view.exe", 128380, date),
            new FileItem("4372ansi.set", 255, date),
            new FileItem("8502ansi.set", 255, date),
            new FileItem("8632ansi.set", 255, date),
            new FileItem("8652ansi.set", 255, date),
            new FileItem("8662ansi.set", 255, date),
            new FileItem("Ajaccgdo", 417392, new DateTime(2002, 10, 12, 9, 2, 0)),
            new FileItem("ansi2437.set", 255, date),
            new FileItem("ansi2850.set", 255, date),
            new FileItem("ansi2863.set", 255, date),
            new FileItem("ansi2865.set", 255, date),
            new FileItem("ansi2866.set", 255, date),
            new FileItem("arcview.exe", 81738, date),
            new FileItem("bitmap.exe", 54805, date),
            new FileItem("bug.nss", 16133, date),
            new FileItem("bungee.nss", 41914, date)
        };
    }

    static string DisplayName(string name)
    {
        if (name.Length > 12) return ShortenName(name, 12);
        string extension = Path.GetExtension(name);
        string stem = Path.GetFileNameWithoutExtension(name);
        if (extension.Length == 0 || name == ".." || extension.Length > 4 || stem.Length > 9)
            return name.PadRight(12);
        return stem.PadRight(9) + extension.TrimStart('.').PadRight(3);
    }

    static string ShortenName(string name, int width)
    {
        if (name.Length <= width) return name;
        string extension = Path.GetExtension(name);
        int length = width - extension.Length - 1;
        if (length > 0) return name.Substring(0, length) + "~" + extension;
        return name.Substring(0, width - 1) + "~";
    }

    static void SetColors(string foreground, string background)
    {
        int fg = Convert.ToInt32(foreground, 16);
        int bg = Convert.ToInt32(background, 16);
        Console.Write($"\u001b[38;2;{fg >> 16};{fg >> 8 & 255};{fg & 255}m");
        Console.Write($"\u001b[48;2;{bg >> 16};{bg >> 8 & 255};{bg & 255}m");
    }

    [DllImport("kernel32.dll")] static extern IntPtr GetStdHandle(int value);
    [DllImport("kernel32.dll")] static extern bool GetConsoleMode(IntPtr handle, out uint mode);
    [DllImport("kernel32.dll")] static extern bool SetConsoleMode(IntPtr handle, uint mode);
    static void EnableColors()
    {
        if (!OperatingSystem.IsWindows()) return;
        IntPtr output = GetStdHandle(-11);
        if (GetConsoleMode(output, out uint mode)) SetConsoleMode(output, mode | 4);
    }

    static void DrawMenuWord(int x, string word)
    {
        SetColors(Yellow, Menu);
        WriteAt(x, 0, word.Substring(0, 1));
        SetColors("000000", Menu);
        WriteAt(x + 1, 0, word.Substring(1));
    }

    static void WriteAt(int x, int y, string text)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(text);
    }

    static void DrawSeparator(int x)
    {
        WriteAt(x, 1, "╤");
        for (int y = 2; y < 20; y++) WriteAt(x, y, "│");
    }

    static void DrawBox(int x, int y, int width, int height)
    {
        Console.SetCursorPosition(x, y);
        Console.Write('\u2554');
        for (int i = 0; i < width; i++) Console.Write('\u2550');
        Console.Write('\u2557');
        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(x, y + 1 + i);
            Console.Write('\u2551');
            for (int j = 0; j < width; j++) Console.Write(' ');
            Console.Write('\u2551');
        }
        Console.SetCursorPosition(x, y + height + 1);
        Console.Write('\u255A');
        for (int i = 0; i < width; i++) Console.Write('\u2550');
        Console.Write('\u255D');
    }
}
