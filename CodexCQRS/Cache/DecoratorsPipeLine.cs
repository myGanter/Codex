using CodexCQRS.CQRS;
using CodexCQRS.Dtos;
using CodexCQRS.Exceptions;

namespace CodexCQRS.Cache
{
    public static class DecoratorsPipeLine
    {
        public static DecorateInitPipeLine<IHandler<TDto>> FromHandler<TDto>()
        {
            CheckExistingPipeline<IHandler<TDto>>();

            return new DecorateInitPipeLine<IHandler<TDto>>();
        }

        public static DecorateInitPipeLine<IAsyncHandler<TDto>> FromAsyncHandler<TDto>()
        {
            CheckExistingPipeline<IAsyncHandler<TDto>>();

            return new DecorateInitPipeLine<IAsyncHandler<TDto>>();
        }

        public static DecorateInitPipeLine<IHandler<TDto, TOut, TError>> FromHandler<TDto, TOut, TError>()
            where TError : class
            where TDto : IDtoContract<TOut, TError>
        {
            CheckExistingPipeline<IHandler<TDto, TOut, TError>>();

            return new DecorateInitPipeLine<IHandler<TDto, TOut, TError>>();
        }

        public static DecorateInitPipeLine<IAsyncHandler<TDto, TOut, TError>> FromAsyncHandler<TDto, TOut, TError>()
            where TError : class
            where TDto : IDtoContract<TOut, TError>
        {
            CheckExistingPipeline<IAsyncHandler<TDto, TOut, TError>>();

            return new DecorateInitPipeLine<IAsyncHandler<TDto, TOut, TError>>();
        }

        private static void CheckExistingPipeline<THandler>()
        {
            if (StaticKeyHashSetCache<THandler, DecorateAfterPipeLine>.Values.Count > 0 ||
                StaticKeyHashSetCache<THandler, DecorateBeforePipeLine>.Values.Count > 0)
                throw new DecorateInitException($"There is already a pipeline for the {typeof(THandler).FullName} handler.");
        }

        public static DecorateInitPipeLine<THandler> Before<THandler, TDecorator>(this DecorateInitPipeLine<THandler> pipeLine)
            where TDecorator : THandler, IHandlerDecorator<THandler>
        {
            AddDecoratorToCache<THandler, TDecorator, DecorateBeforePipeLine>(pipeLine, x => x.BeforeCounter++);

            return pipeLine;
        }

        public static DecorateInitPipeLine<THandler> After<THandler, TDecorator>(this DecorateInitPipeLine<THandler> pipeLine)
            where TDecorator : THandler, IHandlerDecorator<THandler>
        {
            AddDecoratorToCache<THandler, TDecorator, DecorateAfterPipeLine>(pipeLine, x => x.AfterCounter++);

            return pipeLine;
        }

        private static void AddDecoratorToCache<THandler, TDecorator, TPipe>(DecorateInitPipeLine<THandler> pipeLine, Func<DecorateInitPipeLine<THandler>, int> getOrder)
            where TPipe : DecoratePipeLine, new()
        {
            if (pipeLine is null)
                throw new ArgumentNullException(nameof(pipeLine));

            var decoratePipeLine = new TPipe()
            {
                DecoratorType = typeof(TDecorator),
                Order = getOrder(pipeLine)
            };

            StaticKeyHashSetCache<THandler, TPipe>.Add(decoratePipeLine);
        }

        public static DecorateInitPipeLine FromHandlerType(Type handlerType)
        {
            var iHandlerType = ValidateAndGetHandlerInterfaceType(handlerType);

            return new DecorateInitPipeLine(handlerType, iHandlerType);
        }

        private static Type ValidateAndGetHandlerInterfaceType(Type handlerType)
        {
            if (handlerType is null)
                throw new ArgumentNullException(nameof(handlerType));

            var stringTypeInfo = new StringInfoTypeDto(handlerType.Name, handlerType.Namespace);

            if (StaticDictionaryHashSetCache<StringInfoTypeDto, DecorateAfterPipeLine>.ContainsKey(stringTypeInfo) ||
                StaticDictionaryHashSetCache<StringInfoTypeDto, DecorateBeforePipeLine>.ContainsKey(stringTypeInfo))
                throw new DecorateInitException($"There is already a pipeline for the {handlerType.FullName} handler.");

            var iHandlerType = typeof(IHandler<>);
            var iAsyncHandlerType = typeof(IAsyncHandler<>);
            var iResultHandlerType = typeof(IHandler<,,>);
            var iResultAsyncHandlerType = typeof(IAsyncHandler<,,>);            

            if (handlerType.IsInterface)
                throw new DecorateInitException("The handler type should be an implementation, not an interface.");

            if (handlerType.IsGenericType && !handlerType.IsGenericTypeDefinition)
                throw new DecorateInitException("The handler type must not have generic types initialized.");

            var interfaces = handlerType.GetInterfaces();

            Type? res = interfaces.FirstOrDefault(x => (x.Name == iResultAsyncHandlerType.Name && x.Namespace == iResultAsyncHandlerType.Namespace) ||
                (x.Name == iResultHandlerType.Name && x.Namespace == iResultHandlerType.Namespace) ||
                (x.Name == iAsyncHandlerType.Name && x.Namespace == iAsyncHandlerType.Namespace) ||
                (x.Name == iHandlerType.Name && x.Namespace == iHandlerType.Namespace));

            if (res is not null)
                return res;

            throw new DecorateInitException("The passed type does not implement any of the handler interfaces.");
        }

        public static DecorateInitPipeLine Before(this DecorateInitPipeLine pipeLine, Type decoratorType)
        {
            AddDecoratorToCache<DecorateBeforePipeLine>(pipeLine, decoratorType, x => x.BeforeCounter++);

            return pipeLine;
        }

        public static DecorateInitPipeLine After(this DecorateInitPipeLine pipeLine, Type decoratorType)
        {
            AddDecoratorToCache<DecorateAfterPipeLine>(pipeLine, decoratorType, x => x.AfterCounter++);

            return pipeLine;
        }

        private static void AddDecoratorToCache<TPipe>(DecorateInitPipeLine pipeLine, Type decoratorType, Func<DecorateInitPipeLine, int> getOrder)
            where TPipe : DecoratePipeLine, new()
        {
            if (decoratorType is null)
                throw new ArgumentNullException(nameof(decoratorType));

            if (decoratorType.IsInterface)
                throw new DecorateInitException("The decorator type should be an implementation, not an interface.");

            var decoratorInterfaces = decoratorType.GetInterfaces();
            var handlerInterface = decoratorInterfaces.FirstOrDefault(x => x.Name == pipeLine.InterfaceHandlerType.Name && x.Namespace == pipeLine.InterfaceHandlerType.Namespace);

            if (handlerInterface is null)
                throw new DecorateInitException("The decorator type and the handler type must implement the same handler interface.");

            var searchType = typeof(IHandlerDecorator<>).MakeGenericType(handlerInterface);
            var decoratorHandlerInterface = decoratorInterfaces.FirstOrDefault(x => x == searchType);

            if (decoratorHandlerInterface is null)
                throw new DecorateInitException($"The decorator type must implement {searchType.FullName}.");

            if (!decoratorType.GetGenericArguments().All(x => handlerInterface.GenericTypeArguments.Contains(x)))
                throw new DecorateInitException("Decorator type has unsupported generic arguments.");

            for (var i = 0; i < handlerInterface.GenericTypeArguments.Length; ++i)
            {
                if (!handlerInterface.GenericTypeArguments[i].IsGenericParameter &&
                    handlerInterface.GenericTypeArguments[i] != pipeLine.InterfaceHandlerType.GenericTypeArguments[i])
                {
                    throw new DecorateInitException("Decorator type cannot be converted to configured handler type.");
                }
            }

            var decoratePipeLine = new TPipe()
            {
                DecoratorType = decoratorType,
                Order = getOrder(pipeLine)
            };

            StaticDictionaryHashSetCache<StringInfoTypeDto, TPipe>.Add(new StringInfoTypeDto(pipeLine.HandlerType.Name, pipeLine.HandlerType.Namespace), decoratePipeLine);
        }
    }
}
