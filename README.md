# 📈 Evaluador de Acciones con Inteligencia Artificial

Una aplicación web Full-Stack desarrollada en **C# y .NET 8** diseñada para el análisis técnico y predictivo del mercado de valores. Utiliza modelos de Machine Learning para pronosticar precios, detectar anomalías estadísticas y evaluar la estabilidad global del mercado en tiempo real.

## Características Principales

* **Pronóstico de Series Temporales (Forecasting):** Proyecta el precio de cierre a 1 semana, 15 días o 1 mes hacia el futuro utilizando el algoritmo SSA (*Singular Spectrum Analysis*) de **ML.NET**.
* **Detección de Anomalías:** Escanea el historial de precios con algoritmos de Machine Learning (`DetectIidSpike`) para alertar sobre comportamientos irracionales o picos estadísticos inusuales en el mercado.
* **Termómetro de Estabilidad Global:** Se conecta en tiempo real al índice VIX (Índice del Miedo de Chicago) para evaluar el sentimiento macroeconómico y el nivel de pánico o confianza en los mercados mundiales.
* **Indicadores Técnicos (Trading):** Calcula y grafica automáticamente la Media Móvil Simple de 50 días (SMA50) y el Índice de Fuerza Relativa de 14 días (RSI).
* **Data Pipeline Automatizado:** Extrae el histórico de 1 año (365 días) de forma nativa consumiendo la API de Yahoo Finance, soportando tickers de la Bolsa Mexicana de Valores (BMV) y mercados globales (SIC/Wall Street).
* **Dashboard Interactivo:** Interfaz gráfica limpia y responsiva utilizando **Chart.js** con múltiples ejes Y para visualizar precios, predicciones e indicadores técnicos de forma simultánea.
* **Preparado para Tiempo Real:** Infraestructura base de **SignalR** (WebSockets) configurada para recibir actualizaciones de precios en vivo (*push notifications*).

## Stack Tecnológico

* **Backend:** C#, ASP.NET Core MVC (.NET 8)
* **Inteligencia Artificial:** Microsoft.ML, Microsoft.ML.TimeSeries, Microsoft.ML.FastTree
* **Ingesta de Datos:** HttpClient, Newtonsoft.Json (Yahoo Finance API)
* **Frontend:** HTML5, Bootstrap 5, Chart.js
* **Telemetría / Tiempo Real:** SignalR

## Instalación y Ejecución

1. Clona este repositorio en tu máquina local.
2. Abre la solución (`ProyectoBolsaML.sln`) en Visual Studio 2022.
3. Restaura los paquetes NuGet si no se restauran automáticamente.
4. Presiona `F5` o haz clic en "Iniciar" para compilar y ejecutar el proyecto en tu navegador.
5. Ingresa un ticker válido (ej. `VOO`, `AAPL`, `AMXL.MX`, `CEMEXCPO.MX`) en la barra de búsqueda y haz clic en "Analizar".

---
*Desarrollado como una herramienta de ingeniería financiera y soporte de decisiones.*
