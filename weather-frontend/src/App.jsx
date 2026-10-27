import { useEffect, useState } from "react";
import "./App.css";

const API_URL = "https://localhost:7263";

function App() {
  // State for the forecast list.
  const [forecasts, setForecasts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [refreshKey, setRefreshKey] = useState(0);

  // State for the date filter.
  const [date, setDate] = useState("");

  // State for the new forecast form.
  const [newDate, setNewDate] = useState("");
  const [newTemperature, setNewTemperature] = useState("");
  const [newSummary, setNewSummary] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState(null);

  // Load forecasts on page load and whenever refreshKey changes.
  useEffect(() => {
    async function loadForecasts() {
      setLoading(true);
      setError(null);

      try {
        const response = await fetch(`${API_URL}/WeatherForecast`);

        if (!response.ok) {
          throw new Error(`Greška: ${response.status}`);
        }

        const data = await response.json();
        setForecasts(data);
      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    }

    loadForecasts();
  }, [refreshKey]);

  // Send a new forecast to the backend.
  async function handleSubmit(event) {
    event.preventDefault();
    setSubmitError(null);
    setSubmitting(true);

    try {
      const response = await fetch(`${API_URL}/WeatherForecast`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          date: newDate,
          temperatureC:
            newTemperature === "" ? null : Number(newTemperature),
          summary: newSummary === "" ? null : newSummary,
        }),
      });

      if (!response.ok) {
        const text = await response.text();
        throw new Error(text || `Greška: ${response.status}`);
      }

      // Clear the form and reload the list.
      setNewDate("");
      setNewTemperature("");
      setNewSummary("");
      setRefreshKey((value) => value + 1);
    } catch (error) {
      setSubmitError(error.message);
    } finally {
      setSubmitting(false);
    }
  }

  // Filter the forecasts in the browser.
  const filteredForecasts = forecasts.filter((forecast) =>
    forecast.date.startsWith(date)
  );

  return (
    <div className="app">
      <header className="app-header">
        <h1>Vremenska prognoza</h1>
        <p>Pregledajte prognoze po datumu ili dodajte novu.</p>
      </header>

      <div className="layout">
        <main className="panel">
          <div className="panel-heading">
            <h2>Prognoze</h2>
            <span className="badge">{filteredForecasts.length}</span>
          </div>

          <div className="field filter-field">
            <label htmlFor="filterDate">Filtriraj po datumu</label>
            <input
              id="filterDate"
              type="date"
              value={date}
              onChange={(event) => setDate(event.target.value)}
            />
          </div>

          <p className="filter-status">
            {date ? `Prikaz za ${date}` : "Prikaz svih prognoza"}
          </p>

          {loading && <p className="message">Učitavanje...</p>}
          {error && <p className="message error">{error}</p>}

          {!loading && !error && filteredForecasts.length === 0 && (
            <p className="empty-state">Nema prognoza za odabrani datum.</p>
          )}

          {!loading && !error && filteredForecasts.length > 0 && (
            <ul className="forecast-list">
              {filteredForecasts.map((forecast) => (
                <li className="forecast-item" key={forecast.id}>
                  <span className="forecast-date">{forecast.date}</span>
                  <span className="forecast-temp">
                    {forecast.temperatureC == null
                      ? "–"
                      : `${forecast.temperatureC}°C`}
                  </span>
                  <span className="forecast-summary">
                    {forecast.summary || "Bez opisa"}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </main>

        <aside className="panel panel-form">
          <div className="panel-heading">
            <h2>Dodaj prognozu</h2>
          </div>

          <p className="panel-description">
            Unesite datum, temperaturu i kratki opis.
          </p>

          <form className="forecast-form" onSubmit={handleSubmit}>
            <div className="field">
              <label htmlFor="newDate">Datum</label>
              <input
                id="newDate"
                type="date"
                value={newDate}
                onChange={(event) => setNewDate(event.target.value)}
                required
              />
            </div>

            <div className="field">
              <label htmlFor="newTemperature">Temperatura (°C)</label>
              <input
                id="newTemperature"
                type="number"
                value={newTemperature}
                onChange={(event) => setNewTemperature(event.target.value)}
                placeholder="npr. 21"
              />
            </div>

            <div className="field">
              <label htmlFor="newSummary">Opis</label>
              <input
                id="newSummary"
                type="text"
                value={newSummary}
                onChange={(event) => setNewSummary(event.target.value)}
                placeholder="npr. Sunčano"
                maxLength={100}
              />
            </div>

            <button
              className="submit-button"
              type="submit"
              disabled={submitting}
            >
              {submitting ? "Dodavanje..." : "Dodaj prognozu"}
            </button>
          </form>

          {submitError && <p className="message error">{submitError}</p>}
        </aside>
      </div>
    </div>
  );
}

export default App;
