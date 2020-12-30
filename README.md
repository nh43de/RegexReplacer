# RegexReplacer
Performs regex replacing in files with optional preview via command line

## Compiling

Use csc (CSharp compiler command line). You can do this by opening a Developer Command Prompt if you have VS installed.

`csc -recurse:src\*.cs -out:rxr.exe


## Usage

```

RXR.exe - performs regular expression replacing of file contents.
Usage: rxr <dir> <pattern> <replacement> [/pcwr]

 p   Preview changes
 c   Case sensitive
 w   Ignore pattern whitespace
 r   Recursive search
 
 
```