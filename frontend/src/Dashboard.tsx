import Home from "./Home";

export default function Dashboard() {
  const token = localStorage.getItem("access_token");
  console.log(token);
  return token ? (
    <div>
      <h1>Dashboard</h1>
    </div>
  ) : (
    <Home />
  );
}
