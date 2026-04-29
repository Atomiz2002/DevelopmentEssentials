#if DEVELOPMENT_ESSENTIALS_RUNTIME_UNI_TASK
using Cysharp.Threading.Tasks;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class UniTaskExtensions {

        public static bool IsNull(this UniTask uniTask)       => uniTask.Status == UniTaskStatus.Pending;
        public static bool IsNull<T>(this UniTask<T> uniTask) => uniTask.Status == UniTaskStatus.Pending;

    }

}
#endif