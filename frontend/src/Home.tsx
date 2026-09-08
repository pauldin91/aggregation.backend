import { useEffect, useState } from "react";
import Login from "./Login";
import Dashboard from "./Dashboard";

const Home = () => {
  const [auth, setAuth] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("auth_token");
    if (token) setAuth(true);
  }, []);

  useEffect(() => {
    document.body.style.overflow = modalOpen ? "hidden" : "";
  }, [modalOpen]);

  if (error) return <p>{error}</p>;

  if (!auth) {
    return <Login />;
  }

  return (
    <>
      <div
        style={{
          display: "flex",
          alignItems: "center",
          gap: "0.75rem",
          marginBottom: "1rem",
        }}
      >
        <h2 style={{ margin: 0 }}>Batches</h2>
        <button
          onClick={() => setModalOpen(true)}
          title="New batch"
          style={{
            width: 28,
            height: 28,
            borderRadius: "50%",
            border: "1.5px solid #1a1612",
            background: "none",
            fontSize: "1.1rem",
            cursor: "pointer",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            lineHeight: 1,
          }}
        >
          +
        </button>
      </div>

      <Dashboard/>
    </>
  );
};

export default Home;
