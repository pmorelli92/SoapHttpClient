using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace SoapHttpClient.Fixtures.Customizations
{
    public class DefaultCustomization : CompositeCustomization
    {
        public DefaultCustomization()
            : base(
                new MultipleCustomization(),
                new AutoMoqCustomization())
        { }
    }
}
