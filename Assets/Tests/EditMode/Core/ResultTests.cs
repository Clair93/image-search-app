using ImageSearch.Core;
using NUnit.Framework;

namespace ImageSearch.Core.Tests
{
    public class ResultTests
    {
        [Test]
        public void Success_PreservesValue()
        {
            var result = new Result<int, string>.Success(42);

            Assert.That(result, Is.InstanceOf<Result<int, string>.Success>());
            Assert.That(((Result<int, string>.Success)result).Value, Is.EqualTo(42));
        }

        [Test]
        public void Error_PreservesValue()
        {
            var result = new Result<int, NetworkError>.Error(NetworkError.ServerError);

            Assert.That(result, Is.InstanceOf<Result<int, NetworkError>.Error>());
            Assert.That(((Result<int, NetworkError>.Error)result).Value, Is.EqualTo(NetworkError.ServerError));
        }

        [Test]
        public void PatternMatching_DistinguishesSuccessAndError()
        {
            Result<int, string> success = new Result<int, string>.Success(1);
            Result<int, string> error = new Result<int, string>.Error("boom");

            var successOutcome = success switch
            {
                Result<int, string>.Success s => $"ok:{s.Value}",
                Result<int, string>.Error e => $"err:{e.Value}",
                _ => "unreachable"
            };

            var errorOutcome = error switch
            {
                Result<int, string>.Success s => $"ok:{s.Value}",
                Result<int, string>.Error e => $"err:{e.Value}",
                _ => "unreachable"
            };

            Assert.That(successOutcome, Is.EqualTo("ok:1"));
            Assert.That(errorOutcome, Is.EqualTo("err:boom"));
        }

        [Test]
        public void Success_SameValue_AreEqual()
        {
            var a = new Result<int, string>.Success(7);
            var b = new Result<int, string>.Success(7);

            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Success_CannotBeCastToError()
        {
            Result<int, string> success = new Result<int, string>.Success(1);

            var asError = success as Result<int, string>.Error;

            Assert.That(asError, Is.Null);
        }
    }
}
