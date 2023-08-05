using KiwiPortal.Models;
using KiwiPortal.Services;
using Microsoft.AspNetCore.Components;
using PSC.Blazor.Components.Chartjs.Models.Common;
using PSC.Blazor.Components.Chartjs.Models.Line;
using PSC.Blazor.Components.Chartjs.Util;
using ConfigurationProvider = KiwiPortal.Services.ConfigurationProvider;

namespace KiwiPortal.Pages;

public partial class Calculator
{
    [Inject]
    public ConfigurationProvider ConfigurationProvider { get; set; } = null!;

    [Inject]
    public EnergyConsumptionService EnergyConsumptionService { get; set; } = null!;

    [Inject]
    public AwattarService AwattarService { get; set; } = null!;

    private double _gridTaxCent;

    private double _awattarCommissionPercent;
    
    private double _awattarCommissionCent;
    
    private double _awattarBaseFeeEuro;

    private double _valueAddedTaxPercent;

    private double _powerFlatRateEuro;

    private double _measurementFeeEuro;

    private DateTime _selectedDate = DateTime.Today;

    private DateTime _selectedSecondDate = DateTime.Today;

    private LineChartConfig _lineChartConfig = new();

    private List<ConsumptionData> _consumptionData = new();

    private List<AwattarPriceData> _priceData = new();

    private bool _loading;

    protected override async Task OnInitializedAsync()
    {
        var portalConfiguartion = ConfigurationProvider.CurrentConfiguration;

        _gridTaxCent = portalConfiguartion.GridTaxCent;
        _awattarCommissionPercent = portalConfiguartion.AwattarCommissionPercent;
        _awattarCommissionCent = portalConfiguartion.AwattarCommissionCent;
        _awattarBaseFeeEuro = portalConfiguartion.AwattarBaseFeeEuro;
        _valueAddedTaxPercent = portalConfiguartion.ValueAddedTaxPercent;
        _powerFlatRateEuro = portalConfiguartion.PowerFlatRateEuro;
        _measurementFeeEuro = portalConfiguartion.MeasurementFeeEuro;

        await UpdateChart();

        await base.OnInitializedAsync();
    }

    private async Task UpdateChart()
    {
        _loading = true;

        await InvokeAsync(StateHasChanged);

        var date = DateOnly.FromDateTime(_selectedDate);
        var secondDate = DateOnly.FromDateTime(_selectedSecondDate);
        var consumptionData = await EnergyConsumptionService.GetConsumptionData(date, secondDate.AddDays(1));

        _consumptionData = consumptionData?.FirstOrDefault()?.Data
            .Select(x =>
            {
                x.StartTimestamp = new DateTimeOffset(x.StartTimestamp.Year, x.StartTimestamp.Month,
                    x.StartTimestamp.Day, x.StartTimestamp.Hour, 0, 0, 0, TimeSpan.Zero);

                return x;
            })
            .GroupBy(x => x.StartTimestamp)
            .Select(x =>
            {
                var totalConsumption = x.ToList().Sum(y => y.Value);

                return new ConsumptionData
                {
                    Value = totalConsumption,
                    StartTimestamp = x.Key,
                    EndTimestamp = x.Key,
                };
            })
            .ToList() ?? new List<ConsumptionData>();

        var priceData = await AwattarService.GetPriceData(date, secondDate);

        _priceData = priceData?.Data.ToList() ?? new List<AwattarPriceData>();

        _lineChartConfig = new LineChartConfig
        {
            Options = new Options
            {
                Plugins = new Plugins
                {
                    Legend = new Legend
                    {
                        Align = Align.Center,
                        Display = false,
                        Position = LegendPosition.Right,
                    },
                },
                Scales = new Dictionary<string, Axis>
                {
                    {
                        Scales.XAxisId, new Axis
                        {
                            Stacked = true,
                            Ticks = new Ticks
                            {
                                MaxRotation = 0,
                                MinRotation = 0,
                            },
                        }
                    },
                    {
                        Scales.YAxisId, new Axis
                        {
                            Stacked = true,
                        }
                    },
                    {
                        Scales.Y2AxisId, new Axis
                        {
                            Stacked = true,
                            Position = Position.Right,
                        }
                    },
                },
            },
            Data =
            {
                Labels = _consumptionData.Select(x => $"{x.StartTimestamp.ToLocalTime():g}").ToList(),
                Datasets = new List<LineDataset>
                {
                    new()
                    {
                        Label = "Preis [kWh]",
                        Data = _priceData.Select(y => (Convert.ToDecimal(y.MarketPrice) / 10)).ToList(),
                        BorderWidth = 1,
                        YAxisId = Scales.YAxisId,
                        BorderColor = ColorUtil.ColorHexString(37, 90, 138),
                        PointRadius = 0,
                    },
                    new()
                    {
                        Label = "Verbrauch",
                        Data = _consumptionData.Select(y => Convert.ToDecimal(y.Value)).ToList(),
                        BorderWidth = 1,
                        YAxisId = Scales.Y2AxisId,
                        Fill = true,
                        BorderColor = ColorUtil.ColorHexString(252, 193, 0),
                        BackgroundColor = ColorUtil.ColorHexString(252, 193, 0),
                        FillColor = ColorUtil.ColorString(252, 193, 0, 0.1),
                        PointRadius = 0,
                    },
                },
            },
        };

        _loading = false;
    }

    private async Task OnDateChanged(ChangeEventArgs e)
    {
        var dateString = e.Value?.ToString();

        if (dateString is null)
        {
            return;
        }

        var selectedDate = DateTime.Parse(dateString);

        if (selectedDate > _selectedSecondDate)
        {
            return;
        }

        _selectedDate = selectedDate;

        await UpdateChart();
    }

    private async Task OnSecondDateChanged(ChangeEventArgs e)
    {
        var dateString = e.Value?.ToString();

        if (dateString is null)
        {
            return;
        }

        var selectedSecondDate = DateTime.Parse(dateString);

        if (selectedSecondDate < _selectedDate || selectedSecondDate > DateTime.Today)
        {
            return;
        }

        _selectedSecondDate = selectedSecondDate;

        await UpdateChart();
    }

    private double NetEnergyPriceSumEuro =>
        CentToEuro(
            _consumptionData.Select((t, i) =>
                t.Value *
                (AddPercent(GetCentPerKiloWattHourPrice(_priceData[i].MarketPrice), _awattarCommissionPercent) +
                 _awattarCommissionCent)
            ).Sum()
        );
    
    private double TotalConsumptionKwh => _consumptionData.Sum(x => x.Value);

    private double AverageNetPriceCent => EuroToCent(NetEnergyPriceSumEuro / TotalConsumptionKwh);

    private double AverageNetPriceIncludingAwattarCommissionCent => AddPercent(AverageNetPriceCent, _awattarCommissionPercent);

    private double AverageNetAllInPriceCent => AverageNetPriceIncludingAwattarCommissionCent + _gridTaxCent;

    private double AverageGrossAllInPriceCent => AddPercent(AverageNetAllInPriceCent, _valueAddedTaxPercent);

    private double TotalNetGridTaxEuro => _consumptionData.Sum(x => x.Value * NetGridTaxEuro);

    private double NetGridTaxEuro => CentToEuro(_gridTaxCent);
    
    private double TotalNetSumEuro => NetEnergyPriceSumEuro + TotalNetGridTaxEuro + NetFlatRateEuro + NetMeasurementFeeEuro + _awattarBaseFeeEuro;

    private double TotalGrossSumEuro => AddPercent(TotalNetSumEuro, _valueAddedTaxPercent);

    private double NetFlatRateEuro => GetPricePerDay(_powerFlatRateEuro);

    private double NetMeasurementFeeEuro => GetPricePerDay(_measurementFeeEuro);

    private double TotalDays => _selectedSecondDate.Subtract(_selectedDate).TotalDays + 1;
    
    private const int TotalDaysInYear = 365; // this will result in slightly wrong results for leap years
    
    private double GetCentPerKiloWattHourPrice(double euroPerMegaWattHourPrice)
    {
        return euroPerMegaWattHourPrice / 10;
    }
    
    private double GetPricePerDay(double value)
    {
        return (TotalDays * (value / TotalDaysInYear));
    }
    
    private double AddPercent(double value, double percent)
    {
        return (value * (1 + (1.0 / 100 * percent)));
    }

    private double CentToEuro(double value)
    {
        return value / 100;
    }
    
    private double EuroToCent(double value)
    {
        return value * 100;
    }
}
