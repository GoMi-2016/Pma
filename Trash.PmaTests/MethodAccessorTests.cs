using NUnit.Framework;

namespace Trash.Pma.Tests
{
    using Exceptions;

    [TestFixture()]
    public class MethodAccessorTests
    {
        private MethodAccessor typeFixed;

        private PrivateMethodSample target;

        private dynamic accessor;

        public class PrivateMethodSample
        {
            private int test(int i)
            {
                return i + 10010;
            }

            private void @null()
            {
                return;
            }
        }

        [SetUp()]
        public void Init()
        {
            target = new PrivateMethodSample();
            typeFixed = new MethodAccessor(target);
            accessor = typeFixed;
        }

        [Test()]
        public void Normal()
        {
            var @int = (int)accessor.test(01101);
            Assert.AreEqual(@int, 11111);
        }

        [Test()]
        public void Indexer()
        {
            var @int = (int)typeFixed["test"](01101);
            Assert.AreEqual(@int, 11111);
        }

        [Test()]
        public void VoidMethod()
        {
            var @null = accessor.@null();
            Assert.IsNull(@null);
        }

        [Test()]
        public void Defined()
        {
            Assert.IsTrue(typeFixed.IsDefined("test", typeof(int)));
        }
        
        [Test()]
        public void Undefined()
        {
            Assert.IsFalse(typeFixed.IsDefined("error"));
        }

        [Test()]
        public void UndefinedException()
        {
            var test = new TestDelegate(() => accessor.error());
            typeFixed.ConfirmDefined = false;
            Assert.Throws<MethodDoesNotExistException>(test);
        }
    }
}