# Smoke tester

This small console application runs a set of provided URLs through the ExtractorResolver and prints the extracted source and headers.

How to run

1. Clone the repo and checkout the branch:

   git clone https://github.com/schmitt627235-prog/streamflix-extractors-csharp.git
   git checkout convert/smoke-tests

2. Build and run the smoke tester:

   dotnet run --project src/Streamflix.Extractors.SmokeTester/Streamflix.Extractors.SmokeTester.csproj

Notes
- The smoke tester performs live HTTP requests against the target hosts. Your local environment's network, DNS, and any blocking (Cloudflare, captcha, geo‑blocks) will affect results.
- If a host blocks the request, the extractor might throw. Inspect the exception and the extractor implementation to adjust headers or JS deobfuscation.
