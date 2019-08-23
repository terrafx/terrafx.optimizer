// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

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



            return exitCode;
        }
    }
}
