import {useState} from "react";

function EnterForecastDate({ date, onDateChange }) {
  return (
    <div>
      <label htmlFor="forecastDate">
        Unesite datum za koji želite vremensku prognozu:
      </label>
      <br />
      <input
        type="date"
        id="forecastDate"
        value={date}
        onChange={(e) => onDateChange(e.target.value)}
      />
    </div>
  );
}

function Counter(){
  const [count, setCount] = useState(0);
  return(
    <div>
      <p>Vrijednost je: {count}</p>
      <button onClick={() => setCount(count+1)} disabled={count==10}>+1</button>
      <button onClick={() => setCount(0)}>RESET</button>
      <button onClick={() => setCount(count-1)} disabled={count == 0}>-1</button>
      <br></br>
      {count > 8 && <p>Pazi,10 je max!</p>}
    </div>
  )
}

function App() {
  const [date,setDate] = useState("");

  return (
    <div>
      <h1>Izvještaji vremenske prognoze</h1>
      <EnterForecastDate date={date} onDateChange={setDate}/>
      <br></br>
      <p>Odabrani datum: {date}</p>
      <br></br>
      <Counter />
    </div>
  );
}

export default App;