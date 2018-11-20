﻿using System;
using System.Reflection;

using JetBrains.Annotations;

namespace SKBKontur.Catalogue.NUnit.Extensions.EdiTestMachinery.Impl
{
    public static class ExceptionExtensions
    {
        public static void Rethrow([NotNull] this Exception exception)
        {
            prepForRemotingMethodInfo.Invoke(exception, new object[0]);
            throw exception;
        }

        public static void RethrowInnerException([NotNull] this TargetInvocationException exception)
        {
            if(exception.InnerException != null)
                exception.InnerException.Rethrow();
            throw exception;
        }

        private static readonly MethodInfo prepForRemotingMethodInfo = typeof(Exception).GetMethod("PrepForRemoting", BindingFlags.NonPublic | BindingFlags.Instance);
    }
}