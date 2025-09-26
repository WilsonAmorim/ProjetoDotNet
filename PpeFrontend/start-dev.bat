@echo off
REM Aguarda 2 segundos para garantir que o servidor vai iniciar
timeout /t 2 >nul

REM Abre o navegador na porta padrão do Blazor (ajuste se necessário)
start http://localhost:5239

REM Inicia com Hot Reload
dotnet watch run
