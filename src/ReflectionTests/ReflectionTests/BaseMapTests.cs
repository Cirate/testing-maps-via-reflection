using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Mapster;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using ReflectionTests.Domain;

namespace ReflectionTests
{
    [TestFixture]
    public abstract class BaseMapTests<TMap, TSource, TDestination, TDestinationFactory> where TMap : IMap
    {
        private readonly IAdapter _adapter = new Adapter();
        private readonly Type _destinationFactoryType = typeof(TDestinationFactory);
        private readonly Type _destinationType = typeof(TDestination);
        private readonly Type _mapType = typeof(TMap);
        private readonly Type _sourceType = typeof(TSource);

        protected abstract TSource Source { get; }
        protected abstract object[] FactoryParameters { get; }
        protected abstract string FactoryMethodName { get; }

        [Test]
        public void SourceIsMappedCorrectly()
        {
            //Arrange
            var expectedResult = (TDestination)Substitute.For(new[] { _destinationType }, null);
            var mocks = CreateMocks();
            var mockedFactory = GetMockedFactory(mocks);

            RegisterMap(mocks);

            mockedFactory
                .GetType()
                .GetTypeInfo()
                .GetMethod(FactoryMethodName)
                .Invoke(mockedFactory, FactoryParameters)
                .ReturnsForAnyArgs(expectedResult);

            //Act
            var actualResult = _adapter.Adapt(Source, _sourceType, _destinationType);

            //Assert
            GetCallToFactory(mocks)
                .Should()
                .NotBeNull();

            GetCallToFactory(mocks)
                .GetArguments()
                .Should()
                .BeEquivalentTo(FactoryParameters);

            mockedFactory
                .Received(1)
                .GetType()
                .GetTypeInfo()
                .GetMethod(FactoryMethodName)
                .Invoke(mockedFactory, FactoryParameters);

            actualResult
                .Should()
                .BeSameAs(expectedResult);
        }

        private IEnumerable<(Type type, object mock)> CreateMocks()
            => _mapType.GetConstructors()[0]
                       .GetParameters()
                       .Select(x => (x.ParameterType, Substitute.For(new[] { x.ParameterType }, null)))
                       .ToList();

        private void RegisterMap(IEnumerable<(Type type, object mock)> mocks)
        {
            var map = (IMap)Activator.CreateInstance(_mapType, mocks.Select(x => x.mock).ToArray());
            map.Register();
        }

        private ICall GetCallToFactory(IEnumerable<(Type type, object mock)> mocks)
            => mocks.SelectMany(x => x.mock.ReceivedCalls())
                    .SingleOrDefault(x => x.GetReturnType() == _destinationType);

        private object GetMockedFactory(IEnumerable<(Type type, object mock)> mocks)
            => mocks.Single(x => x.type == _destinationFactoryType).mock;
    }
}