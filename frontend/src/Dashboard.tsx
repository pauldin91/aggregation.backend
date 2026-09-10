export default function Dashboard() {
  const token = localStorage.getItem("access_token");

  return (
    <div>
      <h1>Dashboard</h1>
      <p>{token}</p>
    </div>
  );
}
