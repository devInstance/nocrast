using DevInstance.LogScope;
using NoCrast.Shared.Model;
using System;
using System.Threading.Tasks;

namespace NoCrast.Client.Pages.Reports
{
    public partial class AggregateReportsIndex
    {
        private IScopeLog log;

        private ReportItem.RIType selectedType = ReportItem.RIType.Unknown;

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
                    case ReportItem.RIType.Yearly:
                        return "yyy";
                    case ReportItem.RIType.Daily:
                    default:
                        return "MMM d yyy";
                }
            }
        }

        private string selectedPeriod = "";

        private ReportItem Report { get; set; }

        protected async override Task OnInitializedAsync()
        {
            log = ScopeManager.CreateLogger(this);
            try
            {
                selectedDate = TimeProvider.CurrentTime;

                using (var scope = log.TraceScope())
                {
                    RequestReportAsync(ReportItem.RIType.Daily);
                }
            }
            catch (Exception ex)
            {
                log.E(ex);
            }
        }

        public async void RequestReportAsync(ReportToolbar.FilterChangedArgs args)
        {
            selectedDate = args.Date;
            RequestReportAsync(args.Type);
        }

        public async void RequestReportAsync(ReportItem.RIType type)
        {
            Report = null;
            selectedType = type;
            Report = await Service.GetAggregateReportAsync(type, selectedDate);
            if (Report != null)
            {
                startDate = Report.StartDate;
                endDate = Report.EndDate;
                selectedPeriod = $"{startDate.ToString("MMM d yyy")} - {endDate.ToString("MMM d yyy")}";
            }

            StateHasChanged();
        }
    }
}
