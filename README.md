# LRU Trace

Replay a workload across cache sizes and policies, showing the first eviction disagreement and measured hit rates.

## Try it

From this folder with the .NET 10 SDK installed:

```sh
dotnet run -- compare study.tsv
```

First line: one or more positive capacities separated by tabs. Remaining lines: whitespace-separated page identifiers from your workload. Each policy starts with empty frames for each capacity. This models page replacement in a fully associative cache.

The included study.tsv uses tab-separated fields. Comment lines start with `#`.

The original exercise remains available with `dotnet run -- example.tsv`.

## When to use it

An operating-systems student wants to explain why loading C evicts B even though A was loaded first.

Example result:

```text
2 frames: first eviction difference at access 4 (C): LRU B, FIFO A
2 frames, 7 accesses: LRU 1 hits (14.3%), FIFO 2 hits (28.6%)
3 frames: first eviction difference at access 7 (D): LRU C, FIFO A
3 frames, 7 accesses: LRU 3 hits (42.9%), FIFO 3 hits (42.9%)
```

Related course or field workflow: [Carnegie Mellon cache lab recitation](https://www.andrew.cmu.edu/course/14-513-f18/recitations/recitation07-cachelab.pdf).
