#if ruin_my_life
using JetBrains.Annotations;

namespace _PristineMeadow.Extensions.Attributes.References {

    public class TryAddComponentAttribute : RequiredReferenceAttribute {

        public string Name;

        public TryAddComponentAttribute() {}
        public TryAddComponentAttribute([NotNull] string name) => Name = name;

    }

    public class TryGetComponentAttribute : RequiredReferenceAttribute {}

    public class GetComponentInParentAttribute : RequiredReferenceAttribute {}

    public class GetComponentsInChildrenAttribute : RequiredReferenceAttribute {}

    public class GetOrInstantiateChildAttribute : RequiredReferenceAttribute {

        public string ChildName;
        public string ParentFieldName;

        public GetOrInstantiateChildAttribute([NotNull] string childName) => ChildName = childName;

        public GetOrInstantiateChildAttribute([NotNull] string childName, string parentFieldName) {
            ChildName = childName;
            ParentFieldName = parentFieldName;
        }

    }

}
#endif