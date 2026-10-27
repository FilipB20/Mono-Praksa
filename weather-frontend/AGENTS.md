# AGENTS.md

## Commands (run from `weather-frontend/`)
- Install: `npm install` (lockfile is committed; use `npm ci` for a clean install)
- Dev server: `npm run dev` -> defaults to `http://localhost:5173`
- Build: `npm run build`
- Lint: `npm run lint`
- No test script or test project exists; use `npm run build` and `npm run lint` to verify changes.

## Backend coupling
- The API base URL is hardcoded in `src/App.jsx` as `https://localhost:7263`. There is no Vite proxy and no environment variable for it.
- Start the backend with its HTTPS profile, or change `API_URL` to match the HTTP profile (`http://localhost:5220`).
- Backend CORS allows only `http://localhost:5173`. If Vite falls back to 5174+ because 5173 is busy, API calls fail CORS; free 5173 or add the origin in `Mono-Praksa/Program.cs`.
- `App.jsx` calls `GET /WeatherForecast` on mount. It sends no bearer token, so protected backend endpoints will return 401.
- The backend requires `Jwt:Key` even for anonymous requests because `UseAuthentication()` initializes JWT auth on every request. If it is missing, the first request from this frontend triggers the backend error.

## Notes
- React 19 + Vite 8, with ESLint's flat config. `node_modules/` and `dist/` are ignored.
- Forecasts are fetched once and filtered client-side by `date.startsWith(...)`; the date input is not sent to the API.
