# Development Imformations
Hello, developers! Thank you for your contributions! This document is to list out all the imformations that would help setting up the environment. Hope you have a great day!

## Prerequisites
### Required Software
- **.NET SDK:** 9.0
- **IDE:** Visual Studio or Visual Studio Code (With required pluguns)
- **Git**
### Service Dependencies
- **Redis**
- **OAuth Provider**

## Setup
1. Clone project:
```bash
git clone ...
cd ...
```
2. Restore dependencies:
```bash
dotnet restore
```
> 🗒️**Note:** If you open the solution folder with your IDE, it might restore the dependencies automatically.

## Project Structure
```
OneUrl  ─┬─ OneUrl.slnx
         ├─ OneUrl  ─┬─ OneUrl.csproj
         |           ├─ Components
         |           ├─ Models
         |           ├─ Properties
         |           └─ wwwroot
         └─ OneUrlApi   ─┬─ OneUrlApi.csproj
                         ├─ Api
                         ├─ Models
                         ├─ Properties
                         └─ Services
```

## Ports
The port the project using in development environment is defined in `launchSettings.json` under `Proterties` folder.
|Project|Protocol|Port|
|--- | --- |---|
| OneUrl | Http | 5218 |
| OneUrl | Https | 7136 |
| OneUrlApi | Http | 5290 |
| OneUrlApi | Https | 7026 |

## Code formating
To unify the code formating, please **ONLY USE** auto formating feature from Visual Studio or C# plugin.

### Naming Conventions
Please follow the standard C# naming conventions:
- Class, Function, Property: PascalCase.
- Function variable, Area variables: camelCase

### Razor
Since those plugins don't auto format `.razor` files, for common syntax, please follow the following rules:
1. Code Block (e.g. `@code`, `@if`)
```razor
@if (conditions)
{
    // Contents should indent for 4 spaces
}
@else if(conditions)
{
    // ...
}
@else
{
    // ...
}

@code
{
    // C# code should follow the C# formatting rules.
}
```
2. Page Proterties and Declarations
```razor
@page "/page" //If is a page
@layout ... //If needed
@rendermode InteractiveServer

@using ...
@using ...

@inject ...
@inject ...
```
3. Indentation
- The project should always use **4 spaces** to indent.
- If you're using `Tab` key, please make sure the IDE has set the indentation to 4 spaces.
#### Example
```razor
@page "/example"
@layout MainLayout
@rendermode InteractiveServer

@using OneUrl.Components.Layout

@inject NavigationManager NavigationManager

<div>
    Hello World!
</div>

@if (true)
{
    <div>
        Hi!
    </div>
}

@code
{
    //logic here
}
```