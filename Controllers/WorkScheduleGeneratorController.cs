namespace rotating_work_schedule.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using rotating_work_schedule.Models;
using YourNamespace.Repositories;
using rotating_work_schedule.GeneticAlgorithm;

[ApiController]
[Route("api/workschedule")]
public class WorkScheduleGeneratorController() : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        WorkScheduleGenerator workScheduleGenerator = new WorkScheduleGenerator();
        
        // return Ok(workScheduleGenerator.generatePopulation());
        TimeSpan teste = new TimeSpan(2, 0, 1);

        return Ok(teste);
    }
}
