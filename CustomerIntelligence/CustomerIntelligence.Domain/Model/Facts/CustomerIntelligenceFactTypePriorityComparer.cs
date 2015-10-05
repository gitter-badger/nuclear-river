using System;
using System.Collections.Generic;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class CustomerIntelligenceFactTypePriorityComparer : IComparer<Type>
    {
        // ѕриоритетом сейчас выдел€ем только корни агрегации
        // —читаем, что в одном TUC они должны быть обработаны раньше, чем прив€занные к ним сущности и valueobject'ы
        // Ќаиболее приоретными сделаны справочные агрегаты, которые ни от чего не завис€т.
        // “ерритори€ тоже €вл€етс€ справочным агрегатом, но зависит от проекта, поэтому приоритет ниже.
        private static readonly Dictionary<Type, int> Priority
            = new Dictionary<Type, int>
              {
                  { typeof(Territory), 2 },
                  { typeof(Category), 3 },
                  { typeof(CategoryGroup), 3 },
                  { typeof(Project), 3 },
                  { typeof(Firm), 1 },
                  { typeof(Client), 1 },
              };

        public int Compare(Type x, Type y)
        {
            int priorityX;
            if (!Priority.TryGetValue(x, out priorityX))
            {
                priorityX = 0;
            }

            int priorityY;
            if (!Priority.TryGetValue(y, out priorityY))
            {
                priorityY = 0;
            }

            return priorityX - priorityY;
        }
    }
}