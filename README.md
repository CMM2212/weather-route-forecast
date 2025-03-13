
# Dynamic Weather Forecasting for Road Trips

## Project Overview

Making long-distance road trips can be stressful due to unpredictable weather along the route. This project addresses that challenge by integrating GPS routing data with real-time weather forecasts, creating precise weather predictions for travelers throughout their journey.

This full-stack application combines Blazor WebAssembly and Azure Functions, utilizing Azure Maps for route data and Open-Meteo for weather forecasting.

----------

## 📂 Project Structure

```
📦 Weather-Route-Forecast/
├── 📁 Weather.Api        # Azure Functions backend
├── 📁 Weather.Web        # Blazor WebAssembly frontend
├── 📁 docs               # Project documentation and paper
└── 📄 README.md

```

----------

## Technologies Used

-   **Frontend:** Blazor WebAssembly (C#)
-   **Backend:** Azure Functions (C#)
-   **APIs:** Azure Maps (GPS routing), Open-Meteo (weather)
-   **CI/CD:** GitHub Actions (automated deployment)
-   **Mapping:** Leaflet.js for interactive route visualization

----------
## Screenshots
Demonstration of the weather table showing locations/times along the route and the corresponding weather for that location/time:

![Weather Table](docs/forecast-table.png)

Demonstration of weather grid showing the different weather the user would encounter at each location based on their departure time.
The x-axis represents the location along the trip at half hour intervals, the y-axis represents the departure time, and the grid represents the weather at that location given that departure time.
(Wind speed, temperature, precipitation are other alternatives to display in the grid instead of the weather icon)

![Weather Grid](docs/weather-grid.png)

Demonstration of the weather map depicting the route and he weather icons along the way. When scrolled over, the icons show he time you will pass through that location and more details about the weather at that time at that location.

![Weather Map](docs/weather-map.png)


----------

##  Getting Started

###  Prerequisites

-   [.NET SDK 7.0+](https://dotnet.microsoft.com/download)
-   [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)

### Running Locally

**Clone the repository:**

```bash
git clone https://github.com/CMM2212/weather-route-forecast.git
cd weather-route-forecast

```

**Start Backend (Azure Functions):**

```bash
cd Weather.Api
func start

```

**Start Frontend (Blazor WebAssembly):**

```bash
cd ../Weather.Web
dotnet run

```

Open the application at http://localhost:5000/.

----------

## Key Features

-   **Route-Specific Weather:** Accurate 15-minute interval forecasts along travel routes.
-   **Interactive Maps:** Visualize the route and weather conditions dynamically.
-   **Optimized Departure Times:** Adjust your departure to avoid severe weather along your trip.
-   **Efficient Architecture:** Serverless Azure Functions backend provides scalability and cost-effectiveness.
-   **Continuous Deployment:** Automated deployment pipeline via GitHub Actions.

----------

## 📄 Paper and Presentation Slides
Paper and slides for Student Academic Conference presentation:

[ **Read the Full Paper (PDF)**](docs/paper.pdf)

[ **View Slide Presentation (PDF)**](docs/slides.pdf)

