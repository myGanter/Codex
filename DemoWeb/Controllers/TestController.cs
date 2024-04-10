using Codex.CQRS;
using Codex.Extensions;
using Codex.Dispatcher;
using Codex.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Codex.AspNet.Dtos;

namespace DemoWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IDispatcher _dispatcher;

        public TestController(ILogger<TestController> logger,
            IDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        [HttpPost(nameof(ParseNumber1))]
        public async Task<IActionResult> ParseNumber1([FromBody] ParseNumberDto dto)
        {
            return await _dispatcher.DispatchResultAsync<ParseNumberDto, string, ErrorDto>(dto)
                .MatchAsync(x => (IActionResult)Ok(x), err => (IActionResult)BadRequest(err));
        }

        [HttpPost(nameof(ParseNumber2))]
        public async Task<IActionResult> ParseNumber2([FromBody] ParseNumberDto dto)
        {
            return ((ResultOr<string, ErrorDto>)await _dispatcher.DispatchResultAsync(dto))
                .Match(x => (IActionResult)Ok(x), err => (IActionResult)BadRequest(err));
        }
    }

    public class ParseNumberDto : IDtoContract<string, ErrorDto>
    {
        [Required]
        [RegularExpression(@"[+-]?([0-9]*[.])?[0-9]+", ErrorMessage = "Validation faild.")]
        public string Number { get; set; }
    }

    public class ParseNumberAsyncHandler : IAsyncHandler<ParseNumberDto, string, ErrorDto>
    {
        public async Task<ResultOr<string, ErrorDto>> HandleAsync(ParseNumberDto dto, CancellationToken token = default)
        {
            var nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };

            var isPi = dto.Number.StartsWith("3.14") && Math.PI.ToString(nfi).StartsWith(dto.Number);
            var isE = dto.Number.StartsWith("2.7") && Math.E.ToString(nfi).StartsWith(dto.Number);

            return await Task.FromResult($"Your number: {(isPi ?  "PI" : isE ? "E" : dto.Number)}");
        }
    }

    public class ParseNumberAfterDecorator : AsyncHandlerDecorator<ParseNumberDto, string, ErrorDto>
    {
        protected override async Task<ResultOr<string, ErrorDto>> DecorateActionAsync(DecorateInDto<ParseNumberDto, string, ErrorDto> dto, CancellationToken token)
        {
            if (!dto.Out.IsSuccess)
                return dto.Out;

            return await Task.FromResult((dto.Out.Result ?? string.Empty) + " ( ͡° ͜ʖ ͡°)");
        }
    }
}