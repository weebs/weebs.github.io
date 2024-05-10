cd Fable
dotnet run --project src/Fable.Cli/Fable.Cli.fsproj -- src/fable-standalone/test/bench-compiler/bench-compiler.fsproj --watch --runWatch "npx esbuild --bundle --outfile=fable.js src/fable-standalone/test/bench-compiler/test.ts"
