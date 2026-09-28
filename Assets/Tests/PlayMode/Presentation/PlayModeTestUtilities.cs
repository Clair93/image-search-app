using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace ImageSearch.Presentation.Tests
{
    public static class PlayModeTestUtilities
    {
        public static IEnumerator WaitUntilOrFail(Func<bool> condition, float timeoutSeconds, string description)
        {
            var elapsed = 0f;

            while (!condition())
            {
                if (elapsed > timeoutSeconds)
                {
                    Assert.Fail($"Timed out after {timeoutSeconds}s waiting for: {description}");
                }

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}
