using DevInstance.LogScope;
using DevInstance.Timelines;
using NoCrast.Shared.Model;
using System;
using System.Threading.Tasks;

namespace NoCrast.Client.Pages.Reports
{
    public partial class ActivityReportsIndex
    {
        private IScopeLog log;

        private ReportItem.RIType selectedType = ReportItem.RIType.Total;
        private async void UpdateTypeAsync(ReportItem.RIType newValue)
        {
            selectedType = newValue;
            await RequestReportAsync();
        }

        private ReportItem.RIMode selectedMode = ReportItem.RIMode.Combined;
        private async void UpdateModeAsync(ReportItem.RIMode newValue)
        {
            selectedMode = newValue;
            await RequestReportAsync();
        }

        private DateTime selectedDate;
        private DateTime startDate;
        private DateTime endDate;

        private string InputFormat
        {
            get
            {
                switch (selectedType)
                {
                    case ReportItem.RIType.Weekly:
                        return "MMM d yyy";
                    case ReportItem.RIType.Monthly:
                        return "MMM yyy";
                    case ReportItem.RIType.Yearly:
                        return "yyy";
                    default:
                        return "MMM d yyy";
                }
            }
        }

        private string SelectedPeriod
        {
            get
            {
                switch (selectedType)
                {
                    case ReportItem.RIType.Weekly:
                        return $"{startDate.ToString("MMM d yyy")} - {endDate.ToString("MMM d yyy")}";
                    case ReportItem.RIType.Monthly:
                        return $"{startDate.ToString("MMM yyy")}";
                    case ReportItem.RIType.Yearly:
                        return $"{startDate.ToString("yyy")}";
                    default:
                        return null;
                }
            }
        }

        private ReportItem Report { get; set; }

        private Heatline.Line[] Lines = null;

        protected async override Task OnInitializedAsync()
        {
            log = ScopeManager.CreateLogger(this);
            try
            {
                selectedDate = TimeProvider.CurrentTime;
                selectedType = ReportItem.RIType.Total;
                selectedMode = ReportItem.RIMode.Combined;
                using (var scope = log.TraceScope("loading report"))
                {
                    await RequestReportAsync();
                }
            }
            catch (Exception ex)
            {
                log.E(ex);
            }
        }

        public async Task<bool> RequestReportAsync()
        {
            Report = null;
            Report = await Service.GetActivityReportAsync(selectedType, selectedMode, selectedDate);

            Lines = new Heatline.Line[Report.Rows.Length];
            for (int n = 0; n < Lines.Length; n++)
            {
                var data = Report.Rows[n].Data;
                var items = new Heatline.Item[data.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    var item = new Heatline.Item
                    {
                        Time = Report.Columns[i],
                        Value = data[i].Value,
                        Description = data[i].Details
                    };
                    items[i] = item;
                }

                Lines[n] = new Heatline.Line
                {
                    CssClass = "blue",
                    Items = items
                };
            }

            startDate = Report.StartDate;
            endDate = Report.EndDate;

            StateHasChanged();

            return true;
        }

        private DateTime UpdateStep(DateTime aDate, int sign)
        {
            switch (selectedType)
            {
                case ReportItem.RIType.Weekly:
                    return aDate.AddDays(7 * sign);
                case ReportItem.RIType.Monthly:
                    return aDate.AddDays(7 * 5 * sign);
                case ReportItem.RIType.Yearly:
                    return aDate.AddYears(1 * sign);
                default:
                    return aDate;
            }
        }

        public async void PrevAsync()
        {
            selectedDate = UpdateStep(selectedDate, -1);
            await RequestReportAsync();
        }

        public async void NextAsync()
        {
            selectedDate = UpdateStep(selectedDate, 1);
            await RequestReportAsync();
        }

        public async void TodayAsync()
        {
            selectedDate = TimeProvider.CurrentTime;
            await RequestReportAsync();
        }
    }
}
