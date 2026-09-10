import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function Callback() {
  const navigate = useNavigate();

  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);
    const code = urlParams.get("code");
    const iss = urlParams.get("iss");

    if (code && iss) {
      fetch("http://localhost:5146/auth/github/callback", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ code, iss }),
      })
        .then((response) => response.json())
        .then((data) => {
          if (data.accessToken) {
            localStorage.setItem("access_token", data.accessToken);
            navigate("/dashboard");
          } else {
            navigate("/");
          }
        })
        .catch(() => navigate("/"));
    } else {
      navigate("/");
    }
  }, []);

  return <div>Processing GitHub login...</div>;
}
