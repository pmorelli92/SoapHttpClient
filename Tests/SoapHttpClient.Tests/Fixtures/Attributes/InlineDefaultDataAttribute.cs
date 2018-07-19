using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;

namespace SoapHttpClient.Tests.Fixtures.Attributes
{
    public class InlineDefaultDataAttribute : InlineAutoDataAttribute
    {
        public InlineDefaultDataAttribute(
            params object[] valuesAndCustomizationTypes)
            : base(
                new DefaultDataAttribute(
                    valuesAndCustomizationTypes
                        .Where(IsCustomizationType)
                        .Cast<Type>()
                        .ToArray()),
                values: valuesAndCustomizationTypes.Where(x => !IsCustomizationType(x)).ToArray())
        { }

        private static bool IsCustomizationType(object target)
        {
            return ToCustomizationTypeOrDefault(target) != null;
        }

        private static Type ToCustomizationTypeOrDefault(object target)
        {
            var type = target as Type;
            if (type != null && typeof(ICustomization).IsAssignableFrom(type))
            {
                return type;
            }
            return null;
        }
    }
}