#if ruin_my_life
using System;
using Sirenix.Serialization;

namespace _PristineMeadow.Extensions.Attributes.References {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredReferenceAttribute : OdinSerializeAttribute {}

}
#endif