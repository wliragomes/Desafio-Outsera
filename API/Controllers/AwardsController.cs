using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwardsController : ControllerBase
    {
        private readonly IAwardsService _awardsService;

        public AwardsController(IAwardsService awardsService)
        {
            _awardsService = awardsService;
        }

        [HttpGet("producers/intervals")]
        public async Task<IActionResult> GetProducerIntervals()
        {
            try
            {
                var result = await _awardsService.GetProducerIntervalsAsync();
                if (result == null)
                    return NotFound("Nenhum intervalo encontrado.");

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        [HttpPost("load-csv")]
        public async Task<IActionResult> LoadDataFromCsv([FromQuery] string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("O caminho do arquivo deve ser fornecido.");

            if (!System.IO.File.Exists(filePath))
                return NotFound($"O arquivo '{filePath}' não foi encontrado.");

            try
            {
                await _awardsService.LoadDataFromCsvAsync(filePath);
                return Ok("Dados carregados com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao carregar o arquivo: {ex.Message}");
            }
        }
    }
}
