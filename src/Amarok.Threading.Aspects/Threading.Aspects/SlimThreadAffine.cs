// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable 0169

using System;
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

        foreach (var property in builder.Target.Properties)
        {
            if (property.IsStatic)
            {
                continue;
            }

            if (property.Writeability != Writeability.All)
            {
                continue;
            }


            builder.Advice.OverrideAccessors(property, nameof(FieldGetTemplate), nameof(FieldSetTemplate));
        }
    }


    [Template]
    public dynamic? FieldGetTemplate()
    {
        __SlimThreadAffine_VerifyAccess(meta.Target.Member.ToDisplayString());

        return meta.Proceed();
    }

    [Template]
    public void FieldSetTemplate(dynamic value)
    {
        __SlimThreadAffine_VerifyAccess(meta.Target.Member.ToDisplayString());

        meta.Proceed();
    }



    [Introduce(WhenExists = OverrideStrategy.Ignore)]
    private protected readonly Int32 __SlimThreadAffine_ManagedThreadId = Environment.CurrentManagedThreadId;


    [Introduce(WhenExists = OverrideStrategy.Ignore)]
    private protected void __SlimThreadAffine_VerifyAccess(String memberName)
    {
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
