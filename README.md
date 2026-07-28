# Streamflix Extractors (C#)

This repository is an initial scaffolded conversion of the Streamflix Reborn extractors into a .NET 8 class library.

Goal
- Convert all Extractor, API adapters and Scraper logic from the original Streamflix (Kotlin) project into C# (.NET 8) as a reusable library.

What is included in this first commit
- .NET 8 class library project (csproj)
- Extractor base class (Extractor.cs)
- ExtractorResolver (selection logic)
- HttpClientService wrapper
- AngleSharp HTML parser helper
- DI extension to register services
- A minimal Video model

Not yet included (next tasks)
- All concrete extractor classes converted (dozens exist in the original repo). They will be added in follow-up commits in batches.
- Detailed unit tests and CI workflows (can be added on request).

Build instructions

Prerequisites: .NET 8 SDK (or later)

Commands:

1. Clone this repository

   git clone https://github.com/schmitt627235-prog/streamflix-extractors-csharp.git

2. From repository root, create and add solution (optional):

   dotnet new sln -n streamflix.extractors
   dotnet sln add src/Streamflix.Extractors/Streamflix.Extractors.csproj

3. Build

   dotnet build src/Streamflix.Extractors/Streamflix.Extractors.csproj

License & Attribution
- This project is derived from the original Streamflix Reborn project (https://github.com/streamflix-reborn2/streamflix) which is Apache-2.0 licensed. This conversion will carry the same Apache-2.0 license. See LICENSE file.

