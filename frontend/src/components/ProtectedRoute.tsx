import { Navigate } from "react-router-dom";
import type { JSX } from "react/jsx-runtime";

const ProtectedRoute = ({ children }: { children: JSX.Element }) => {
  const token = localStorage.getItem("access_token");
  return token ? children : <Navigate to="/" replace />;
};

export default ProtectedRoute;
