using CodexCQRS.AspNet.Dtos;
using CodexCQRS.CQRS;
using CodexCQRS.Dtos;
using CodexCQRS.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DemoWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestStringGeneratorController : ControllerBase
    {
        private readonly ILogger<TestNumberController> _logger;
        private readonly IGenerateStringAsyncHandler _generateStringhandler;

        public TestStringGeneratorController(ILogger<TestNumberController> logger,
            IGenerateStringAsyncHandler generateStringhandler)
        {
            _logger = logger;
            _generateStringhandler = generateStringhandler;
        }

        [HttpPost(nameof(GenerateString))]
        public async Task<IActionResult> GenerateString([FromBody] GenerateStringDto dto, CancellationToken ct)
        {
            return await _generateStringhandler.HandleAsync(dto, ct)
                .MatchAsync(x => (IActionResult)Ok(x), err => (IActionResult)BadRequest(err));
        }

        [HttpPost(nameof(GenerateStringWithDecorators))]
        public async Task<IActionResult> GenerateStringWithDecorators([FromBody] GenerateStringDto dto, CancellationToken ct)
        {
            return await _generateStringhandler.DecoratedAsyncResultHandler.HandleAsync(dto, ct)
                .MatchAsync(x => (IActionResult)Ok(x), err => (IActionResult)BadRequest(err));
        }
    }

    public class GenerateStringDto : IDtoContract<GenerateStringResultDto, ErrorDto>
    {
        [Required]
        [Range(1, 1000, ErrorMessage = "Value range should be 1 to 1000.")]
        public int StringLength { get; set; }
    }

    public class GenerateStringResultDto
    {
        public string Result { get; set; } = null!;
    }

    public interface IGenerateStringAsyncHandler : IAsyncHandler<GenerateStringDto, GenerateStringResultDto, ErrorDto>,
        IHasAsyncDecoratedHandler<GenerateStringDto, GenerateStringResultDto, ErrorDto>
    { }

    public class GenerateStringAsyncHandler : IGenerateStringAsyncHandler
    {
        public IAsyncHandler<GenerateStringDto, GenerateStringResultDto, ErrorDto> DecoratedAsyncResultHandler { get; set; } = null!;

        public async Task<ResultOr<GenerateStringResultDto, ErrorDto>> HandleAsync(GenerateStringDto dto, CancellationToken token = default)
        {
            var size = dto.StringLength;
            
            return await Task.FromResult(new GenerateStringResultDto()
            {
                Result = new string(Enumerable.Range(0, size).Select(x => (char)Random.Shared.Next(32, 126)).ToArray())
            });
        }
    }

    public class GenerateStringToUpperAsyncDecorator<TDto> : AsyncHandlerDecorator<TDto, GenerateStringResultDto, ErrorDto>
        where TDto : IDtoContract<GenerateStringResultDto, ErrorDto>
    {
        protected override Task<ResultOr<GenerateStringResultDto, ErrorDto>> DecorateActionAsync(DecorateInDto<TDto, GenerateStringResultDto, ErrorDto> dto, CancellationToken token)
        {
            return Task.FromResult(dto.Out.Match(x => 
            {
                x!.Result = x.Result.ToUpper();

                return new ResultOr<GenerateStringResultDto, ErrorDto>(x);
            }));
        }
    }
}
