static class CompareReport
{
    public static void Run(string[] args)
    {
        if (args.Length != 2)
            throw new FormatException("Usage: compare study.tsv");
        var rows = Rows(args[1]);
        Require(rows.Count >= 2, "Include capacities and a workload.");
        var pages = rows.Skip(1).SelectMany(r => r).SelectMany(s => s.Split((char[]? )null, StringSplitOptions.RemoveEmptyEntries)).ToArray();
        Require(pages.Length > 0, "Empty workload.");
        foreach (var raw in rows[0].Distinct())
        {
            var capacity = Integer(raw);
            Require(capacity > 0, "Capacity must be positive.");
            var recent = new LinkedList<string>();
            var index = new Dictionary<string, LinkedListNode<string>>();
            var fifo = new Queue<string>();
            var members = new HashSet<string>();
            int lruHits = 0, fifoHits = 0;
            bool difference = false;
            for (int i = 0; i < pages.Length; i++)
            {
                string p = pages[i];
                string? lruOut = null, fifoOut = null;
                if (index.Remove(p, out var node))
                {
                    recent.Remove(node);
                    lruHits++;
                }
                else if (recent.Count == capacity)
                {
                    lruOut = recent.Last!.Value;
                    index.Remove(lruOut);
                    recent.RemoveLast();
                }

                index[p] = recent.AddFirst(p);
                if (members.Contains(p))
                    fifoHits++;
                else
                {
                    if (fifo.Count == capacity)
                    {
                        fifoOut = fifo.Dequeue();
                        members.Remove(fifoOut);
                    }

                    fifo.Enqueue(p);
                    members.Add(p);
                }

                if (!difference && lruOut != fifoOut)
                {
                    Console.WriteLine($"{capacity} frames: first eviction difference at access {i + 1} ({p}): LRU {lruOut ?? "none"}, FIFO {fifoOut ?? "none"}");
                    difference = true;
                }
            }

            if (!difference)
                Console.WriteLine($"{capacity} frames: eviction decisions agree throughout.");
            Console.WriteLine($"{capacity} frames, {pages.Length} accesses: LRU {lruHits} hits ({100m * lruHits / pages.Length:0.#}%), FIFO {fifoHits} hits ({100m * fifoHits / pages.Length:0.#}%)");
        }
    }

    static List<string[]> Rows(string file, int width = 0, bool allowEmpty = false)
    {
        using var reader = file == "-" ? Console.In : File.OpenText(file);
        var rows = new List<string[]>();
        int line = 0;
        while (reader.ReadLine()is string s)
        {
            line++;
            if (string.IsNullOrWhiteSpace(s) || s.TrimStart().StartsWith("#"))
                continue;
            var cells = s.Split('\t', StringSplitOptions.TrimEntries);
            if ((width > 0 && cells.Length != width) || (!allowEmpty && cells.Any(string.IsNullOrEmpty)))
                throw new FormatException($"Invalid columns on line {line}.");
            rows.Add(cells);
        }

        if (rows.Count == 0)
            throw new FormatException("Empty input.");
        return rows;
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
            throw new FormatException(message);
    }

    static long Integer(string text)
    {
        var value = long.Parse(text);
        Require(value >= 0, "Count must be nonnegative.");
        return value;
    }
}
