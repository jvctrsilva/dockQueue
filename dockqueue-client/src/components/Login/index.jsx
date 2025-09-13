import React, { useState } from "react";
import styles from "./Login.module.css";
import { login } from "../../services/authService";

const Login = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        try {
            const user = await login(email, password);
            console.log("Usuário logado:", user);
            alert(`Bem-vindo ${user.name || user.email}`);
            // Aqui você pode salvar token no localStorage e redirecionar
        } catch (err) {
            setError(err.message);
        }
    };

    return (
        <div className={styles.container}>
            <form className={styles.loginForm} onSubmit={handleSubmit}>
                <h2 className={styles.title}>Login</h2>
                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className={styles.input}
                />
                <input
                    type="password"
                    placeholder="Senha"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className={styles.input}
                />
                <button type="submit" className={styles.button}>
                    Entrar
                </button>
                {error && <p style={{ color: "red", marginTop: "10px" }}>{error}</p>}
            </form>
        </div>
    );
};

export default Login;
