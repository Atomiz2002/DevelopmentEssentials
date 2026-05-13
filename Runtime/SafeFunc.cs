using System;
using DevelopmentEssentials.Extensions.CS;

namespace DevelopmentEssentials {

    public class SafeFunc<TResult> {

        private readonly Func<TResult> func;
        public SafeFunc(Func<TResult> func) => this.func = func;

        public static implicit operator Func<TResult>(SafeFunc<TResult> wrapper) => () =>
            wrapper.func.InvokeSafe();

    }

    public class SafeFunc<T, TResult> {

        private readonly Func<T, TResult> func;
        public SafeFunc(Func<T, TResult> func) => this.func = func;

        public static implicit operator Func<T, TResult>(SafeFunc<T, TResult> wrapper) => a =>
            wrapper.func.InvokeSafe(a);

    }

    public class SafeFunc<T1, T2, TResult> {

        private readonly Func<T1, T2, TResult> func;
        public SafeFunc(Func<T1, T2, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, TResult>(SafeFunc<T1, T2, TResult> w) => (a1, a2) =>
            w.func.InvokeSafe(a1, a2);

    }

    public class SafeFunc<T1, T2, T3, TResult> {

        private readonly Func<T1, T2, T3, TResult> func;
        public SafeFunc(Func<T1, T2, T3, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, TResult>(SafeFunc<T1, T2, T3, TResult> w) => (a1, a2, a3) =>
            w.func.InvokeSafe(a1, a2, a3);

    }

    public class SafeFunc<T1, T2, T3, T4, TResult> {

        private readonly Func<T1, T2, T3, T4, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, TResult>(SafeFunc<T1, T2, T3, T4, TResult> w) => (a1, a2, a3, a4) =>
            w.func.InvokeSafe(a1, a2, a3, a4);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, TResult>(SafeFunc<T1, T2, T3, T4, T5, TResult> w) => (a1, a2, a3, a4, a5) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, TResult> w) => (a1, a2, a3, a4, a5, a6) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, TResult> w) => (a1, a2, a3, a4, a5, a6, a7) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8, a9) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8, a9);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);

    }

    public class SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> {

        private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func;
        public SafeFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func) => this.func = func;

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(SafeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> w) => (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16) =>
            w.func.InvokeSafe(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);

    }

}