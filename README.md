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
