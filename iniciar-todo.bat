@echo off
echo Levantando los 5 microservicios...
start "Products.API - 5001" /D src\Products.API cmd /k dotnet run --launch-profile http
start "Users.API - 5002" /D src\Users.API cmd /k dotnet run --launch-profile http
start "Cart.API - 5003" /D src\Cart.API cmd /k dotnet run --launch-profile http
start "Orders.API - 5004" /D src\Orders.API cmd /k dotnet run --launch-profile http
start "Notifications.API - 5005" /D src\Notifications.API cmd /k dotnet run --launch-profile http
echo Esperando que arranquen...
timeout /t 20 /nobreak
start http://localhost:5001/swagger
start http://localhost:5002/swagger
start http://localhost:5003/swagger
start http://localhost:5004/swagger
start http://localhost:5005/swagger
