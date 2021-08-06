# StocksApp
StocksApp. 

Sorry i did'nt write unit tests(did not have enough time), but i know that every program should be covered by unit tests more than possible(ideally 100%)

Technologies used:
1) Newtonsoft.Json for json serializing/deserializing
2) CsvHelper for csv serializing/deserializing
3) Razor for html generation from template
4) SelectPdf for pdf generation from html
5) Microsoft.Extensions.Configuration for reading configs from appsettings.local.json file
6) Microsoft.Extensions.DependencyInjection as a DI container
7) NET. 5 and all used nuget packages are compatible with NET. 5

This application downloads historical data from different stock API's by certain filters:
1) Ticker name
2) Date interval
3) Start Date
4) End Date
5) API select

And then suggests to save obtained historical data to pdf file in user selected folder

Solution structure:
  Projects:
    
    1) StocksApp - windows form application. As a backend developer, i dont have big experience of developing desktop applications with right architecture, but i tried to store all logics(except html templates) it in single project
      1.1) There is some not right sides of architecture, there are some business logic in Form, adding Use Cases layer may help to reach better architecture to unload logics from Forms
    2) StocksApp.Templates - razor class library to contain cshtml html templates

I implemented clients for 2 stocks API - Yahoo Finance and Tiingo, both of them implements IStocksApiClient interface, but there are some violation of Liskov substitution principle by different behavior in date interval choosing, because both of API's have different variations of date intervals, but i tried to use same interface for both of them

1) Yahoo finance - 1m, 2m, 5m, 15m, 30m, 60m, 90m, 1h, 1d, 5d, 1wk, 1mo, 3mo, **Note: 1m, 2m, 5m, 15m, 30m, 60m, 90m, 1h** intervals not works and API returns "404 Not Found: AdjClose object not present in indicators", so i deleted them, and final list of intervals is **1d, 5d, 1wk, 1mo, 3mo**
2) Tiingo - daily, weekly, monthly, annually

You can choose certain client in last combobox in GUI
