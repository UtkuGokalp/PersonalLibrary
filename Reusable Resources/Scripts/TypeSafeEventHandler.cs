using System;

namespace Utility.Development
{
    public delegate void TypeSafeEventHandler<SenderType, EventArgsType>(SenderType sender, EventArgsType e) where EventArgsType : EventArgs;
}