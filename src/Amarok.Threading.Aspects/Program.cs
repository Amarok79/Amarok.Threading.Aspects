// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using System.Threading.Tasks;
using Amarok.Threading.Aspects;


namespace Amarok;


public static class Program
{
    public static async Task Main()
    {
        var sample = new SlimThreadAffineSample();

        await sample.MemberAsyncMethod();

        await foreach (var x in sample.MemberAsyncIterator())
        {
        }
    }
}
