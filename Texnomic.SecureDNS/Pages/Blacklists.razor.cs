using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Data.Models;
using Texnomic.SecureDNS.Services;

namespace Texnomic.SecureDNS.Pages;

public class BlacklistsBase : ComponentBase
{
    [Inject]
    protected BlacklistsService BlacklistsService { get; set; }

    [Inject]
    protected DatabaseContext DatabaseContext { get; set; }

    protected string Output;

    protected SfGrid<Blacklist> DefaultGrid;


    public void ExcelExport()
    {
        DefaultGrid.ExcelExport();
    }

    public void PdfExport()
    {
        var ExportProperties = new PdfExportProperties();
        var Theme = new PdfTheme();

        var HeaderBorder = new PdfBorder
        {
            Color = "#64FA50"
        };

        var HeaderThemeStyle = new PdfThemeStyle()
        {
            FontColor = "#64FA50",
            FontName = "Calibri",
            FontSize = 17,
            Bold = true,
            Border = HeaderBorder
        };

        Theme.Header = HeaderThemeStyle;

        var RecordThemeStyle = new PdfThemeStyle()
        {
            FontColor = "#64FA50",
            FontName = "Calibri",
            FontSize = 17

        };
        Theme.Record = RecordThemeStyle;

        var CaptionThemeStyle = new PdfThemeStyle()
        {
            FontColor = "#64FA50",
            FontName = "Calibri",
            FontSize = 17,
            Bold = true

        };

        Theme.Caption = CaptionThemeStyle;

        ExportProperties.Theme = Theme;

        DefaultGrid.PdfExport(ExportProperties);
    }


    protected async Task InitializeAsync()
    {
        await BlacklistsService.InitializeAsync();

        Output = "Blacklists Initialized Successfully.";
    }
}