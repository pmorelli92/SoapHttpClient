using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Linq;
namespace SoapHttpClient.Fixtures.Attributes
{
    public class InlineDefaultDataAttribute : InlineAutoDataAttribute
    {
        public InlineDefaultDataAttribute(
            params object[] valuesAndCustomizationTypes)
            : base(
                new DefaultDataAttribute(
                    valuesAndCustomizationTypes
                        .Where(ct => IsCustomizationType(ct))
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
