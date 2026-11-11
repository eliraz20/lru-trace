using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
if (args.Length != 1 || args[0] == "--help")
{
    Console.WriteLine("Usage: dotnet run -- <input-file|->");
    return args.Length == 1 ? 0 : 2;
}

try
{
    string[] lines = args[0] == "-" ? Console.In.ReadToEnd().ReplaceLineEndings("\n").TrimEnd('\n').Split('\n') : File.ReadAllLines(args[0]);
    Check(lines.Length >= 2 && lines.Any(line => !string.IsNullOrWhiteSpace(line)), "Input is empty or missing records.");
    var capacity = Whole(lines[0], 1);
    var recent = new LinkedList<string>();
    var index = new Dictionary<string, LinkedListNode<string>>();
    long hits = 0, misses = 0;
    foreach (var page in lines.Skip(1).SelectMany(line => line.Split((char[]? )null, StringSplitOptions.RemoveEmptyEntries)))
    {
        string action;
        if (index.TryGetValue(page, out var existing))
        {
            recent.Remove(existing);
            hits++;
            action = "hit";
        }
        else
        {
            misses++;
            action = "miss";
            if (recent.Count == capacity)
            {
                var victim = recent.Last!.Value;
                index.Remove(victim);
                recent.RemoveLast();
                action += $", evict {victim}";
            }
        }

        index[page] = recent.AddFirst(page);
        Console.WriteLine($"{page}: {action}  [{string.Join(", ", recent)}]");
    }

    Console.WriteLine($"{hits} hits, {misses} misses");
    return 0;
}
catch (Exception e)when (e is IOException or FormatException or OverflowException or ArgumentException or UnauthorizedAccessException or DivideByZeroException)
{
    Console.Error.WriteLine(e.Message);
    return 1;
}

static void Check(bool valid, string message)
{
    if (!valid)
        throw new FormatException(message);
}

static long Whole(string value, long minimum)
{
    var number = long.Parse(value, CultureInfo.InvariantCulture);
    Check(number >= minimum, $"Expected an integer of at least {minimum}.");
    return number;
}
