// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

#pragma warning disable LAMA0048

using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Amarok.Threading.Aspects;


[SlimThreadAffine]
public class SlimThreadAffineSample
{
    // Fields

    public const Int32 ConstField = -1;

    public static readonly Int32 StaticReadOnlyField = -1;

    public static Int32 StaticField = -1;

    public readonly Int32 MemberReadOnlyField = 123;

    public Int32 MemberField;


    // Properties

    public static Int32 StaticGetSetProperty { get; set; }

    public static Int32 StaticGetProperty { get; }

    public static Int32 StaticSetProperty
    {
        set => _ = value;
    }

    public Int32 MemberGetSetProperty { get; set; }

    public Int32 MemberGetInitProperty { get; init; }

    public Int32 MemberGetPrivateSetProperty { get; private set; }

    public Int32 MemberGetProperty { get; }

    public Int32 MemberSetProperty
    {
        set => _ = value;
    }

    public Int32 this[Int32 index]
    {
        get => -1;
        set => _ = value;
    }

    // Methods

    public static void StaticMethod()
    {
    }

    public void MemberMethod()
    {
    }

    public Int32 MemberReturnMethod()
    {
        return -1;
    }

    public IEnumerable<Int32> MemberIterator()
    {
        yield return 123;
        yield return 456;
    }

    public async Task MemberAsyncMethod()
    {
        await Task.Yield();

        await Task.Yield();
    }

    public async Task<Int32> MemberAsyncReturnMethod()
    {
        await Task.Yield();
        await Task.Yield();

        return -1;
    }

    public async IAsyncEnumerable<Int32> MemberAsyncIterator()
    {
        await Task.Yield();

        yield return 123;

        await Task.Yield();

        yield return 456;
    }
}

//[SlimThreadAffine]
public class SlimThreadAffineSampleSub : SlimThreadAffineSample
{
    public Int32 SubMemberGetSetProperty { get; set; }
}
