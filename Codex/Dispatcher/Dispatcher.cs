using Codex.Cache;
using Codex.CQRS;
using Codex.Dtos;
using Codex.Exceptions;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Codex.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        private static readonly ConcurrentDictionary<Type, Action<Dispatcher, object, bool>> _dynamicDispatchCache;
        private static readonly ConcurrentDictionary<Type, Func<Dispatcher, object, CancellationToken, bool, Task>> _dynamicDispatchAsyncCache;
        private static readonly ConcurrentDictionary<Type, Func<Dispatcher, object, bool, object>> _dynamicDispatchResultCache;
        private static readonly ConcurrentDictionary<Type, Func<Dispatcher, object, CancellationToken, bool, Task<object>>> _dynamicDispatchResultAsyncCache;

        static Dispatcher()
        {
            _dynamicDispatchCache = new ConcurrentDictionary<Type, Action<Dispatcher, object, bool>>();
            _dynamicDispatchAsyncCache = new ConcurrentDictionary<Type, Func<Dispatcher, object, CancellationToken, bool, Task>>();
            _dynamicDispatchResultCache = new ConcurrentDictionary<Type, Func<Dispatcher, object, bool, object>>();
            _dynamicDispatchResultAsyncCache = new ConcurrentDictionary<Type, Func<Dispatcher, object, CancellationToken, bool, Task<object>>>();
        }

        private readonly IDiAdapter _diAdapter;

        public Dispatcher(IDiAdapter diAdapter)
        {
            _diAdapter = diAdapter;
        }

        public void Dispatch<TDto>(TDto dto, bool buildDecorator = true)
        {
            var handler = BuildHandler<IHandler<TDto>>(buildDecorator);

            handler.Handle(dto);
        }

        private void DispatchAdapter<TDto>(object dto, bool buildDecorator = true)
        {
            Dispatch((TDto)dto, buildDecorator);
        }

        public void Dispatch(object dto, bool buildDecorator = true)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var dtoType = dto.GetType();

            if (!_dynamicDispatchCache.TryGetValue(dtoType, out Action<Dispatcher, object, bool>? dispatchAction))
            {
                dispatchAction = CreateDispatchAction<Action<Dispatcher, object, bool>>(nameof(DispatchAdapter), dtoType);

                _dynamicDispatchCache.TryAdd(dtoType, dispatchAction);
            }

            dispatchAction(this, dto, buildDecorator);
        }

        public async Task DispatchAsync<TDto>(TDto dto, CancellationToken token = default, bool buildDecorator = true)
        {
            var handler = BuildHandler<IAsyncHandler<TDto>>(buildDecorator);

            await handler.HandleAsync(dto, token);
        }

        private async Task DispatchAsyncAdapter<TDto>(object dto, CancellationToken token = default, bool buildDecorator = true)
        {
            await DispatchAsync((TDto)dto, token, buildDecorator);
        }

        public async Task DispatchAsync(object dto, CancellationToken token = default, bool buildDecorator = true)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var dtoType = dto.GetType();

            if (!_dynamicDispatchAsyncCache.TryGetValue(dtoType, out Func<Dispatcher, object, CancellationToken, bool, Task>? dispatchAction))
            {
                dispatchAction = CreateDispatchAction<Func<Dispatcher, object, CancellationToken, bool, Task>>(nameof(DispatchAsyncAdapter), dtoType);

                _dynamicDispatchAsyncCache.TryAdd(dtoType, dispatchAction);
            }

            await dispatchAction(this, dto, token, buildDecorator);
        }

        public ResultOr<TOut, TError> DispatchResult<TDto, TOut, TError>(TDto dto, bool buildDecorator = true)
            where TDto : IDtoContract<TOut, TError>
            where TError : class
        {
            var handler = BuildHandler<IHandler<TDto, TOut, TError>>(buildDecorator);

            return handler.Handle(dto);
        }

        private object DispatchResultAdapter<TDto, TOut, TError>(object dto, bool buildDecorator = true)
            where TDto : IDtoContract<TOut, TError>
            where TError : class
        {
            return DispatchResult<TDto, TOut, TError>((TDto)dto, buildDecorator);
        }

        public object DispatchResult(object dto, bool buildDecorator = true)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var dtoType = dto.GetType();

            var dtoContract = dtoType.GetInterface(typeof(IDtoContract<,>).Name);

            if (dtoContract is null)
                throw CreateContractExceprion();

            if (!_dynamicDispatchResultCache.TryGetValue(dtoType, out Func<Dispatcher, object, bool, object>? dispatchAction))
            {
                var contractTypes = dtoContract.GetGenericArguments();

                dispatchAction = CreateDispatchAction<Func<Dispatcher, object, bool, object>>(nameof(DispatchResultAdapter), 
                    dtoType, contractTypes[0], contractTypes[1]);

                _dynamicDispatchResultCache.TryAdd(dtoType, dispatchAction);
            }

            return dispatchAction(this, dto, buildDecorator);
        }

        public async Task<ResultOr<TOut, TError>> DispatchResultAsync<TDto, TOut, TError>(TDto dto, CancellationToken token = default, bool buildDecorator = true)
            where TDto : IDtoContract<TOut, TError>
            where TError : class
        {
            var handler = BuildHandler<IAsyncHandler<TDto, TOut, TError>>(buildDecorator);

            return await handler.HandleAsync(dto, token);
        }

        private async Task<object> DispatchResultAsyncAdapter<TDto, TOut, TError>(object dto, CancellationToken token = default, bool buildDecorator = true)
            where TDto : IDtoContract<TOut, TError>
            where TError : class
        {
            return await DispatchResultAsync<TDto, TOut, TError>((TDto)dto, token, buildDecorator);
        }

        public async Task<object> DispatchResultAsync(object dto, CancellationToken token = default, bool buildDecorator = true)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var dtoType = dto.GetType();

            var dtoContract = dtoType.GetInterface(typeof(IDtoContract<,>).Name);

            if (dtoContract is null)
                throw CreateContractExceprion();

            if (!_dynamicDispatchResultAsyncCache.TryGetValue(dtoType, out Func<Dispatcher, object, CancellationToken, bool, Task<object>>? dispatchAction))
            {
                var contractTypes = dtoContract.GetGenericArguments();

                dispatchAction = CreateDispatchAction<Func<Dispatcher, object, CancellationToken, bool, Task<object>>>(nameof(DispatchResultAsyncAdapter),
                    dtoType, contractTypes[0], contractTypes[1]);

                _dynamicDispatchResultAsyncCache.TryAdd(dtoType, dispatchAction);
            }

            return await dispatchAction(this, dto, token, buildDecorator);
        }

        private static DispatchException CreateContractExceprion()
        {
            return new DispatchException($"The input dto does not implement the {typeof(IDtoContract<,>).Name} interface.");
        }

        private static TAction CreateDispatchAction<TAction>(string adapterMethodName, params Type[] genericTypes)
            where TAction : Delegate
        {
            var method = typeof(Dispatcher)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(x => x.Name == adapterMethodName && x.IsGenericMethod);

            method = method.MakeGenericMethod(genericTypes);
            return (TAction)Delegate.CreateDelegate(typeof(TAction), method);
        }

        private THandler BuildHandler<THandler>(bool buildDecorator)
            where THandler : class
        {
            var afterDecorators = StaticKeyHashSetCache<THandler, DecorateAfterPipeLine>.Values;
            var beforeDecoratots = StaticKeyHashSetCache<THandler, DecorateBeforePipeLine>.Values;

            var handler = _diAdapter.Create<THandler>();

            if (handler is null)
                throw new DispatchException($"Handler {typeof(THandler).FullName} failed to create.");

            if (!buildDecorator)
                return handler;

            if (!afterDecorators.Any() && !beforeDecoratots.Any())
            {
                var handlerType = handler.GetType();
                var key = new StringInfoTypeDto(handlerType.Name, handlerType.Namespace);

                StaticDictionaryHashSetCache<StringInfoTypeDto, DecorateAfterPipeLine>.TryGet(key, out ReadOnlyCollection<DecorateAfterPipeLine> dynamicAfterDecorators);
                StaticDictionaryHashSetCache<StringInfoTypeDto, DecorateBeforePipeLine>.TryGet(key, out ReadOnlyCollection<DecorateBeforePipeLine> dynamicBeforeDecoratots);

                if (dynamicAfterDecorators.Any() || dynamicBeforeDecoratots.Any())
                {
                    InitDynamicDecorators<THandler, DecorateAfterPipeLine>(dynamicAfterDecorators);
                    InitDynamicDecorators<THandler, DecorateBeforePipeLine>(dynamicBeforeDecoratots);

                    afterDecorators = StaticKeyHashSetCache<THandler, DecorateAfterPipeLine>.Values;
                    beforeDecoratots = StaticKeyHashSetCache<THandler, DecorateBeforePipeLine>.Values;
                }
            }

            handler = BuildPipeLine(handler, true, afterDecorators.OrderBy(x => x.Order));
            handler = BuildPipeLine(handler, false, beforeDecoratots.OrderByDescending(x => x.Order));

            return handler;
        }

        private static void InitDynamicDecorators<THandler, TPipe>(IEnumerable<DecoratePipeLine> pipeLineInfos)
            where TPipe : DecoratePipeLine, new()
        {
            var tHandlerType = typeof(THandler);

            var typedDecorate = pipeLineInfos
                .Select(decInf =>
                {
                    var decoratorHandlerType = decInf.DecoratorType
                        .GetInterfaces()
                        .First(x => x.Name == tHandlerType.Name && x.Namespace == tHandlerType.Namespace);

                    var decoratorType = decInf.DecoratorType;

                    if (decInf.DecoratorType.IsGenericTypeDefinition)
                    {
                        var dynamicGenericTypes = decInf.DecoratorType
                            .GetGenericArguments()
                            .Select(x => tHandlerType.GenericTypeArguments[Array.IndexOf(decoratorHandlerType.GenericTypeArguments, x)])
                            .ToArray();

                        decoratorType = decInf.DecoratorType.MakeGenericType(dynamicGenericTypes);
                    }

                    return new TPipe()
                    {
                        DecoratorType = decoratorType,
                        Order = decInf.Order
                    };
                })
                .ToList();

            StaticKeyHashSetCache<THandler, TPipe>.AddRange(typedDecorate);
        }

        private THandler BuildPipeLine<THandler>(THandler handler, bool isAfter, IEnumerable<DecoratePipeLine> pipeLineInfos)
        {
            foreach (var decoratorInfo in pipeLineInfos)
            {
                var decorator = (IHandlerDecorator<THandler>?)_diAdapter.Create(decoratorInfo.DecoratorType);

                if (decorator is null)
                    throw new DispatchException($"Decorator {decoratorInfo.DecoratorType.FullName} failed to create.");

                decorator.IsAfter = isAfter;
                decorator.DecorateHandler = handler;

                handler = (THandler)decorator;
            }

            return handler;
        }
    }
}
