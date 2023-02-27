using System;

namespace Jump.Unlocks
{
    public class GenericUnlockable : UnlockableBase
    {
        public override string FormattedDescription => String.Format(Tr(description), Formats);
    }
}