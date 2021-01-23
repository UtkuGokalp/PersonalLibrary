namespace Utility.Development
{
    public delegate void TypeSafeEventHandler<SenderType, EventArgsType>(SenderType sender, EventArgsType e);
}