# LRU Trace

Replay a memory access sequence and see which pages an LRU cache evicts.

## Try it

Install the .NET 10 SDK, open this folder, and run:

```sh
dotnet run -- example.tsv
```

Look for `1 hits, 4 misses` in the output. To use your own data, copy the example and pass the new filename. Use `-` to read from standard input.

## Input

First line: positive cache capacity. Remaining lines: space-separated page labels. Cache contents are printed most recently used first. Separate columns with actual tabs, omit a header row, and use a period for decimal fractions.

Built for computer science coursework. [ISCED-F field 061](https://uis.unesco.org/sites/default/files/documents/isced-fields-of-education-and-training-2013-en.pdf).
