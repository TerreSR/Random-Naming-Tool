using System.Text.Json;
using Spectre.Console;

class RandomName
{
    public class NameData
    {
        public List<string>? male_names {get; set;} = new();
        public List<string>? female_names {get; set;}= new();
        public List<string>? unisex_names {get; set;}= new();
        public List<string>? surnames {get; set;}
    }
    static string jsonEnglish = File.ReadAllText("common_english_names.json");
    static string jsonSpanish = File.ReadAllText("common_spanish_names.json");
    static string jsonChinese = File.ReadAllText("common_chinese_names.json");
    static string jsonGerman = File.ReadAllText("common_german_names.json");
    static string jsonJapanise = File.ReadAllText("common_japanese_names.json");
    static string jsonKorian = File.ReadAllText("common_korean_names.json");
    static string jsonRussian = File.ReadAllText("common_russian_names.json");
    static NameData? nameData = null;
    static Queue<string> nameQueue = new Queue<string>();
    static Queue<string> readingQueue = new Queue<string>();
    static Queue<string> setType = new Queue<string>();
    public static void Main(string[] args)
    {
        var figlet = new FigletText("Welcome To Randome Naming Tool ]:)");
        AnsiConsole.Write(figlet);
        AnsiConsole.MarkupLine("[red] *This App Use AI-Gen Data [/]");
        Thread.Sleep(1500);
        Console.WriteLine("");
        string? choice = null;
        while(true)
        {
            while(true)
            {
                string? inQueue = string.Join(" + ", setType);
                if(inQueue == null) inQueue = "None";
                AnsiConsole.Clear();
            Console.MarkupLine($"[blue] The current element is :\n  {inQueue}[/]");
            choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
            .Title("[green]please choice nationality type:[/]")
            .AddChoices(new[]
            {
                "English",
                "Spanish",
                "Chinese",
                "German",
                "Japanise",
                "Korean",
                "Russian",
                "[Yellow bold]Generate[/]",
                "[red]Deselect[/]"
            }));
            if(choice == "[red]Deselect[/]") {
                Queue<string> tmpQ = new Queue<string>();
                while(nameQueue.Count > 1) tmpQ.Enqueue(nameQueue.Dequeue());
                nameQueue.Clear();
                while(tmpQ.Count > 0) nameQueue.Enqueue(tmpQ.Dequeue());
                
                while(readingQueue.Count > 1) tmpQ.Enqueue(readingQueue.Dequeue());
                readingQueue.Clear();
                while(tmpQ.Count > 0) readingQueue.Enqueue(tmpQ.Dequeue());

                while(setType.Count > 1) tmpQ.Enqueue(setType.Dequeue());
                setType.Clear();
                while(tmpQ.Count > 0) setType.Enqueue(tmpQ.Dequeue());
            }
            if(choice == "[Yellow bold]Generate[/]") break;
            string choiceSex = null;
            if(choice != "[red]Deselect[/]")
            {
                choiceSex = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title($"[green] please choice sex:[/] \n Current Celection:{choice}")
                .AddChoices(new[]
                {
                    "Male",
                    "Female",
                    "Surname",
                    "Back"
                })
                );
                }
            ChoiceNames(choice, choiceSex);
            }
            if (nameQueue.Count > 0 && readingQueue.Count > 0)
            {
                string afterDequeue = nameQueue.Dequeue();
                while(nameQueue.Count > 0) afterDequeue += " " + nameQueue.Dequeue();
                string readDequeue = readingQueue.Dequeue();
                while(readingQueue.Count > 0) readDequeue += "・" + readingQueue.Dequeue();
                Console.Write(afterDequeue);
                Console.WriteLine("(" + readDequeue + ")");
                string[] line = {$"{afterDequeue}({readDequeue})"};
                File.AppendAllLines("result.txt", line);
            }
            if(!AnsiConsole.Confirm("[yellow] Continue?[/]\n [red] *The current name will be lost, but you can check it in Logs.txt.[/]")){ break; }
        }
        
    }
    
    static void ChoiceNames(string national, string sex)
    {
        List<string> Name = new List<string>();
        Random rand = new Random();

        switch (national)
        {
            case "English":
                nameData = JsonSerializer.Deserialize<NameData>(jsonEnglish);
                break;
            case "Spanish":
                nameData = JsonSerializer.Deserialize<NameData>(jsonSpanish);
                break;
            case "Chinese":
                nameData = JsonSerializer.Deserialize<NameData>(jsonChinese);
                break;
            case "German":
                nameData = JsonSerializer.Deserialize<NameData>(jsonGerman);
                break;
            case "Japanise":
                nameData = JsonSerializer.Deserialize<NameData>(jsonJapanise);
                break;
            case "Russian":
                nameData = JsonSerializer.Deserialize<NameData>(jsonRussian);
                break;
            case "Korean":
                nameData = JsonSerializer.Deserialize<NameData>(jsonKorian);
                break;
        }
        if(sex == "Surname")
        {
            Name.AddRange(nameData.surnames);
        }
        else if(sex == "Male")
            {
                Name.AddRange(nameData.male_names);
                Name.AddRange(nameData.unisex_names);
            }
        else if (sex == "Female")
            {
                Name.AddRange(nameData.female_names);
                Name.AddRange(nameData.unisex_names);
            }
        else return;

        setType.Enqueue("{" + national + sex + "} ");

        string GetRandomItem(List<string> list)
        {
            if(list == null || list.Count == 0) return string.Empty;
            int index = rand.Next(list.Count);
            return list[index];
        }
        string choice = GetRandomItem(Name);
        (string nameSpell, string nameReading) = StringSplitOptions(choice);
        nameQueue.Enqueue(nameSpell);
        readingQueue.Enqueue(nameReading);
    }

    private static (string Spell, string Reading) StringSplitOptions(string choice)
    {
        if(string.IsNullOrEmpty(choice)) return ("", "");

        int openBraceIndex = choice.IndexOf(':');
        if(openBraceIndex == -1) return (choice, choice);
        string name = choice.Substring(0, openBraceIndex);
        string reading = choice.Substring(openBraceIndex + 1);

        return (name, reading);
    }
}