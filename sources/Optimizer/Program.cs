// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using TerraFX.Optimization.CodeAnalysis;

namespace TerraFX.Optimization
{
    public static class Program
    {
        private static RootCommand s_rootCommand = new RootCommand();

        public static async Task<int> Main(params string[] args)
        {
            s_rootCommand.Description = "TerraFX IL Optimizer";
            s_rootCommand.Handler = CommandHandler.Create(typeof(Program).GetMethod(nameof(Run)));

            return await s_rootCommand.InvokeAsync(args);
        }

        public static int Run(InvocationContext context)
        {
            int exitCode = 0;

            using var peStream = File.OpenRead("TerraFX.Optimization.dll");
            using var peReader = new PEReader(peStream);

            var metadataReader = peReader.GetMetadataReader();

            foreach (var methodDefinitionHandle in metadataReader.MethodDefinitions)
            {
                var methodDefinition = metadataReader.GetMethodDefinition(methodDefinitionHandle);

                var typeHandle = methodDefinition.GetDeclaringType();
                var typeDefinition = metadataReader.GetTypeDefinition(typeHandle);

                var namespaceName = metadataReader.GetString(typeDefinition.Namespace);
                var typeName = metadataReader.GetString(typeDefinition.Name);
                var methodName = metadataReader.GetString(methodDefinition.Name);

                Console.WriteLine($"{namespaceName}.{typeName}.{methodName}");

                var methodBody = peReader.GetMethodBody(methodDefinition.RelativeVirtualAddress);
                var flowgraph = FlowGraph.Decode(metadataReader, methodBody);

                for (int i = 0; i < flowgraph.Blocks.Count; i++)
                {
                    Console.WriteLine($"  BB{i:X2}");
                    var block = flowgraph.Blocks[i];
                    
                    for (var instruction = block.FirstInstruction; instruction != block.LastInstruction; instruction = instruction.Next!)
                    {
                        Console.Write("    ");
                        Console.WriteLine(instruction);
                    }

                    Console.Write("    ");
                    Console.WriteLine(block.LastInstruction);
                }
            }

            return exitCode;
        }
    }
}
