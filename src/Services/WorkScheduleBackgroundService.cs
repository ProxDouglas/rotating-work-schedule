namespace WorkSchedule.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkSchedule.Entities;
using WorkSchedule.GeneticAlgorithm;
using WorkSchedule.GeneticAlgorithm.PrePocessing;
using WorkSchedule.Mappers;
using WorkSchedule.QueueRabbitMQ;

public class WorkScheduleBackgroundService : BackgroundService
{
   private readonly IQueuePubSub _queuePubSub;
   private readonly ILogger<WorkScheduleBackgroundService> _logger;
   private readonly IServiceScope _scope;
   private readonly ChromosomeMapper _chromosomeMapper;

   public WorkScheduleBackgroundService(
       IServiceProvider serviceProvider,
       ILogger<WorkScheduleBackgroundService> logger)
   {
      _scope = serviceProvider.CreateScope();
      _queuePubSub = _scope.ServiceProvider.GetRequiredService<IQueuePubSub>();
      _logger = logger;
      _chromosomeMapper = _scope.ServiceProvider.GetRequiredService<ChromosomeMapper>();
   }

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      _logger.LogInformation("Work Schedule Background Service started");

      while (!stoppingToken.IsCancellationRequested)
      {
         try
         {
            WorkScheduleRequest? order = await _queuePubSub.ConsumeMessage<WorkScheduleRequest>("GenerationRequest");

            if (order != null)
            {
               _logger.LogInformation($"Received order with Id: {order.Id}");
               WorkScheduleGenerated workScheduleGenerated = await ProcessWorkScheduleRequest(order);
               await _queuePubSub.ProduceMessage(workScheduleGenerated, "GenerationResponse", "GenerationResponse");
            }
            else
            {
               await Task.Delay(5000, stoppingToken);
            }
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error occurred while processing work schedule requests");
            // Aguarda um pouco mais em caso de erro
            await Task.Delay(5000, stoppingToken);
         }
      }
   }

   private async Task<WorkScheduleGenerated> ProcessWorkScheduleRequest(WorkScheduleRequest order)
   {
      try
      {
         var configuration = new ConfigurationSchedule(
             order.Employees,
             order.JobPositions,
             order.OperatingSchedule,
             order.WorkDay.ToArray(),
             0
         );

         var generateDayOff = new GenerateDayOff();
         generateDayOff.Run(order.Employees, order.WorkDay);

         var workScheduleGenerator = new WorkScheduleGenerator(configuration);
         var bestSchedule = await workScheduleGenerator.RunGeneticAlgorithmAsync();

         WorkScheduleGenerated workScheduleGenerated = _chromosomeMapper.toWorkScheduleGenerated(bestSchedule, order.Employees, configuration.StartDate);

         return workScheduleGenerated;
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, $"Error processing work schedule for order Id: {order.Id}");
         WorkScheduleGenerated? workScheduleGenerated = new()
         {
            Id = order.Id,
            IsValid = false,
            ErrorMessage = $"Error processing work schedule for order Id: {order.Id} - " + ex.Message
         };
         return workScheduleGenerated;
      }
   }

   public override void Dispose()
   {
      _scope?.Dispose();
      base.Dispose();
   }
}
