using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models.Linkables;
using MediatR;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class AddressSystemFromZipTests {
        internal static IRequestHandler<AddressSystemFromZipCode.Command, IReadOnlyCollection<GridLinkable>> Handler;

        private readonly Dictionary<string, List<GridLinkable>> _links = new Dictionary<string, List<GridLinkable>>(1);

        public AddressSystemFromZipTests()
        {
            _links.Add("1", new List<GridLinkable> { new ZipGridLink(1, "grid", 1) });
            var mockCache = new Mock<ILookupCache>();
            mockCache.Setup(x => x.ZipCodesGrids).Returns(_links);

            Handler = new AddressSystemFromZipCode.Handler(mockCache.Object, new Mock<ILogger>().Object);
        }

        [Fact]
        public async Task Should_return_grid_from_zip() {
            var request = new AddressSystemFromZipCode.Command(1);
            var result = await Handler.Handle(request, CancellationToken.None);

            result.Count.ShouldBe(1);
            result.First().Grid.ShouldBe("grid");
        }

        [Fact]
        public async Task Should_return_empty_when_zip_not_found() {
            var request = new AddressSystemFromZipCode.Command(0);
            var result = await Handler.Handle(request, CancellationToken.None);

            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_return_empty_when_zip_is_null() {
            var request = new AddressSystemFromZipCode.Command(null);
            var result = await Handler.Handle(request, CancellationToken.None);

            result.ShouldBeEmpty();
        }
    }
}
