using Microsoft.Extensions.Options;

using S3ImageProcessing.Data;
using S3ImageProcessing.Services.Implementations;
using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing.UnitTests
{
    public class ParsedImageStoreTests
    {
        private readonly IParsedImageStore _parsedImageStore;

        public ParsedImageStoreTests()
        {
            _parsedImageStore = new ParsedImageStore(
                new DbAccess(
                    Options.Create(
                        new DatabaseOption()
                        {
                            ProviderName = "AKIAWQMANXGNLZIT73WZ",
                            ConnectionString = "M6H2A6+FqLFtiLYJJHBDuLSlnI0ms6XNl1eE2Uw8",
                        }))
            );
        }
    }
}