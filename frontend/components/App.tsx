'use client'

import { Button } from "@/components/ui/button"

import { useEffect, useState } from "react"

export default function App() {
  const [weatherForecasts, setWeatherForecasts] = useState<WeatherForecast[]>(
    []
  )

  interface WeatherForecast {
    date: string
    temperatureC: number
    summary: string
  }

  useEffect(() => {
    fetch("http://localhost:5056/weatherforecast")
      .then((res) => res.json())
      .then((data) => {
        console.log(data)
        setWeatherForecasts(data)
      })
  }, [])

  return (
    <div className="App">
      <ul>
        {weatherForecasts.map((forecast, index) => (
          <li key={index}>
            {forecast.date} &nbsp; &nbsp; {forecast.temperatureC}&deg;C &nbsp;
            &nbsp; Feels: {forecast.summary}
          </li>
        ))}
      </ul>
      <div>
        <Button>This is how the Button looks</Button>
      </div>
    </div>
  )
}
