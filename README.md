# 4.0Parser
A (mostly complete) LR Parser Generator that functions completely in-memory (no code generation required)

## Currently Under Development

[![Master Build status](https://ci.appveyor.com/api/projects/status/dq99fo90n27j8buq/branch/master?svg=true&passingText=master%20-%20passing&failingText=master%20-%20failing&pendingText=master%20-%20pending)](https://ci.appveyor.com/project/KallynGowdy/4-0parser/branch/master)
[![Develop Build status](https://ci.appveyor.com/api/projects/status/dq99fo90n27j8buq/branch/develop?svg=true&passingText=develop%20-%20passing&failingText=develop%20-%20failing&pendingText=develop%20-%20pending)](https://ci.appveyor.com/project/KallynGowdy/4-0parser/branch/master)

Goals:

- A reusable Syntax Tree API that functions and appears similar to Microsoft's Project Roslyn API.
- A super parser generator that can create a parser for any context-free grammar and parse it into a strongly-typed syntax tree

### Right Now

1). Remove unused code
  - [x] Get rid of unnessesary projects

2). Add Syntax Tree API
  - [x] Add basic working implementation
  - [ ] Refactor to mimic Roslyn API
  - [ ] Add detailed documentation on using and extending (utilize tests for examples)

3). Refactor Parser Generator
  - [ ] Simplify API, Write Tests to describe the target API
  - [ ] Integrate with new Syntax Tree API
  - [ ] Add high level features for multiple parsing passes (Lexer/Parser) to Simplify the API and make it easier to use.
  - [ ] Build Parser Code Generator
