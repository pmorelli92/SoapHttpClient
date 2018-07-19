using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using SoapHttpClient.Tests.Fixtures.Customizations;

namespace SoapHttpClient.Tests.Fixtures.Attributes
{
    public class DefaultDataAttribute : AutoDataAttribute
    {
        public DefaultDataAttribute()
            : base(fixtureFactory: FixtureFactory(new Type[] { }))
        { }

        public DefaultDataAttribute(params Type[] customizationTypes)
            : base(fixtureFactory: FixtureFactory(customizationTypes ?? new Type[] { }))
        { }

        private static Func<IFixture> FixtureFactory(Type[] customizationTypes)
        {
            return () =>
            {
                var fixture = new Fixture();
                fixture.Customize(new CompositeCustomization(
                    new ICustomization[] { new DefaultCustomization() }
                        .Concat(customizationTypes.Select(t => (ICustomization)Activator.CreateInstance(t, null)))));
                fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());

                return fixture;
            };
        }
    }
}