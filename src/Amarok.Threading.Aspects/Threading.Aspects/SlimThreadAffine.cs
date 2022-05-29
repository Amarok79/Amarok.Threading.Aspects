// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable 0169

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;


namespace Amarok.Threading.Aspects;


[Inherited]
public class SlimThreadAffine : TypeAspect
{
    public override void BuildEligibility(IEligibilityBuilder<INamedType> builder)
    {
        base.BuildEligibility(builder);

        builder.MustBeNonStatic();
    }

    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        base.BuildAspect(builder);

        _BuildAspect_Properties(builder);
        _BuildAspect_Methods(builder);
    }

    [CompileTime]
    private void _BuildAspect_Methods(IAspectBuilder<INamedType> builder)
    {
        foreach (var method in builder.Target.Methods)
        {
            if (method.IsStatic || method.IsAbstract || method.IsOpenGeneric)
            {
                continue;
            }

            builder.Advice.Override(
                method,
                new MethodTemplateSelector(
                    nameof(MethodTemplate),
                    nameof(AsyncMethodTemplate),
                    null,
                    null,
                    nameof(AsyncEnumerableMethodTemplate)
                )
            );
        }
    }

    [CompileTime]
    private void _BuildAspect_Properties(IAspectBuilder<INamedType> builder)
    {
        foreach (var property in builder.Target.Properties)
        {
            if (property.IsStatic || property.IsAbstract)
            {
                continue;
            }

            if (property.Writeability == Writeability.ConstructorOnly)
            {
                continue;
            }

            if (property.Writeability == Writeability.InitOnly || property.Writeability == Writeability.None)
            {
                builder.Advice.OverrideAccessors(property, nameof(PropertyGetTemplate));
            }

            if (property.Writeability == Writeability.All)
            {
                builder.Advice.OverrideAccessors(property, nameof(PropertyGetTemplate), nameof(PropertySetTemplate));
            }
        }
    }


    [Template]
    public dynamic? MethodTemplate()
    {
        __SlimThreadAffine_VerifyAccess(meta.Target.Member.ToDisplayString());

        return meta.Proceed();
    }

    [Template]
    public Task<dynamic?> AsyncMethodTemplate()
    {
        __SlimThreadAffine_VerifyAccess(meta.Target.Member.ToDisplayString());

        return meta.ProceedAsync();
    }

    [Template]
    public async IAsyncEnumerable<dynamic?> AsyncEnumerableMethodTemplate()
    {
        __SlimThreadAffine_VerifyAccess(meta.Target.Member.ToDisplayString());

        await foreach (var item in meta.ProceedAsyncEnumerable())
        {
            __SlimThreadAffine_VerifyAccess(meta.Target.Member.ToDisplayString());

            yield return item;
        }
    }



    [Template]
    public dynamic? PropertyGetTemplate()
    {
        __SlimThreadAffine_VerifyAccess(meta.Target.Member.ToDisplayString());

        return meta.Proceed();
    }

    [Template]
    public void PropertySetTemplate(dynamic value)
    {
        __SlimThreadAffine_VerifyAccess(meta.Target.Member.ToDisplayString());

        meta.Proceed();
    }



    [Introduce(WhenExists = OverrideStrategy.Ignore)]
    private protected readonly Int32 __SlimThreadAffine_ManagedThreadId = Environment.CurrentManagedThreadId;


    [Introduce(WhenExists = OverrideStrategy.Ignore)]
    private protected void __SlimThreadAffine_VerifyAccess(String memberName)
    {
        Console.WriteLine("xxx");

        if (Environment.CurrentManagedThreadId == __SlimThreadAffine_ManagedThreadId)
        {
            return;
        }

        __SlimThreadAffine_Throw(memberName);
    }

    [Introduce(WhenExists = OverrideStrategy.Ignore)]
    private protected void __SlimThreadAffine_Throw(String memberName)
    {
        throw new InvalidOperationException(
            $"Thread '{Environment.CurrentManagedThreadId}' is not allowed to access member '{memberName}' " +
            $"on object '{ToString()}' of type '{GetType().AssemblyQualifiedName}' as the object is bound " +
            $"to thread '{__SlimThreadAffine_ManagedThreadId}'."
        );
    }
}
