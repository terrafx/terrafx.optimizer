// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using TerraFX.Optimization.CodeAnalysis;

namespace TerraFX.Optimization;

public static class Program
{
    private static readonly RootCommand s_rootCommand = new RootCommand("TerraFX IL Optimizer");

    public static async Task<int> Main(params string[] args)
    {
        Handler.SetHandler(s_rootCommand, Run);

        var parser = new CommandLineBuilder(s_rootCommand)
            .UseHelp()
            .UseEnvironmentVariableDirective()
            .UseParseDirective()
            .UseSuggestDirective()
            .RegisterWithDotnetSuggest()
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .UseExceptionHandler()
            .CancelOnProcessTermination()
            .Build();
        return await parser.InvokeAsync(args).ConfigureAwait(false);
    }

    public static void Run()
    {
        using var peStream = File.OpenRead("TerraFX.Optimization.dll");
        using var peReader = new PEReader(peStream);

        var metadataReader = peReader.GetMetadataReader();

        foreach (var methodDefinitionHandle in metadataReader.MethodDefinitions)
        {
            var methodDefinitionInfo = CompilerInfo.Instance.Resolve(methodDefinitionHandle, metadataReader);
            Debug.Assert(methodDefinitionInfo is not null);

            var relativeVirtualAddress = methodDefinitionInfo.RelativeVirtualAddress;

            if (relativeVirtualAddress == 0)
            {
                continue;
            }

            Console.WriteLine(methodDefinitionInfo);

            var methodBody = peReader.GetMethodBody(relativeVirtualAddress);
            var flowgraph = Flowgraph.Decode(metadataReader, methodBody);

            var i = 0;
            foreach (var block in flowgraph.Blocks)
            {
                Console.WriteLine($"  // BB{i:X2}");

                for (var instruction = block.FirstInstruction; instruction != block.LastInstruction; instruction = instruction.Next!)
                {
                    Console.Write("    ");
                    Console.WriteLine(instruction);
                }

                Console.Write("    ");
                Console.WriteLine(block.LastInstruction);

                i++;
            }
        }
    }
}
