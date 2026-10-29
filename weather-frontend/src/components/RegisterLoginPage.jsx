import { useState } from "react";

const API_URL = "https://localhost:7263";

function RegisterLoginPage({onLogin}) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState(null);
  const [isRegistering, setRegistering] = useState(true);
  const [isLoggingIn,setLoggingIn] = useState(false);

  async function handleRegister() {
    setSubmitError(null);
    setSubmitting(true);
    try {
      const response = await fetch(`${API_URL}/api/Auth/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          Username: username,
          Password: password,
        }),
      });
      if (!response.ok) {
        const text = await response.text();
        throw new Error(text || `Greška: ${response.status}`);
      }
      setUsername("");
      setPassword("");
    } catch (error) {
      setSubmitError(error.message);
    } finally {
      setSubmitting(false);
    }
  }

  async function handleLogin() {
    setSubmitError(null);
    setSubmitting(true);
    try {
      const response = await fetch(`${API_URL}/api/Auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          Username: username,
          Password: password,
        }),
      });
      if (!response.ok) {
        const text = await response.text();
        throw new Error(text || `Greška: ${response.status}`);
      }
    const data = await response.json();
    onLogin(data.token);
      setUsername("");
      setPassword("");
    } catch (error) {
      setSubmitError(error.message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div>
      <button onClick={()=> {
        setLoggingIn(true);
        setRegistering(false);}}>
          Prijava</button>
      <button onClick={()=>{
        setRegistering(true);
        setLoggingIn(false);
      }}>Registracija</button>
      <input
        type="text"
        value={username}
        onChange={(event) => setUsername(event.target.value)}
        placeholder="Vaše korisničko ime"
      ></input>

      <br></br>

      <input
        type="password"
        value={password}
        onChange={(event) => setPassword(event.target.value)}
        placeholder="Vaša zaporka"
      ></input>

      <br></br>
      {isRegistering && <button onClick={handleRegister} disabled={submitting} >
            {submitting ? "Registriranje..." : "Registriraj se!"}
      </button>}
      <br></br>
      {isLoggingIn && <button onClick={handleLogin} disabled={submitting}>
            {submitting ? "Prijavljivanje..." : "Prijavi se!"}
      </button>}
            {submitError && <p className="message error">{submitError}</p>}
    </div>
  );
}

export default RegisterLoginPage;