using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using SoapHttpClient.Fixtures.Customizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoapHttpClient.Fixtures.Attributes
{
    public class DefaultDataAttribute : AutoDataAttribute
    {
        private ICustomization _defaultCustomization;
        private Type[] _customizationTypes;

        public DefaultDataAttribute(params Type[] customizationTypes)
        {
            _defaultCustomization = new DefaultCustomization();
            _customizationTypes = customizationTypes;
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            this.Fixture.Customize(new CompositeCustomization(
                    new ICustomization[] { _defaultCustomization }
                        .Concat(_customizationTypes.Select(t => (ICustomization)Activator.CreateInstance(t, null)))));

            return base.GetData(methodUnderTest);
        }
    }
}
