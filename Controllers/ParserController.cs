using Microsoft.AspNetCore.Mvc;
using BabyNI.Models;

[Route("api/[controller]")]
[ApiController]
public class DataParserController : ControllerBase
{
    private readonly DataParserService _dataParserService;

    public DataParserController(DataParserService dataParserService)
    {
        _dataParserService = dataParserService;
    }

    [HttpPost]
    public IActionResult ProcessFile()
    {
        // Get the file path from the request (you may need to adjust the way you pass the file path)
        string inputFilePath = Request.Form["inputFilePath"];

        if (string.IsNullOrEmpty(inputFilePath))
        {
            return BadRequest("Invalid data.");
        }

        // Process the input data using the DataParserService
        _dataParserService.ProcessFile(inputFilePath);

        return Ok("Data processed successfully.");
    }
}
