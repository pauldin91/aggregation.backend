import { useState, type JSX } from 'react'
import { BrowserRouter, Routes, Route } from "react-router-dom";
import './App.css'
import Home from './Home';
import Login from './Login';
import Dashboard from './Dashboard';
import Callback from './Callback';

function App() {

  const ProtectedRoute = ({ children }: { children: JSX.Element }) => {
    const token = localStorage.getItem("access_token");

    if (!token) {
      Login
    }

    return children;
  };
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/auth/github/callback" element={<Callback />} />
        <Route path="/dashboard" element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        } />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
