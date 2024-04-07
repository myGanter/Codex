using Codex.CQRS;
using Codex.Dispatcher;
using Codex.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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
            var result = await _dispatcher.DispatchResultAsync<ParseNumberDto, string, ErrorResult>(dto);

            return result.IsSuccess ? Ok(result.Result) : BadRequest(result.Error);
        }

        [HttpPost(nameof(ParseNumber2))]
        public async Task<IActionResult> ParseNumber2([FromBody] ParseNumberDto dto)
        {
            var result = (ResultOr<string, ErrorResult>)await _dispatcher.DispatchResultAsync(dto);

            return result.IsSuccess ? Ok(result.Result) : BadRequest(result.Error);
        }
    }

    public class ErrorResult
    {
        public string Error { get; set; }
    }

    public class ParseNumberDto : IDtoContract<string, ErrorResult>
    {
        [Required]
        [RegularExpression(@"[+-]?([0-9]*[.])?[0-9]+", ErrorMessage = "Validation faild.")]
        public string Number { get; set; }
    }

    public class ValidationAsyncDecorator<TDto, TOut> : AsyncHandlerDecorator<TDto, TOut, ErrorResult>
        where TOut : class
        where TDto : IDtoContract<TOut, ErrorResult>
    {
        protected override async Task<ResultOr<TOut, ErrorResult>> DecorateActionAsync(DecorateInDto<TDto, TOut, ErrorResult> dto, CancellationToken token)
        {
            var context = new ValidationContext(dto.In);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(dto.In, context, results, true))
            {
                return new ErrorResult() { Error = string.Join(", ", results.Select(x => x.ErrorMessage)) };
            }

            return await Task.FromResult(default(TOut));
        }
    }

    public class ParseNumberAsyncHandler : IAsyncHandler<ParseNumberDto, string, ErrorResult>
    {
        public async Task<ResultOr<string, ErrorResult>> HandleAsync(ParseNumberDto dto, CancellationToken token = default)
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

    public class ParseNumberAfterDecorator : AsyncHandlerDecorator<ParseNumberDto, string, ErrorResult>
    {
        protected override async Task<ResultOr<string, ErrorResult>> DecorateActionAsync(DecorateInDto<ParseNumberDto, string, ErrorResult> dto, CancellationToken token)
        {
            if (!dto.Out.IsSuccess)
                return dto.Out;

            return await Task.FromResult((dto.Out.Result ?? string.Empty) + " ( ͡° ͜ʖ ͡°)");
        }
    }
}