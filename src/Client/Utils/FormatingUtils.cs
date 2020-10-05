using NoCrast.Client.ModelExtensions;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Utils
{
    public static class FormatingUtils
    {
        private const int TruncationLimit = 8;

        public static string FormatTimerElapsedTime(TaskItem item, ITimeProvider provider)
        {
            return String.Format("{0:hh}:{0:mm}:{0:ss}", item.GetElapsedTimeSpan(provider));
        }

        public static string GetTimerTitle(TaskItem item, ITimeProvider provider)
        {
            var pageName = item.Title;
            if (pageName.Length > TruncationLimit)
            {
                pageName = pageName.Substring(0, TruncationLimit - 1) + "...";
            }
            return $"{pageName} {FormatTimerElapsedTime(item, provider)}";
        }

    }
}
