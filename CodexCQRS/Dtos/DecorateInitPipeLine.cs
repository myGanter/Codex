namespace CodexCQRS.Dtos
{
    public sealed class DecorateInitPipeLine<THandler>
    {
        internal DecorateInitPipeLine() { }

        internal int BeforeCounter { get; set; }

        internal int AfterCounter { get; set; }
    }

    public sealed class DecorateInitPipeLine
    {
        internal DecorateInitPipeLine(Type handlerType, Type handlerKeyTypeForCache, Type interfaceHandlerType) 
        {
            HandlerType = handlerType;
            InterfaceHandlerType = interfaceHandlerType;
            HandlerKeyTypeForCache = handlerKeyTypeForCache;
        }

        internal Type HandlerType { get; set; }

        internal Type HandlerKeyTypeForCache { get; set; }

        internal Type InterfaceHandlerType { get; set; }

        internal int BeforeCounter { get; set; }

        internal int AfterCounter { get; set; }
    }
}
