# Parser.Net
A Parser and Parser Generator for .Net

## Currently Under Development

[![Master Build status](https://ci.appveyor.com/api/projects/status/2bllnjr75h37vi5w/branch/master?svg=true&passingText=master%20-%20passing&failingText=master%20-%20failing&pendingText=master%20-%20pending)](https://ci.appveyor.com/project/KallynGowdy/parser-net/branch/master)
[![Develop Build status](https://ci.appveyor.com/api/projects/status/2bllnjr75h37vi5w/branch/develop?svg=true&passingText=develop%20-%20passing&failingText=develop%20-%20failing&pendingText=develop%20-%20pending)](https://ci.appveyor.com/project/KallynGowdy/parser-net/branch/develop)

Goals:

- A reusable Syntax Tree API that functions and appears similar to Microsoft's Project Roslyn API.
- A super parser generator that can create a parser for any context-free grammar and integrate with a strongly-typed syntax tree

### Right Now

1). Refactor Project Structure
  - [x] Get rid of unnecessary projects

2). Add Syntax Tree API
  - [x] Add basic working implementation
  - [ ] Refactor to mimic Roslyn API
  - [ ] Add detailed documentation on using and extending (utilize tests for examples)

3). Refactor Parser Generator
  - [ ] Separate project into different components (Interfaces, Parser Description, Code Generation)
  - [ ] Simplify API, Write Tests to describe the target API, also start with writing interfaces (abstract tests)
  - [ ] Integrate with new Syntax Tree API
  - [ ] Add high level features for multiple parsing passes (Lexer/Parser) to Simplify the API and make it easier to use.
  - [ ] Build Parser Generator

4). Port Projects over to PCL
  - [x] Syntax Tree API
  - [ ] Parser Generator

# License

```
  Copyright 2015 Kallyn Gowdy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
```
