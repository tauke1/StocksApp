# StocksApp
StocksApp

This application downloads historical data from different stock API's by certain filters:
1) Ticker name
2) Date interval
3) Start Date
4) End Date
5) API select

And then suggests to save obtained historical data to pdf file in user selected folder

Solution structure:
  Projects:
    
    1) StocksApp - windows form application. As a backend developer, i dont have big experience of developing desktop applications with right architecture, but i tried to divide it in single project
      1.1) There is some not right sides of architecture, there are some business logic in Form, maybe add some Use Cases would be better
    2) StocksApp.Templates - razor class library to contain cshtml html templates

I implemented clients for 2 stocks API - Yahoo Finance and Tiingo, both of them implements IStocksApiClient interface, but there are some violation of Liskov substitution principle by different behavior in date interval choosing, because both of API's have different variations of date intervals, but i tried to use same interface for both of them

1) Yahoo finance - 1m, 2m, 5m, 15m, 30m, 60m, 90m, 1h, 1d, 5d, 1wk, 1mo, 3mo
2) Tiingo - daily, weekly, monthly, annually

You can choose certain client in last combobox in GUI
