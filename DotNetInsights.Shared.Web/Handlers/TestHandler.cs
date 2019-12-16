using System.Threading.Tasks;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Services;
using DotNetInsights.Shared.Services.Attributes;

namespace DotNetInsights.Shared.WebApp.Handlers
{
    public class TestHandler : DefaultEventHandler<IEvent<Test>>
    {
        public override async Task<IEvent<Test>> Push(IEvent<Test> @event)
        {
            return @event;
        }
    }

    [OptionalRequired(1, nameof(PetTypeId), nameof(PetType))]
    public class Test
    {
        public int Id { get; set; }
        public int PetTypeId { get; set; }
        public string PetType { get; set; }
        public string Samuel { get; set; }
        public string Tom { get; set; }
        public string Harry { get; set; }
    }
}