using System.Collections.Generic;

namespace Stellamod.Common.QuestSystem
{
    public class QuestComparer : IComparer<Quest>
    {
        public int Compare(Quest x, Quest y)
        {
            return x.DisplayName.CompareTo(y.DisplayName);
        }
    }
}
