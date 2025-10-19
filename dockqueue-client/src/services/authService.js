export async function login(email, password) {
    const response = await fetch("/api/Login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }

    return response.json();
}
