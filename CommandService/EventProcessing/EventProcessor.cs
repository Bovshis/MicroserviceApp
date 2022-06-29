using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        
        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _scopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private static EventType DetermineEvent(string message)
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);

            return eventType?.Event switch
            {
                "Platform_published" => EventType.PlatformPublished,
                _ => EventType.Undetermined
            };
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!repo.ExternalPlatformExist(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not add platform to DB: {e.Message}");
            }
        }
    }

    internal enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
